namespace AquaCuve.Partage.Regles;

/// <summary>
/// Règle d'alerte de défaut de débit sur le remplissage de la cuve.
/// </summary>
public static class RegleAlerteDebit
{
    /// <summary>
    /// Niveau (en % de la hauteur) sous lequel le débit est surveillé : hauteur du robinet flotteur selon le cahier des charges.
    /// </summary>
    public const double SeuilNiveauParDefautPourcent = 80;

    /// <summary>
    /// Débit minimal attendu (en m³/h) pendant le remplissage selon le cahier des charges.
    /// </summary>
    public const double DebitMiniParDefautM3ParHeure = 1;

    /// <summary>
    /// Dit s'il y a un défaut de débit : niveau sous le seuil ET débit sous le débit minimal.
    /// </summary>
    /// <remarks>
    /// Règle issue du cahier des charges : si niveau &lt; 80 % ET débit &lt; 1 m³/h, alors défaut débit.
    /// Les deux valeurs sont des paramètres facultatifs pour pouvoir les ajuster sans modifier le code.
    /// </remarks>
    /// <param name="pourcentage">Niveau actuel de la cuve, de 0 à 100 (en % de la hauteur).</param>
    /// <param name="debitM3ParHeure">Débit mesuré, en mètres cubes par heure.</param>
    /// <param name="seuilNiveauPourcent">Niveau sous lequel le débit est surveillé, en % de la hauteur (80 par défaut).</param>
    /// <param name="debitMiniM3ParHeure">Débit sous lequel on considère le débit insuffisant, en m³/h (1 par défaut).</param>
    /// <returns>Vrai si le niveau est strictement sous le seuil et le débit strictement sous le débit minimal.</returns>
    /// <exception cref="ArgumentException">Si le pourcentage ou le seuil de niveau est hors de 0 à 100, ou si un débit est négatif.</exception>
    public static bool DetecterDefautDebit(
        double pourcentage,
        double debitM3ParHeure,
        double seuilNiveauPourcent = SeuilNiveauParDefautPourcent,
        double debitMiniM3ParHeure = DebitMiniParDefautM3ParHeure)
    {
        VerificationsEntrees.VerifierPourcentage(pourcentage, nameof(pourcentage));
        VerificationsEntrees.VerifierPourcentage(seuilNiveauPourcent, nameof(seuilNiveauPourcent));
        VerificationsEntrees.VerifierPositifOuNul(debitM3ParHeure, nameof(debitM3ParHeure));
        VerificationsEntrees.VerifierPositifOuNul(debitMiniM3ParHeure, nameof(debitMiniM3ParHeure));

        return pourcentage < seuilNiveauPourcent && debitM3ParHeure < debitMiniM3ParHeure;
    }
}
