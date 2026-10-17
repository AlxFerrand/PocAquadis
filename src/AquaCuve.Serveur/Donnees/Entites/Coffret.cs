namespace AquaCuve.Serveur.Donnees.Entites;

/// <summary>
/// Coffret de suivi installé sur une cuve (simulé dans le POC, réel et connecté en MQTT plus tard).
/// </summary>
public sealed class Coffret
{
    /// <summary>Identifiant technique du coffret.</summary>
    public int Id { get; set; }

    /// <summary>Identifiant fonctionnel du coffret (unique), par exemple son numéro de série.</summary>
    public string Identifiant { get; set; } = string.Empty;

    /// <summary>Identifiant de l'installation équipée du coffret.</summary>
    public int InstallationId { get; set; }

    /// <summary>Installation équipée du coffret.</summary>
    public Installation Installation { get; set; } = null!;

    /// <summary>Date et heure (UTC) de la dernière mesure reçue, ou null si le coffret n'a jamais communiqué.</summary>
    public DateTime? DernierContactUtc { get; set; }
}
