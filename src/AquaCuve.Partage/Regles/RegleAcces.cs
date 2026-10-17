namespace AquaCuve.Partage.Regles;

/// <summary>
/// Règle d'accès aux installations selon le profil de l'utilisateur.
/// </summary>
public static class RegleAcces
{
    /// <summary>
    /// Dit si un utilisateur a le droit de consulter une installation.
    /// </summary>
    /// <remarks>
    /// Un Admin accède à toutes les installations ; un agriculteur uniquement aux siennes.
    /// Un compte sans agriculteur rattaché (et qui n'est pas Admin) n'a accès à rien.
    /// </remarks>
    /// <param name="estAdmin">Vrai si l'utilisateur a le rôle Admin.</param>
    /// <param name="agriculteurIdUtilisateur">Identifiant de l'agriculteur rattaché à l'utilisateur, ou null s'il n'en a pas.</param>
    /// <param name="agriculteurIdInstallation">Identifiant de l'agriculteur propriétaire de l'installation.</param>
    /// <returns>Vrai si l'accès est autorisé.</returns>
    public static bool PeutAccederInstallation(
        bool estAdmin, int? agriculteurIdUtilisateur, int agriculteurIdInstallation)
    {
        return estAdmin || agriculteurIdUtilisateur == agriculteurIdInstallation;
    }
}
