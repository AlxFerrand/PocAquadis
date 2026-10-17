namespace AquaCuve.Serveur.Configuration;

/// <summary>
/// Comptes de démonstration créés au démarrage (section « ComptesDemo » de la configuration).
/// </summary>
public sealed class OptionsComptesDemo
{
    /// <summary>
    /// Nom de la section de configuration.
    /// </summary>
    public const string NomSection = "ComptesDemo";

    /// <summary>
    /// Compte de l'administrateur Aquadis.
    /// </summary>
    public CompteDemo Admin { get; set; } = new();

    /// <summary>
    /// Compte de l'agriculteur de démonstration.
    /// </summary>
    public CompteDemo Agriculteur { get; set; } = new();
}
