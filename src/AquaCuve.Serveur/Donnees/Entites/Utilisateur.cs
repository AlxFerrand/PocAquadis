using Microsoft.AspNetCore.Identity;

namespace AquaCuve.Serveur.Donnees.Entites;

/// <summary>
/// Compte de connexion (ASP.NET Core Identity), rattaché à un agriculteur ou sans rattachement pour un Admin.
/// </summary>
public sealed class Utilisateur : IdentityUser
{
    /// <summary>Identifiant de l'agriculteur rattaché, ou null pour un Admin.</summary>
    public int? AgriculteurId { get; set; }

    /// <summary>Agriculteur rattaché, ou null pour un Admin.</summary>
    public Agriculteur? Agriculteur { get; set; }
}
