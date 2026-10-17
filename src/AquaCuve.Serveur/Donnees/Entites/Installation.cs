namespace AquaCuve.Serveur.Donnees.Entites;

/// <summary>
/// Installation d'une cuve de stockage avec ses paramètres de réglage.
/// </summary>
public sealed class Installation
{
    /// <summary>Identifiant de l'installation.</summary>
    public int Id { get; set; }

    /// <summary>Nom de la cuve affiché dans l'application.</summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>Identifiant de l'agriculteur propriétaire.</summary>
    public int AgriculteurId { get; set; }

    /// <summary>Agriculteur propriétaire.</summary>
    public Agriculteur Agriculteur { get; set; } = null!;

    /// <summary>Hauteur d'eau correspondant à 0 % (fond de cuve), en centimètres.</summary>
    public double HauteurMiniCm { get; set; }

    /// <summary>Hauteur d'eau correspondant à 100 % (cuve pleine), en centimètres.</summary>
    public double HauteurMaxiCm { get; set; }

    /// <summary>Sous ce niveau (en % de la hauteur), remplissage par l'eau de forage.</summary>
    public double SeuilForagePourcent { get; set; }

    /// <summary>Sous ce niveau (en % de la hauteur), bascule sur l'eau de ville.</summary>
    public double SeuilVillePourcent { get; set; }

    /// <summary>Sous ce niveau (en % de la hauteur), sécurité manque d'eau.</summary>
    public double SeuilSecuritePourcent { get; set; }

    /// <summary>Option d'alerte : prévenir au passage sur l'eau de ville (affichage seul dans le POC).</summary>
    public bool AlerteSmsPassageVilleActive { get; set; }

    /// <summary>Option d'alerte : prévenir en cas de manque d'eau (affichage seul dans le POC).</summary>
    public bool AlerteSmsManqueEauActive { get; set; }

    /// <summary>Numéro de téléphone pour les alertes, s'il est renseigné.</summary>
    public string? Telephone { get; set; }

    /// <summary>Adresse e-mail pour les alertes, si elle est renseignée.</summary>
    public string? Courriel { get; set; }

    /// <summary>Coffret (simulé) installé sur la cuve.</summary>
    public Coffret? Coffret { get; set; }
}
