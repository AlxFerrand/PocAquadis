# AQUA'Cuve — POC

Preuve de concept d'une application web mobile (PWA) de suivi de cuves d'eau pour agriculteurs, à partir du cahier des charges du client Aquadis. Les mesures du coffret sont **simulées** : le matériel et le broker MQTT sont hors périmètre du POC.

Le POC démontre l'historisation des mesures (24 h et 30 jours), le paramétrage de l'installation, l'affichage en temps réel, deux profils (agriculteur et Admin) et une PWA installable sur mobile.

## Prérequis

- Visual Studio récent, compatible .NET 10, avec la charge de travail « ASP.NET et développement web »
- SDK .NET 10 (version LTS)
- Pour tester la PWA sur téléphone avant l'hébergement : un tunnel HTTPS (Dev Tunnels ou Cloudflare Tunnel)

## Structure

| Dossier | Rôle |
|---|---|
| `src/AquaCuve.Partage` | Contrats d'API (DTO) et règles métier pures |
| `src/AquaCuve.Serveur` | API, SignalR, Identity, EF Core, simulateur ; sert aussi le client |
| `src/AquaCuve.Client` | Client Blazor WebAssembly (PWA) |
| `tests/AquaCuve.Tests` | Tests unitaires des règles métier (xUnit) |

## Lancement en développement

1. Ouvrir `AquaCuve.sln` dans Visual Studio.
2. Définir `AquaCuve.Serveur` comme projet de démarrage, puis lancer avec F5.
3. Si le navigateur signale le certificat HTTPS : `dotnet dev-certs https --trust`.

En ligne de commande :

```
dotnet build AquaCuve.sln
dotnet test AquaCuve.sln
dotnet run --project src/AquaCuve.Serveur
```

## Comptes de démonstration

À définir à l'étape 3 (authentification). Uniquement des mots de passe de démonstration, jamais ceux d'un vrai environnement.

## Déploiement de démonstration

À définir à l'étape 10 (Dockerfile et hébergeur gratuit).

## Conventions de code

Tout est en français, sans accent dans le code (accents autorisés dans les commentaires et les textes affichés). Chaque méthode, y compris privée et de test, porte un commentaire `///` avec `summary`, `param` et `returns`. Le style est imposé par `.editorconfig`.

## Conventions Git

Branche `main` stable, une branche par étape fusionnée après relecture. Commits courts, en français, commençant par un verbe.
