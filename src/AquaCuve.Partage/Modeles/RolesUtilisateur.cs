namespace AquaCuve.Partage.Modeles;

/// <summary>
/// Noms des rôles de l'application, partagés entre le serveur (autorisation) et le client (affichage).
/// </summary>
public static class RolesUtilisateur
{
    /// <summary>
    /// Rôle de l'administrateur Aquadis : accès à toutes les installations.
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Rôle de l'agriculteur : accès uniquement à ses propres installations.
    /// </summary>
    public const string Agriculteur = "Agriculteur";
}
