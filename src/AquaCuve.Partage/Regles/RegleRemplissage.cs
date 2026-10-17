using AquaCuve.Partage.Modeles;

namespace AquaCuve.Partage.Regles;

/// <summary>
/// Règles de pilotage du remplissage d'une cuve (eau de forage, eau de ville, sécurité manque d'eau).
/// </summary>
/// <remarks>
/// Règle issue du cahier des charges : quand le forage ne suffit plus à remplir la cuve, on passe sur l'eau de ville.
/// Pour éviter que les électrovannes ne battent autour d'un seuil, la décision tient compte de l'état précédent (hystérésis).
/// </remarks>
public static class RegleRemplissage
{
    /// <summary>
    /// Détermine l'état des sorties de remplissage à partir du niveau actuel et de l'état précédent.
    /// </summary>
    /// <param name="pourcentage">Niveau actuel de la cuve, de 0 à 100 (en % de la hauteur).</param>
    /// <param name="seuils">Seuils de remplissage paramétrés pour l'installation.</param>
    /// <param name="etatPrecedent">État des sorties au cycle précédent (utiliser <see cref="EtatSorties.Repos"/> au démarrage).</param>
    /// <returns>Le nouvel état des sorties : forage, ville et sécurité manque d'eau.</returns>
    /// <exception cref="ArgumentException">Si le pourcentage est hors de 0 à 100 ou si les seuils sont incohérents.</exception>
    public static EtatSorties DeterminerEtatSorties(
        double pourcentage, SeuilsRemplissage seuils, EtatSorties etatPrecedent)
    {
        VerificationsEntrees.VerifierPourcentage(pourcentage, nameof(pourcentage));
        RegleParametres.VerifierSeuils(seuils);

        bool villeActive = DeterminerVilleActive(pourcentage, seuils, etatPrecedent);
        bool forageActif = !villeActive && DeterminerForageActif(pourcentage, seuils, etatPrecedent);
        bool securiteManqueEau = DeterminerSecuriteManqueEau(pourcentage, seuils);

        return new EtatSorties(forageActif, villeActive, securiteManqueEau);
    }

    /// <summary>
    /// Dit si l'eau de ville doit remplir la cuve : elle démarre sous le seuil ville et continue jusqu'au seuil forage.
    /// </summary>
    /// <param name="pourcentage">Niveau actuel de la cuve, en % de la hauteur.</param>
    /// <param name="seuils">Seuils de remplissage paramétrés.</param>
    /// <param name="etatPrecedent">État des sorties au cycle précédent.</param>
    /// <returns>Vrai si l'électrovanne d'eau de ville doit être ouverte.</returns>
    private static bool DeterminerVilleActive(
        double pourcentage, SeuilsRemplissage seuils, EtatSorties etatPrecedent)
    {
        bool sousSeuilVille = pourcentage < seuils.SeuilVillePourcent;
        bool poursuitJusquAuSeuilForage = etatPrecedent.VilleActive
            && pourcentage < seuils.SeuilForagePourcent;

        return sousSeuilVille || poursuitJusquAuSeuilForage;
    }

    /// <summary>
    /// Dit si l'eau de forage doit remplir la cuve : elle démarre sous le seuil forage et continue jusqu'à la cuve pleine.
    /// </summary>
    /// <param name="pourcentage">Niveau actuel de la cuve, en % de la hauteur.</param>
    /// <param name="seuils">Seuils de remplissage paramétrés.</param>
    /// <param name="etatPrecedent">État des sorties au cycle précédent.</param>
    /// <returns>Vrai si l'électrovanne de forage doit être ouverte (hors priorité de l'eau de ville).</returns>
    private static bool DeterminerForageActif(
        double pourcentage, SeuilsRemplissage seuils, EtatSorties etatPrecedent)
    {
        bool sousSeuilForage = pourcentage < seuils.SeuilForagePourcent;
        bool poursuitJusquALaCuvePleine = etatPrecedent.ForageActif
            && pourcentage < RegleNiveauCuve.PourcentageMaximum;

        return sousSeuilForage || poursuitJusquALaCuvePleine;
    }

    /// <summary>
    /// Dit si la sécurité manque d'eau doit être activée (sans hystérésis : sortie de sécurité).
    /// </summary>
    /// <param name="pourcentage">Niveau actuel de la cuve, en % de la hauteur.</param>
    /// <param name="seuils">Seuils de remplissage paramétrés.</param>
    /// <returns>Vrai si le niveau est strictement inférieur au seuil de sécurité.</returns>
    private static bool DeterminerSecuriteManqueEau(double pourcentage, SeuilsRemplissage seuils)
    {
        return pourcentage < seuils.SeuilSecuritePourcent;
    }
}
