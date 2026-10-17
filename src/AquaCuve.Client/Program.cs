using AquaCuve.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// Point d'entrée du client Blazor WebAssembly.
// Étape 1 : squelette minimal. Connexion, pages et PWA arrivent aux étapes suivantes.

var constructeur = WebAssemblyHostBuilder.CreateDefault(args);
constructeur.RootComponents.Add<App>("#app");
constructeur.RootComponents.Add<HeadOutlet>("head::after");

await constructeur.Build().RunAsync();
