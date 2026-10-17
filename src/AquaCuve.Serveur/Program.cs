// Point d'entrée du serveur AQUA'Cuve.
// Étape 3 : base SQLite recréée au démarrage avec des données de démonstration, authentification par cookie (Identity).
// Le simulateur, SignalR et le service du client arrivent aux étapes suivantes.

using AquaCuve.Serveur.Authentification;
using AquaCuve.Serveur.Donnees;
using AquaCuve.Serveur.Installations;

var constructeur = WebApplication.CreateBuilder(args);

constructeur.AjouterBaseDeDonnees();
constructeur.AjouterAuthentification();

var application = constructeur.Build();

await application.InitialiserBaseAsync();

application.UseAuthentication();
application.UseAuthorization();

application.MapGet("/", () => "AQUA'Cuve — serveur du POC démarré.");
application.MapPointsAccesAuthentification();
application.MapPointsAccesInstallations();

application.Run();
