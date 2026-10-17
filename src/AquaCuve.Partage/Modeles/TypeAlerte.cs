namespace AquaCuve.Partage.Modeles;

/// <summary>
/// Types d'alertes affichées dans l'application (aucun envoi de SMS ni d'e-mail dans le POC).
/// </summary>
public enum TypeAlerte
{
    /// <summary>
    /// Consommation du jour supérieure de X % à celle de la veille.
    /// </summary>
    Fuite,

    /// <summary>
    /// Niveau sous le seuil du robinet flotteur et débit insuffisant.
    /// </summary>
    DefautDebit,

    /// <summary>
    /// Le remplissage est passé de l'eau de forage à l'eau de ville.
    /// </summary>
    PassageEauDeVille,

    /// <summary>
    /// La sécurité manque d'eau est activée.
    /// </summary>
    ManqueEau,
}
