namespace AquaCuve.Partage.Contrats;

/// <summary>
/// Identifiants envoyés par le client pour ouvrir une session.
/// </summary>
/// <param name="Courriel">Adresse e-mail du compte, utilisée comme identifiant de connexion.</param>
/// <param name="MotDePasse">Mot de passe du compte.</param>
public sealed record DemandeConnexionDto(string Courriel, string MotDePasse);
