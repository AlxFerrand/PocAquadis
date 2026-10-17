using AquaCuve.Partage.Modeles;
using AquaCuve.Partage.Regles;

namespace AquaCuve.Tests.Regles;

/// <summary>
/// Tests de la règle de pilotage du remplissage (forage, ville, sécurité manque d'eau) avec hystérésis.
/// Seuils utilisés : forage 90 %, ville 50 %, sécurité 10 %.
/// </summary>
public class RegleRemplissageTests
{
    /// <summary>
    /// Fournit les seuils standard des tests : forage 90 %, ville 50 %, sécurité 10 %.
    /// </summary>
    /// <returns>Les seuils de remplissage standard.</returns>
    private static SeuilsRemplissage CreerSeuilsStandard()
    {
        return new SeuilsRemplissage(SeuilForagePourcent: 90, SeuilVillePourcent: 50, SeuilSecuritePourcent: 10);
    }

    /// <summary>
    /// Vérifie l'état des sorties selon le niveau et l'état précédent, y compris aux seuils exacts et en hystérésis.
    /// </summary>
    /// <param name="pourcentage">Niveau actuel de la cuve, en % de la hauteur.</param>
    /// <param name="foragePrecedent">Vrai si le forage était actif au cycle précédent.</param>
    /// <param name="villePrecedente">Vrai si l'eau de ville était active au cycle précédent.</param>
    /// <param name="forageAttendu">Vrai si le forage doit être actif.</param>
    /// <param name="villeAttendue">Vrai si l'eau de ville doit être active.</param>
    /// <param name="securiteAttendue">Vrai si la sécurité manque d'eau doit être active.</param>
    /// <returns>Rien : le test échoue si l'état des sorties diffère de l'attendu.</returns>
    [Theory]
    // Cuve bien remplie, tout est à l'arrêt
    [InlineData(95, false, false, false, false, false)]
    // Sous le seuil forage : le forage démarre
    [InlineData(85, false, false, true, false, false)]
    // Seuil forage exact : pas de démarrage (comparaison stricte)
    [InlineData(90, false, false, false, false, false)]
    // Hystérésis : le forage continue au-dessus de son seuil de démarrage
    [InlineData(95, true, false, true, false, false)]
    // Cuve pleine : le forage s'arrête
    [InlineData(100, true, false, false, false, false)]
    // Sous le seuil ville : bascule sur l'eau de ville, le forage s'arrête
    [InlineData(40, true, false, false, true, false)]
    // Seuil ville exact : pas de bascule
    [InlineData(50, false, false, true, false, false)]
    // Hystérésis : la ville continue tant que le seuil forage n'est pas atteint
    [InlineData(70, false, true, false, true, false)]
    // Seuil forage exact : la ville s'arrête et le forage ne démarre pas encore
    [InlineData(90, false, true, false, false, false)]
    // Au-dessus du seuil forage : la ville s'arrête
    [InlineData(95, false, true, false, false, false)]
    // Sous le seuil sécurité : sécurité active, la ville remplit
    [InlineData(5, false, false, false, true, true)]
    // Seuil sécurité exact : sécurité inactive
    [InlineData(10, false, false, false, true, false)]
    public void DeterminerEtatSorties_NiveauEtEtatPrecedent_RetourneLEtatAttendu(
        double pourcentage,
        bool foragePrecedent,
        bool villePrecedente,
        bool forageAttendu,
        bool villeAttendue,
        bool securiteAttendue)
    {
        // Organiser
        SeuilsRemplissage seuils = CreerSeuilsStandard();
        EtatSorties etatPrecedent = new(foragePrecedent, villePrecedente, false);

        // Agir
        EtatSorties resultat = RegleRemplissage.DeterminerEtatSorties(pourcentage, seuils, etatPrecedent);

        // Vérifier
        Assert.Equal(new EtatSorties(forageAttendu, villeAttendue, securiteAttendue), resultat);
    }

    /// <summary>
    /// Vérifie l'hystérésis de la sécurité manque d'eau : elle démarre sous le seuil sécurité et ne s'arrête qu'au seuil ville.
    /// </summary>
    /// <param name="pourcentage">Niveau actuel de la cuve, en % de la hauteur.</param>
    /// <param name="securitePrecedente">Vrai si la sécurité manque d'eau était active au cycle précédent.</param>
    /// <param name="securiteAttendue">Vrai si la sécurité manque d'eau doit être active.</param>
    /// <returns>Rien : le test échoue si l'état de la sécurité diffère de l'attendu.</returns>
    [Theory]
    // Sous le seuil sécurité : elle s'active
    [InlineData(5, false, true)]
    // Seuil sécurité exact : pas d'activation (comparaison stricte)
    [InlineData(10, false, false)]
    // Hystérésis : elle reste active au-dessus du seuil sécurité
    [InlineData(30, true, true)]
    // Seuil ville exact : elle s'arrête
    [InlineData(50, true, false)]
    // Au-dessus du seuil ville : elle reste arrêtée
    [InlineData(70, true, false)]
    public void DeterminerEtatSorties_SecuriteManqueEau_AppliqueLHysteresis(
        double pourcentage, bool securitePrecedente, bool securiteAttendue)
    {
        // Organiser
        SeuilsRemplissage seuils = CreerSeuilsStandard();
        EtatSorties etatPrecedent = new(false, false, securitePrecedente);

        // Agir
        EtatSorties resultat = RegleRemplissage.DeterminerEtatSorties(pourcentage, seuils, etatPrecedent);

        // Vérifier
        Assert.Equal(securiteAttendue, resultat.SecuriteManqueEau);
    }

    /// <summary>
    /// Vérifie qu'un pourcentage hors de 0 à 100 est refusé.
    /// </summary>
    /// <param name="pourcentage">Pourcentage invalide à tester.</param>
    /// <returns>Rien : le test échoue si aucune exception n'est levée.</returns>
    [Theory]
    [InlineData(-1)]
    [InlineData(100.5)]
    [InlineData(double.NaN)]
    public void DeterminerEtatSorties_PourcentageHorsPlage_LeveUneException(double pourcentage)
    {
        // Organiser
        SeuilsRemplissage seuils = CreerSeuilsStandard();

        // Agir et Vérifier
        Assert.Throws<ArgumentException>(
            () => RegleRemplissage.DeterminerEtatSorties(pourcentage, seuils, EtatSorties.Repos));
    }

    /// <summary>
    /// Vérifie que des seuils mal ordonnés sont refusés.
    /// </summary>
    /// <returns>Rien : le test échoue si aucune exception n'est levée.</returns>
    [Fact]
    public void DeterminerEtatSorties_SeuilsMalOrdonnes_LeveUneException()
    {
        // Organiser
        SeuilsRemplissage seuilsInverses = new(SeuilForagePourcent: 10, SeuilVillePourcent: 50, SeuilSecuritePourcent: 90);

        // Agir et Vérifier
        Assert.Throws<ArgumentException>(
            () => RegleRemplissage.DeterminerEtatSorties(60, seuilsInverses, EtatSorties.Repos));
    }

    /// <summary>
    /// Vérifie le scénario complet d'une cuve qui se vide puis se remplit : forage, ville, puis retour au forage.
    /// </summary>
    /// <returns>Rien : le test échoue si l'enchaînement des états est incorrect.</returns>
    [Fact]
    public void DeterminerEtatSorties_CuveQuiSeVidePuisSeRemplit_EnchaineForageVilleForage()
    {
        // Organiser
        SeuilsRemplissage seuils = CreerSeuilsStandard();

        // Agir
        EtatSorties apresForage = RegleRemplissage.DeterminerEtatSorties(85, seuils, EtatSorties.Repos);
        EtatSorties apresBaisse = RegleRemplissage.DeterminerEtatSorties(45, seuils, apresForage);
        EtatSorties apresRemontee = RegleRemplissage.DeterminerEtatSorties(80, seuils, apresBaisse);
        EtatSorties apresSeuilForage = RegleRemplissage.DeterminerEtatSorties(92, seuils, apresRemontee);

        // Vérifier
        Assert.True(apresForage.ForageActif);
        Assert.True(apresBaisse.VilleActive);
        Assert.False(apresBaisse.ForageActif);
        Assert.True(apresRemontee.VilleActive);
        Assert.False(apresSeuilForage.VilleActive);
        Assert.False(apresSeuilForage.ForageActif);
    }
}
