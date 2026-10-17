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

## Base de données

SQLite, dans le fichier `aquacuve.db` du dossier d'exécution du serveur (ignoré par Git). La base est **supprimée puis recréée à chaque démarrage** avec les données de démonstration : un agriculteur (Ferme Le Goff) avec trois cuves aux profils différents et leurs coffrets simulés, plus deux comptes (Admin et agriculteur). L'historique de mesures est généré à l'étape 4.

## Comptes de démonstration

Deux comptes sont créés au démarrage : un **Admin** (voit toutes les cuves) et un **agriculteur** (ne voit que ses cuves). Leurs courriels sont dans `src/AquaCuve.Serveur/appsettings.json` (section `ComptesDemo`).

**Les mots de passe ne sont écrits nulle part dans le dépôt** : vous les choisissez et les gardez en local. Ils doivent respecter les règles par défaut d'ASP.NET Core Identity (6 caractères minimum, avec majuscule, minuscule, chiffre et caractère spécial).

En développement, avec les secrets utilisateur (stockés hors du dépôt) :

```
cd src/AquaCuve.Serveur
dotnet user-secrets set "ComptesDemo:Admin:MotDePasse" "<mot de passe de votre choix>"
dotnet user-secrets set "ComptesDemo:Agriculteur:MotDePasse" "<mot de passe de votre choix>"
```

Dans Visual Studio : clic droit sur le projet `AquaCuve.Serveur`, puis « Gérer les secrets utilisateur ».

Chez l'hébergeur de démonstration : variables d'environnement `ComptesDemo__Admin__MotDePasse` et `ComptesDemo__Agriculteur__MotDePasse`.

Sans ces valeurs, le serveur refuse de démarrer et indique quelle clé de configuration manque.

## Tester l'API sans interface

Le client arrive à l'étape 5. En attendant, avec curl (le cookie de session est conservé dans `cookies.txt`) :

```
curl -k -c cookies.txt -H "Content-Type: application/json" -d "{\"courriel\":\"agriculteur@aquacuve.demo\",\"motDePasse\":\"<votre mot de passe>\"}" https://localhost:53196/api/auth/connexion
curl -k -b cookies.txt https://localhost:53196/api/auth/moi
curl -k -b cookies.txt https://localhost:53196/api/installations
```

Un fichier `src/AquaCuve.Serveur/AquaCuve.Serveur.http` permet aussi de tester depuis Visual Studio (voir les commentaires en tête du fichier).

## Déploiement de démonstration

À définir à l'étape 10 (Dockerfile et hébergeur gratuit).

## Conventions de code

Tout est en français, sans accent dans le code (accents autorisés dans les commentaires et les textes affichés). Chaque méthode, y compris privée et de test, porte un commentaire `///` avec `summary`, `param` et `returns`. Le style est imposé par `.editorconfig`.

## Conventions Git

Branche `main` stable, une branche par étape fusionnée après relecture. Commits courts, en français, commençant par un verbe.
