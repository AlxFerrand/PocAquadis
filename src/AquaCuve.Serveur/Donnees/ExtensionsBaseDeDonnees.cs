using AquaCuve.Serveur.Configuration;
using Microsoft.EntityFrameworkCore;

namespace AquaCuve.Serveur.Donnees;

/// <summary>
/// Branchement de la base SQLite et de l'initialisation des données de démonstration dans l'application.
/// </summary>
public static class ExtensionsBaseDeDonnees
{
    private const string NomChaineConnexion = "Base";

    /// <summary>
    /// Enregistre le contexte EF Core (SQLite), l'horloge, les comptes de démonstration et l'initialiseur.
    /// </summary>
    /// <param name="constructeur">Constructeur de l'application web.</param>
    /// <returns>Le même constructeur, pour enchaîner les appels.</returns>
    /// <exception cref="InvalidOperationException">Si la chaîne de connexion « Base » est absente de la configuration.</exception>
    public static WebApplicationBuilder AjouterBaseDeDonnees(this WebApplicationBuilder constructeur)
    {
        string chaineConnexion = constructeur.Configuration.GetConnectionString(NomChaineConnexion)
            ?? throw new InvalidOperationException(
                $"La chaîne de connexion « {NomChaineConnexion} » est absente de la configuration.");

        constructeur.Services.AddDbContext<ContexteAquaCuve>(options => options.UseSqlite(chaineConnexion));
        constructeur.Services.AddSingleton(TimeProvider.System);
        constructeur.Services.Configure<OptionsComptesDemo>(
            constructeur.Configuration.GetSection(OptionsComptesDemo.NomSection));
        constructeur.Services.AddScoped<InitialiseurBase>();

        return constructeur;
    }

    /// <summary>
    /// Recrée la base et insère les données de démonstration. À appeler une fois, avant de démarrer le serveur.
    /// </summary>
    /// <param name="application">Application web construite.</param>
    /// <returns>Une tâche terminée quand la base est prête.</returns>
    public static async Task InitialiserBaseAsync(this WebApplication application)
    {
        using IServiceScope portee = application.Services.CreateScope();
        InitialiseurBase initialiseur = portee.ServiceProvider.GetRequiredService<InitialiseurBase>();

        await initialiseur.InitialiserAsync(application.Lifetime.ApplicationStopping);
    }
}
