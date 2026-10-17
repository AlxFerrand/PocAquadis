namespace AquaCuve.Partage.Contrats;

/// <summary>
/// Résumé d'une installation (cuve) pour les listes.
/// </summary>
/// <param name="Id">Identifiant de l'installation.</param>
/// <param name="Nom">Nom de la cuve.</param>
/// <param name="AgriculteurId">Identifiant de l'agriculteur propriétaire.</param>
/// <param name="NomAgriculteur">Nom de l'agriculteur propriétaire.</param>
public sealed record InstallationResumeDto(
    int Id,
    string Nom,
    int AgriculteurId,
    string NomAgriculteur);
