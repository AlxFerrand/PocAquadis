namespace AquaCuve.Serveur.Configuration;

/// <summary>
/// Identifiants d'un compte de démonstration lus dans la configuration.
/// Le courriel est dans appsettings.json ; le mot de passe n'est jamais dans le dépôt (secrets utilisateur ou variables d'environnement).
/// </summary>
public sealed class CompteDemo
{
    /// <summary>
    /// Adresse e-mail du compte, utilisée comme identifiant de connexion.
    /// </summary>
    public string Courriel { get; set; } = string.Empty;

    /// <summary>
    /// Mot de passe du compte, fourni uniquement par la configuration locale ou l'hébergeur.
    /// </summary>
    public string MotDePasse { get; set; } = string.Empty;

    /// <summary>
    /// Dit si le compte est utilisable : courriel et mot de passe renseignés.
    /// </summary>
    /// <returns>Vrai si le courriel et le mot de passe ne sont ni vides ni composés d'espaces.</returns>
    public bool EstComplet()
    {
        return !string.IsNullOrWhiteSpace(Courriel) && !string.IsNullOrWhiteSpace(MotDePasse);
    }
}
