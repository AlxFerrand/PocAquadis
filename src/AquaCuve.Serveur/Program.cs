// Point d'entrée du serveur AQUA'Cuve.
// Étape 1 : squelette minimal. L'API, SignalR, Identity et EF Core arrivent aux étapes suivantes.

var constructeur = WebApplication.CreateBuilder(args);
var application = constructeur.Build();

application.MapGet("/", () => "AQUA'Cuve — serveur du POC démarré.");

application.Run();
