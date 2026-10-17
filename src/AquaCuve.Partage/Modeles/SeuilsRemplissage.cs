namespace AquaCuve.Partage.Modeles;

/// <summary>
/// Seuils de pilotage du remplissage d'une cuve, exprimés en pourcentage de la hauteur (de 0 à 100).
/// Ordre attendu : sécurité &lt; ville &lt; forage (voir <c>RegleParametres.VerifierSeuils</c>).
/// </summary>
/// <param name="SeuilForagePourcent">Sous ce niveau, la cuve est remplie par l'eau de forage.</param>
/// <param name="SeuilVillePourcent">Sous ce niveau, le remplissage bascule sur l'eau de ville.</param>
/// <param name="SeuilSecuritePourcent">Sous ce niveau, la sécurité manque d'eau est activée.</param>
public sealed record SeuilsRemplissage(
    double SeuilForagePourcent,
    double SeuilVillePourcent,
    double SeuilSecuritePourcent);
