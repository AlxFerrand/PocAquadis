using AquaCuve.Serveur.Donnees.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AquaCuve.Serveur.Donnees;

/// <summary>
/// Contexte EF Core de l'application : tables métier et tables ASP.NET Core Identity, dans la même base SQLite.
/// </summary>
public sealed class ContexteAquaCuve : IdentityDbContext<Utilisateur>
{
    private const int LongueurNomMax = 100;
    private const int LongueurIdentifiantMax = 50;

    /// <summary>
    /// Crée le contexte avec les options (fournisseur SQLite, chaîne de connexion) fournies par l'injection de dépendances.
    /// </summary>
    /// <param name="options">Options de configuration du contexte.</param>
    public ContexteAquaCuve(DbContextOptions<ContexteAquaCuve> options)
        : base(options)
    {
    }

    /// <summary>Agriculteurs, propriétaires des cuves.</summary>
    public DbSet<Agriculteur> Agriculteurs => Set<Agriculteur>();

    /// <summary>Installations (cuves) et leurs paramètres.</summary>
    public DbSet<Installation> Installations => Set<Installation>();

    /// <summary>Coffrets de suivi.</summary>
    public DbSet<Coffret> Coffrets => Set<Coffret>();

    /// <summary>Mesures remontées par les coffrets.</summary>
    public DbSet<Mesure> Mesures => Set<Mesure>();

    /// <summary>Alertes levées sur les installations.</summary>
    public DbSet<Alerte> Alertes => Set<Alerte>();

    /// <summary>
    /// Applique la conversion UTC à toutes les dates du modèle (dates stockées et relues en UTC).
    /// </summary>
    /// <param name="configurationBuilder">Constructeur des conventions du modèle.</param>
    /// <returns>Rien : la configuration est appliquée au constructeur.</returns>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<ConvertisseurDateUtc>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<ConvertisseurDateUtc>();
    }

    /// <summary>
    /// Décrit le modèle : tables Identity, relations, index et longueurs maximales.
    /// </summary>
    /// <param name="constructeur">Constructeur du modèle.</param>
    /// <returns>Rien : la configuration est appliquée au constructeur.</returns>
    protected override void OnModelCreating(ModelBuilder constructeur)
    {
        base.OnModelCreating(constructeur);

        ConfigurerUtilisateur(constructeur);
        ConfigurerAgriculteur(constructeur);
        ConfigurerInstallation(constructeur);
        ConfigurerCoffret(constructeur);
        ConfigurerMesure(constructeur);
        ConfigurerAlerte(constructeur);
    }

    /// <summary>
    /// Configure le rattachement optionnel d'un compte à un agriculteur (vide pour un Admin).
    /// </summary>
    /// <param name="constructeur">Constructeur du modèle.</param>
    /// <returns>Rien : la configuration est appliquée au constructeur.</returns>
    private static void ConfigurerUtilisateur(ModelBuilder constructeur)
    {
        constructeur.Entity<Utilisateur>()
            .HasOne(utilisateur => utilisateur.Agriculteur)
            .WithMany()
            .HasForeignKey(utilisateur => utilisateur.AgriculteurId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    /// <summary>
    /// Configure la longueur maximale du nom d'un agriculteur.
    /// </summary>
    /// <param name="constructeur">Constructeur du modèle.</param>
    /// <returns>Rien : la configuration est appliquée au constructeur.</returns>
    private static void ConfigurerAgriculteur(ModelBuilder constructeur)
    {
        constructeur.Entity<Agriculteur>()
            .Property(agriculteur => agriculteur.Nom)
            .HasMaxLength(LongueurNomMax);
    }

    /// <summary>
    /// Configure une installation : appartenance à un agriculteur et longueur du nom.
    /// </summary>
    /// <param name="constructeur">Constructeur du modèle.</param>
    /// <returns>Rien : la configuration est appliquée au constructeur.</returns>
    private static void ConfigurerInstallation(ModelBuilder constructeur)
    {
        constructeur.Entity<Installation>(entite =>
        {
            entite.Property(installation => installation.Nom).HasMaxLength(LongueurNomMax);
            entite.HasOne(installation => installation.Agriculteur)
                .WithMany(agriculteur => agriculteur.Installations)
                .HasForeignKey(installation => installation.AgriculteurId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Configure un coffret : identifiant unique et relation un pour un avec son installation.
    /// </summary>
    /// <param name="constructeur">Constructeur du modèle.</param>
    /// <returns>Rien : la configuration est appliquée au constructeur.</returns>
    private static void ConfigurerCoffret(ModelBuilder constructeur)
    {
        constructeur.Entity<Coffret>(entite =>
        {
            entite.Property(coffret => coffret.Identifiant).HasMaxLength(LongueurIdentifiantMax);
            entite.HasIndex(coffret => coffret.Identifiant).IsUnique();
            entite.HasOne(coffret => coffret.Installation)
                .WithOne(installation => installation.Coffret)
                .HasForeignKey<Coffret>(coffret => coffret.InstallationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Configure une mesure : appartenance à un coffret et index (coffret, date) pour lire l'historique rapidement.
    /// </summary>
    /// <param name="constructeur">Constructeur du modèle.</param>
    /// <returns>Rien : la configuration est appliquée au constructeur.</returns>
    private static void ConfigurerMesure(ModelBuilder constructeur)
    {
        constructeur.Entity<Mesure>(entite =>
        {
            entite.HasIndex(mesure => new { mesure.CoffretId, mesure.HorodatageUtc });
            entite.HasOne(mesure => mesure.Coffret)
                .WithMany()
                .HasForeignKey(mesure => mesure.CoffretId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Configure une alerte : appartenance à une installation, type stocké en texte et index (installation, date).
    /// </summary>
    /// <param name="constructeur">Constructeur du modèle.</param>
    /// <returns>Rien : la configuration est appliquée au constructeur.</returns>
    private static void ConfigurerAlerte(ModelBuilder constructeur)
    {
        constructeur.Entity<Alerte>(entite =>
        {
            entite.Property(alerte => alerte.Type).HasConversion<string>();
            entite.HasIndex(alerte => new { alerte.InstallationId, alerte.HorodatageUtc });
            entite.HasOne(alerte => alerte.Installation)
                .WithMany()
                .HasForeignKey(alerte => alerte.InstallationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
