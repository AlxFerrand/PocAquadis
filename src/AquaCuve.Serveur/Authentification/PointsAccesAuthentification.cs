using System.Security.Claims;
using AquaCuve.Partage.Contrats;
using AquaCuve.Serveur.Donnees;
using AquaCuve.Serveur.Donnees.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AquaCuve.Serveur.Authentification;

/// <summary>
/// Points d'accès de l'API pour se connecter, se déconnecter et connaître l'utilisateur courant.
/// </summary>
public static class PointsAccesAuthentification
{
    /// <summary>
    /// Déclare les routes /api/auth/connexion, /api/auth/deconnexion et /api/auth/moi.
    /// </summary>
    /// <param name="routes">Table de routage de l'application.</param>
    /// <returns>La même table de routage, pour enchaîner les appels.</returns>
    public static IEndpointRouteBuilder MapPointsAccesAuthentification(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder groupe = routes.MapGroup("/api/auth");

        groupe.MapPost("/connexion", ConnecterAsync);
        groupe.MapPost("/deconnexion", DeconnecterAsync).RequireAuthorization();
        groupe.MapGet("/moi", ObtenirUtilisateurCourantAsync).RequireAuthorization();

        return routes;
    }

    /// <summary>
    /// Ouvre une session (cookie) si le courriel et le mot de passe sont corrects.
    /// Le compte est verrouillé temporairement après plusieurs échecs consécutifs.
    /// </summary>
    /// <param name="demande">Courriel et mot de passe envoyés par le client.</param>
    /// <param name="gestionnaireUtilisateurs">Gestionnaire des comptes Identity.</param>
    /// <param name="gestionnaireConnexion">Gestionnaire de connexion Identity (pose le cookie).</param>
    /// <param name="contexte">Contexte de la base de données.</param>
    /// <returns>200 avec l'utilisateur connecté, 400 si la demande est incomplète, 401 si les identifiants sont refusés.</returns>
    private static async Task<IResult> ConnecterAsync(
        DemandeConnexionDto demande,
        UserManager<Utilisateur> gestionnaireUtilisateurs,
        SignInManager<Utilisateur> gestionnaireConnexion,
        ContexteAquaCuve contexte)
    {
        if (string.IsNullOrWhiteSpace(demande.Courriel) || string.IsNullOrEmpty(demande.MotDePasse))
        {
            return Results.BadRequest("Le courriel et le mot de passe sont obligatoires.");
        }

        Utilisateur? utilisateur = await gestionnaireUtilisateurs.FindByEmailAsync(demande.Courriel);
        if (utilisateur is null)
        {
            return Results.Unauthorized();
        }

        SignInResult resultat = await gestionnaireConnexion.PasswordSignInAsync(
            utilisateur, demande.MotDePasse, isPersistent: true, lockoutOnFailure: true);
        if (!resultat.Succeeded)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(await ConstruireUtilisateurConnecteAsync(utilisateur, gestionnaireUtilisateurs, contexte));
    }

    /// <summary>
    /// Ferme la session de l'utilisateur courant.
    /// </summary>
    /// <param name="gestionnaireConnexion">Gestionnaire de connexion Identity (supprime le cookie).</param>
    /// <returns>204 une fois la session fermée.</returns>
    private static async Task<IResult> DeconnecterAsync(SignInManager<Utilisateur> gestionnaireConnexion)
    {
        await gestionnaireConnexion.SignOutAsync();
        return Results.NoContent();
    }

    /// <summary>
    /// Renvoie les informations de l'utilisateur actuellement connecté.
    /// </summary>
    /// <param name="principal">Identité de l'utilisateur lue dans le cookie de session.</param>
    /// <param name="gestionnaireUtilisateurs">Gestionnaire des comptes Identity.</param>
    /// <param name="contexte">Contexte de la base de données.</param>
    /// <returns>200 avec l'utilisateur connecté, 401 si le compte n'existe plus.</returns>
    private static async Task<IResult> ObtenirUtilisateurCourantAsync(
        ClaimsPrincipal principal,
        UserManager<Utilisateur> gestionnaireUtilisateurs,
        ContexteAquaCuve contexte)
    {
        Utilisateur? utilisateur = await gestionnaireUtilisateurs.GetUserAsync(principal);
        if (utilisateur is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(await ConstruireUtilisateurConnecteAsync(utilisateur, gestionnaireUtilisateurs, contexte));
    }

    /// <summary>
    /// Assemble les informations renvoyées au client : courriel, rôle et agriculteur rattaché.
    /// </summary>
    /// <param name="utilisateur">Compte de l'utilisateur.</param>
    /// <param name="gestionnaireUtilisateurs">Gestionnaire des comptes Identity (lecture des rôles).</param>
    /// <param name="contexte">Contexte de la base de données (lecture du nom de l'agriculteur).</param>
    /// <returns>Les informations de l'utilisateur connecté.</returns>
    private static async Task<UtilisateurConnecteDto> ConstruireUtilisateurConnecteAsync(
        Utilisateur utilisateur,
        UserManager<Utilisateur> gestionnaireUtilisateurs,
        ContexteAquaCuve contexte)
    {
        IList<string> roles = await gestionnaireUtilisateurs.GetRolesAsync(utilisateur);
        string? nomAgriculteur = await contexte.Agriculteurs
            .AsNoTracking()
            .Where(agriculteur => agriculteur.Id == utilisateur.AgriculteurId)
            .Select(agriculteur => agriculteur.Nom)
            .FirstOrDefaultAsync();

        return new UtilisateurConnecteDto(
            utilisateur.Email ?? string.Empty,
            roles.FirstOrDefault() ?? string.Empty,
            utilisateur.AgriculteurId,
            nomAgriculteur);
    }
}
