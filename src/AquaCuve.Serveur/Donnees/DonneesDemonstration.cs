using AquaCuve.Partage.Modeles;
using AquaCuve.Partage.Regles;
using AquaCuve.Serveur.Donnees.Entites;

namespace AquaCuve.Serveur.Donnees;

/// <summary>
/// Fabrique des données de démonstration (agriculteur, cuves aux profils différents, coffrets), sans accès à la base.
/// </summary>
public static class DonneesDemonstration
{
    private const string NomAgriculteur = "Ferme Le Goff";
    private const string TelephoneDemonstration = "06 00 00 00 00";
    private const string CourrielDemonstration = "contact@ferme-legoff.example";

    /// <summary>
    /// Décrit le profil d'une cuve de démonstration.
    /// </summary>
    /// <param name="Nom">Nom de la cuve.</param>
    /// <param name="HauteurMiniCm">Hauteur correspondant à 0 %, en centimètres.</param>
    /// <param name="HauteurMaxiCm">Hauteur correspondant à 100 %, en centimètres.</param>
    /// <param name="Seuils">Seuils de remplissage de la cuve.</param>
    /// <param name="IdentifiantCoffret">Identifiant du coffret installé sur la cuve.</param>
    private sealed record ProfilCuve(
        string Nom,
        double HauteurMiniCm,
        double HauteurMaxiCm,
        SeuilsRemplissage Seuils,
        string IdentifiantCoffret);

    /// <summary>
    /// Liste les profils des cuves de démonstration (hauteurs et seuils volontairement différents).
    /// </summary>
    /// <returns>Les trois profils de cuve : bovins, volailles et porcs.</returns>
    private static List<ProfilCuve> ListerProfils()
    {
        return
        [
            new ProfilCuve("Cuve bovins", 0, 500, new SeuilsRemplissage(90, 50, 10), "COFFRET-001"),
            new ProfilCuve("Cuve volailles", 0, 300, new SeuilsRemplissage(85, 40, 15), "COFFRET-002"),
            new ProfilCuve("Cuve porcs", 20, 450, new SeuilsRemplissage(90, 60, 20), "COFFRET-003"),
        ];
    }

    /// <summary>
    /// Crée l'agriculteur de démonstration avec ses trois installations et leurs coffrets.
    /// </summary>
    /// <param name="maintenantUtc">Date et heure actuelles en UTC, utilisées comme dernier contact des coffrets.</param>
    /// <returns>L'agriculteur de démonstration, prêt à être enregistré en base.</returns>
    public static Agriculteur CreerAgriculteur(DateTime maintenantUtc)
    {
        return new Agriculteur
        {
            Nom = NomAgriculteur,
            Installations = ListerProfils()
                .Select(profil => CreerInstallation(profil, maintenantUtc))
                .ToList(),
        };
    }

    /// <summary>
    /// Crée une installation de démonstration à partir d'un profil, après avoir vérifié la cohérence de ses paramètres.
    /// </summary>
    /// <param name="profil">Profil de la cuve (nom, hauteurs, seuils, coffret).</param>
    /// <param name="maintenantUtc">Date et heure actuelles en UTC, utilisées comme dernier contact du coffret.</param>
    /// <returns>L'installation avec son coffret.</returns>
    /// <exception cref="ArgumentException">Si les hauteurs ou les seuils du profil sont incohérents.</exception>
    private static Installation CreerInstallation(ProfilCuve profil, DateTime maintenantUtc)
    {
        RegleParametres.Verifier(
            new ParametresInstallation(profil.HauteurMiniCm, profil.HauteurMaxiCm, profil.Seuils));

        return new Installation
        {
            Nom = profil.Nom,
            HauteurMiniCm = profil.HauteurMiniCm,
            HauteurMaxiCm = profil.HauteurMaxiCm,
            SeuilForagePourcent = profil.Seuils.SeuilForagePourcent,
            SeuilVillePourcent = profil.Seuils.SeuilVillePourcent,
            SeuilSecuritePourcent = profil.Seuils.SeuilSecuritePourcent,
            AlerteSmsPassageVilleActive = true,
            AlerteSmsManqueEauActive = true,
            Telephone = TelephoneDemonstration,
            Courriel = CourrielDemonstration,
            Coffret = new Coffret
            {
                Identifiant = profil.IdentifiantCoffret,
                DernierContactUtc = maintenantUtc,
            },
        };
    }
}
