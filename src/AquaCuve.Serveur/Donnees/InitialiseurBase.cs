using AquaCuve.Partage.Modeles;
using AquaCuve.Serveur.Configuration;
using AquaCuve.Serveur.Donnees.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AquaCuve.Serveur.Donnees;

/// <summary>
/// Recrée la base de démonstration au démarrage : rôles, agriculteur, cuves, coffrets et comptes de connexion.
/// La base est supprimée puis recréée à chaque démarrage (disque éphémère chez l'hébergeur de démonstration).
/// </summary>
public sealed class InitialiseurBase
{
    private readonly ContexteAquaCuve _contexte;
    private readonly UserManager<Utilisateur> _gestionnaireUtilisateurs;
    private readonly RoleManager<IdentityRole> _gestionnaireRoles;
    private readonly OptionsComptesDemo _comptes;
    private readonly TimeProvider _horloge;

    /// <summary>
    /// Crée l'initialiseur avec ses dépendances, fournies par l'injection de dépendances.
    /// </summary>
    /// <param name="contexte">Contexte de la base de données.</param>
    /// <param name="gestionnaireUtilisateurs">Gestionnaire des comptes Identity.</param>
    /// <param name="gestionnaireRoles">Gestionnaire des rôles Identity.</param>
    /// <param name="options">Comptes de démonstration lus dans la configuration.</param>
    /// <param name="horloge">Horloge, injectée pour pouvoir être remplacée dans les tests.</param>
    public InitialiseurBase(
        ContexteAquaCuve contexte,
        UserManager<Utilisateur> gestionnaireUtilisateurs,
        RoleManager<IdentityRole> gestionnaireRoles,
        IOptions<OptionsComptesDemo> options,
        TimeProvider horloge)
    {
        _contexte = contexte;
        _gestionnaireUtilisateurs = gestionnaireUtilisateurs;
        _gestionnaireRoles = gestionnaireRoles;
        _comptes = options.Value;
        _horloge = horloge;
    }

    /// <summary>
    /// Recrée la base et y insère les données de démonstration.
    /// </summary>
    /// <param name="jeton">Jeton d'annulation de l'opération.</param>
    /// <returns>Une tâche terminée quand la base est prête.</returns>
    /// <exception cref="InvalidOperationException">Si un compte de démonstration est incomplet dans la configuration ou si Identity refuse sa création.</exception>
    public async Task InitialiserAsync(CancellationToken jeton)
    {
        VerifierCompteComplet(_comptes.Admin, nameof(OptionsComptesDemo.Admin));
        VerifierCompteComplet(_comptes.Agriculteur, nameof(OptionsComptesDemo.Agriculteur));

        await RecreerBaseAsync(jeton);
        await CreerRolesAsync();
        Agriculteur agriculteur = await CreerAgriculteurAsync(jeton);
        await CreerCompteAsync(_comptes.Admin, RolesUtilisateur.Admin, agriculteurId: null);
        await CreerCompteAsync(_comptes.Agriculteur, RolesUtilisateur.Agriculteur, agriculteur.Id);
    }

    /// <summary>
    /// Vérifie qu'un compte de démonstration a son courriel et son mot de passe, et explique où les renseigner sinon.
    /// </summary>
    /// <param name="compte">Compte de démonstration à contrôler.</param>
    /// <param name="nomCompte">Nom du compte dans la configuration (Admin ou Agriculteur), repris dans le message d'erreur.</param>
    /// <returns>Rien : la méthode lève une exception si le compte est incomplet.</returns>
    /// <exception cref="InvalidOperationException">Si le courriel ou le mot de passe est absent de la configuration.</exception>
    private static void VerifierCompteComplet(CompteDemo compte, string nomCompte)
    {
        if (compte.EstComplet())
        {
            return;
        }

        string section = $"{OptionsComptesDemo.NomSection}:{nomCompte}";
        throw new InvalidOperationException(
            $"Le compte de démonstration « {nomCompte} » est incomplet : renseignez « {section}:Courriel » et « {section}:MotDePasse » " +
            $"dans la configuration (secrets utilisateur en développement, variable d'environnement « {section.Replace(":", "__")}__MotDePasse » " +
            "chez l'hébergeur). Les mots de passe ne sont jamais enregistrés dans le dépôt.");
    }

    /// <summary>
    /// Supprime la base existante puis la recrée vide avec toutes ses tables.
    /// </summary>
    /// <param name="jeton">Jeton d'annulation de l'opération.</param>
    /// <returns>Une tâche terminée quand la base vide est créée.</returns>
    private async Task RecreerBaseAsync(CancellationToken jeton)
    {
        await _contexte.Database.EnsureDeletedAsync(jeton);
        await _contexte.Database.EnsureCreatedAsync(jeton);
    }

    /// <summary>
    /// Crée les rôles Admin et Agriculteur.
    /// </summary>
    /// <returns>Une tâche terminée quand les rôles sont créés.</returns>
    /// <exception cref="InvalidOperationException">Si Identity refuse la création d'un rôle.</exception>
    private async Task CreerRolesAsync()
    {
        string[] noms = [RolesUtilisateur.Admin, RolesUtilisateur.Agriculteur];

        foreach (string nom in noms)
        {
            IdentityResult resultat = await _gestionnaireRoles.CreateAsync(new IdentityRole(nom));
            VerifierResultat(resultat, $"création du rôle {nom}");
        }
    }

    /// <summary>
    /// Enregistre en base l'agriculteur de démonstration avec ses cuves et ses coffrets.
    /// </summary>
    /// <param name="jeton">Jeton d'annulation de l'opération.</param>
    /// <returns>L'agriculteur enregistré, avec son identifiant renseigné.</returns>
    private async Task<Agriculteur> CreerAgriculteurAsync(CancellationToken jeton)
    {
        DateTime maintenantUtc = _horloge.GetUtcNow().UtcDateTime;
        Agriculteur agriculteur = DonneesDemonstration.CreerAgriculteur(maintenantUtc);

        _contexte.Agriculteurs.Add(agriculteur);
        await _contexte.SaveChangesAsync(jeton);

        return agriculteur;
    }

    /// <summary>
    /// Crée un compte de connexion et lui attribue son rôle.
    /// </summary>
    /// <param name="compte">Courriel et mot de passe du compte, lus dans la configuration.</param>
    /// <param name="role">Rôle à attribuer (voir <see cref="RolesUtilisateur"/>).</param>
    /// <param name="agriculteurId">Identifiant de l'agriculteur rattaché, ou null pour un Admin.</param>
    /// <returns>Une tâche terminée quand le compte est créé.</returns>
    /// <exception cref="InvalidOperationException">Si Identity refuse la création du compte (mot de passe trop faible, par exemple).</exception>
    private async Task CreerCompteAsync(CompteDemo compte, string role, int? agriculteurId)
    {
        Utilisateur utilisateur = new()
        {
            UserName = compte.Courriel,
            Email = compte.Courriel,
            EmailConfirmed = true,
            AgriculteurId = agriculteurId,
        };

        IdentityResult creation = await _gestionnaireUtilisateurs.CreateAsync(utilisateur, compte.MotDePasse);
        VerifierResultat(creation, $"création du compte {compte.Courriel}");

        IdentityResult attribution = await _gestionnaireUtilisateurs.AddToRoleAsync(utilisateur, role);
        VerifierResultat(attribution, $"attribution du rôle {role} à {compte.Courriel}");
    }

    /// <summary>
    /// Lève une exception lisible si une opération Identity a échoué.
    /// </summary>
    /// <param name="resultat">Résultat renvoyé par Identity.</param>
    /// <param name="operation">Description de l'opération, reprise dans le message d'erreur.</param>
    /// <returns>Rien : la méthode lève une exception si l'opération a échoué.</returns>
    /// <exception cref="InvalidOperationException">Si le résultat n'est pas un succès.</exception>
    private static void VerifierResultat(IdentityResult resultat, string operation)
    {
        if (resultat.Succeeded)
        {
            return;
        }

        string erreurs = string.Join(" ; ", resultat.Errors.Select(erreur => erreur.Description));
        throw new InvalidOperationException($"Échec de l'opération « {operation} » : {erreurs}");
    }
}
