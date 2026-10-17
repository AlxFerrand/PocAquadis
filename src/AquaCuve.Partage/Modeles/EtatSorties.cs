namespace AquaCuve.Partage.Modeles;

/// <summary>
/// État des sorties relais du coffret liées au remplissage de la cuve.
/// </summary>
/// <param name="ForageActif">Vrai si l'électrovanne de remplissage par l'eau de forage est ouverte.</param>
/// <param name="VilleActive">Vrai si l'électrovanne d'eau de ville est ouverte.</param>
/// <param name="SecuriteManqueEau">Vrai si la sortie sécurité manque d'eau est activée.</param>
public sealed record EtatSorties(bool ForageActif, bool VilleActive, bool SecuriteManqueEau)
{
    /// <summary>
    /// État de repos : aucune sortie n'est activée (point de départ avant la première mesure).
    /// </summary>
    public static EtatSorties Repos => new(false, false, false);
}
