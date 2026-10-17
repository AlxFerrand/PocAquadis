namespace AquaCuve.Serveur.Donnees.Entites;

/// <summary>
/// Client d'Aquadis, propriétaire d'une ou plusieurs cuves.
/// </summary>
public sealed class Agriculteur
{
    /// <summary>Identifiant de l'agriculteur.</summary>
    public int Id { get; set; }

    /// <summary>Nom de l'exploitation ou de l'agriculteur.</summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>Installations (cuves) de l'agriculteur.</summary>
    public List<Installation> Installations { get; set; } = [];
}
