namespace AquaCuve.Partage.Contrats;

/// <summary>
/// Informations sur l'utilisateur connecté, renvoyées après la connexion et par la route « moi ».
/// </summary>
/// <param name="Courriel">Adresse e-mail du compte.</param>
/// <param name="Role">Rôle du compte (voir <c>RolesUtilisateur</c>).</param>
/// <param name="AgriculteurId">Identifiant de l'agriculteur rattaché au compte, ou null pour un Admin.</param>
/// <param name="NomAgriculteur">Nom de l'agriculteur rattaché au compte, ou null pour un Admin.</param>
public sealed record UtilisateurConnecteDto(
    string Courriel,
    string Role,
    int? AgriculteurId,
    string? NomAgriculteur);
