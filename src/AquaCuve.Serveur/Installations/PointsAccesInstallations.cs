using System.Security.Claims;
using AquaCuve.Partage.Contrats;
using AquaCuve.Partage.Modeles;
using AquaCuve.Partage.Regles;
using AquaCuve.Serveur.Donnees;
using AquaCuve.Serveur.Donnees.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AquaCuve.Serveur.Installations;

/// <summary>
/// Points d'accès de l'API pour lister les installations accessibles à l'utilisateur connecté.
/// </summary>
public static class PointsAccesInstallations
{
    /// <summary>
    /// Déclare la route GET /api/installations (réservée aux utilisateurs connectés).
    /// </summary>
    /// <param name="routes">Table de routage de l'application.</param>
    /// <returns>La même table de routage, pour enchaîner les appels.</returns>
    public static IEndpointRouteBuilder MapPointsAccesInstallations(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder groupe = routes.MapGroup("/api/installations").RequireAuthorization();

        groupe.MapGet("/", ListerAsync);

        return routes;
    }

    /// <summary>
    /// Liste les installations que l'utilisateur connecté a le droit de consulter (toutes pour un Admin, les siennes pour un agriculteur).
    /// </summary>
    /// <param name="principal">Identité de l'utilisateur lue dans le cookie de session.</param>
    /// <param name="gestionnaireUtilisateurs">Gestionnaire des comptes Identity.</param>
    /// <param name="contexte">Contexte de la base de données.</param>
    /// <param name="jeton">Jeton d'annulation de la requête.</param>
    /// <returns>200 avec la liste des installations accessibles, 401 si le compte n'existe plus.</returns>
    private static async Task<IResult> ListerAsync(
        ClaimsPrincipal principal,
        UserManager<Utilisateur> gestionnaireUtilisateurs,
        ContexteAquaCuve contexte,
        CancellationToken jeton)
    {
        Utilisateur? utilisateur = await gestionnaireUtilisateurs.GetUserAsync(principal);
        if (utilisateur is null)
        {
            return Results.Unauthorized();
        }

        bool estAdmin = principal.IsInRole(RolesUtilisateur.Admin);
        List<InstallationResumeDto> installations = await ChargerInstallationsAsync(contexte, jeton);

        return Results.Ok(FiltrerSelonAcces(installations, estAdmin, utilisateur.AgriculteurId));
    }

    /// <summary>
    /// Charge toutes les installations avec le nom de leur agriculteur, triées par nom.
    /// </summary>
    /// <param name="contexte">Contexte de la base de données.</param>
    /// <param name="jeton">Jeton d'annulation de la requête.</param>
    /// <returns>Les résumés de toutes les installations.</returns>
    private static async Task<List<InstallationResumeDto>> ChargerInstallationsAsync(
        ContexteAquaCuve contexte, CancellationToken jeton)
    {
        return await contexte.Installations
            .AsNoTracking()
            .OrderBy(installation => installation.Nom)
            .Select(installation => new InstallationResumeDto(
                installation.Id,
                installation.Nom,
                installation.AgriculteurId,
                installation.Agriculteur.Nom))
            .ToListAsync(jeton);
    }

    /// <summary>
    /// Ne garde que les installations accessibles, en réutilisant la règle d'accès unique de AquaCuve.Partage.
    /// Le filtre se fait en mémoire : le volume du POC est très faible et la règle reste écrite à un seul endroit.
    /// </summary>
    /// <param name="installations">Installations à filtrer.</param>
    /// <param name="estAdmin">Vrai si l'utilisateur a le rôle Admin.</param>
    /// <param name="agriculteurIdUtilisateur">Agriculteur rattaché à l'utilisateur, ou null.</param>
    /// <returns>Les installations que l'utilisateur peut consulter.</returns>
    private static List<InstallationResumeDto> FiltrerSelonAcces(
        List<InstallationResumeDto> installations, bool estAdmin, int? agriculteurIdUtilisateur)
    {
        return installations
            .Where(installation => RegleAcces.PeutAccederInstallation(
                estAdmin, agriculteurIdUtilisateur, installation.AgriculteurId))
            .ToList();
    }
}
