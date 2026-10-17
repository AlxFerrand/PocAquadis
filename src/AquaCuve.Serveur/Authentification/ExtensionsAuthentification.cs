using AquaCuve.Serveur.Donnees;
using AquaCuve.Serveur.Donnees.Entites;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace AquaCuve.Serveur.Authentification;

/// <summary>
/// Configuration de l'authentification : ASP.NET Core Identity avec cookie, rôles Admin et Agriculteur.
/// </summary>
public static class ExtensionsAuthentification
{
    private const string NomCookie = "AquaCuve.Session";
    private const int DureeSessionJours = 14;

    /// <summary>
    /// Enregistre Identity (comptes et rôles stockés dans la base), le cookie de session et l'autorisation.
    /// </summary>
    /// <param name="constructeur">Constructeur de l'application web.</param>
    /// <returns>Le même constructeur, pour enchaîner les appels.</returns>
    public static WebApplicationBuilder AjouterAuthentification(this WebApplicationBuilder constructeur)
    {
        constructeur.Services
            .AddIdentityCore<Utilisateur>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ContexteAquaCuve>()
            .AddSignInManager();

        constructeur.Services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        constructeur.Services.AddAuthorization();
        constructeur.Services.ConfigureApplicationCookie(ConfigurerCookie);

        return constructeur;
    }

    /// <summary>
    /// Règle le cookie de session : inaccessible au JavaScript, durée limitée, et codes 401 / 403 plutôt que des redirections (API).
    /// </summary>
    /// <param name="options">Options du cookie d'authentification à régler.</param>
    /// <returns>Rien : les options sont modifiées directement.</returns>
    private static void ConfigurerCookie(CookieAuthenticationOptions options)
    {
        options.Cookie.Name = NomCookie;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(DureeSessionJours);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = contexte => RepondreCode(contexte.Response, StatusCodes.Status401Unauthorized);
        options.Events.OnRedirectToAccessDenied = contexte => RepondreCode(contexte.Response, StatusCodes.Status403Forbidden);
    }

    /// <summary>
    /// Renvoie directement un code d'état HTTP au lieu de rediriger vers une page de connexion.
    /// </summary>
    /// <param name="reponse">Réponse HTTP à renseigner.</param>
    /// <param name="codeEtat">Code d'état HTTP à renvoyer (401 ou 403).</param>
    /// <returns>Une tâche déjà terminée.</returns>
    private static Task RepondreCode(HttpResponse reponse, int codeEtat)
    {
        reponse.StatusCode = codeEtat;
        return Task.CompletedTask;
    }
}
