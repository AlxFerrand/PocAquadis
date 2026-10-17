using AquaCuve.Partage.Modeles;

namespace AquaCuve.Serveur.Donnees.Entites;

/// <summary>
/// Alerte levée sur une installation (affichage dans l'application uniquement).
/// </summary>
public sealed class Alerte
{
    /// <summary>Identifiant de l'alerte.</summary>
    public int Id { get; set; }

    /// <summary>Identifiant de l'installation concernée.</summary>
    public int InstallationId { get; set; }

    /// <summary>Installation concernée.</summary>
    public Installation Installation { get; set; } = null!;

    /// <summary>Type de l'alerte.</summary>
    public TypeAlerte Type { get; set; }

    /// <summary>Date et heure de l'alerte, en UTC.</summary>
    public DateTime HorodatageUtc { get; set; }
}
