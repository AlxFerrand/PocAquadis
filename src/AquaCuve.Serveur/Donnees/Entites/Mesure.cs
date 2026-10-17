namespace AquaCuve.Serveur.Donnees.Entites;

/// <summary>
/// Mesure remontée par un coffret : niveau, compteurs, débit et état des sorties.
/// La conversion des impulsions en volume sera précisée à l'étape 4 avec le simulateur.
/// </summary>
public sealed class Mesure
{
    /// <summary>Identifiant de la mesure.</summary>
    public long Id { get; set; }

    /// <summary>Identifiant du coffret qui a émis la mesure.</summary>
    public int CoffretId { get; set; }

    /// <summary>Coffret qui a émis la mesure.</summary>
    public Coffret Coffret { get; set; } = null!;

    /// <summary>Date et heure de la mesure, en UTC.</summary>
    public DateTime HorodatageUtc { get; set; }

    /// <summary>Hauteur d'eau mesurée, en centimètres.</summary>
    public double NiveauCm { get; set; }

    /// <summary>Compteur cumulé d'impulsions de l'eau de forage.</summary>
    public long CompteurForageImpulsions { get; set; }

    /// <summary>Compteur cumulé d'impulsions de l'eau de ville.</summary>
    public long CompteurVilleImpulsions { get; set; }

    /// <summary>Débit de remplissage, en mètres cubes par heure.</summary>
    public double DebitM3ParHeure { get; set; }

    /// <summary>Vrai si l'électrovanne de forage était ouverte.</summary>
    public bool ForageActif { get; set; }

    /// <summary>Vrai si l'électrovanne d'eau de ville était ouverte.</summary>
    public bool VilleActive { get; set; }

    /// <summary>Vrai si la sécurité manque d'eau était activée.</summary>
    public bool SecuriteManqueEau { get; set; }
}
