namespace AquaCuve.Partage.Regles;

/// <summary>
/// Règle d'alerte de fuite d'eau, détectée par une surconsommation par rapport à la veille.
/// </summary>
public static class RegleAlerteFuite
{
    private const double DiviseurPourcentage = 100;

    /// <summary>
    /// Dit si la consommation du jour dépasse celle de la veille de plus du pourcentage toléré.
    /// </summary>
    /// <remarks>
    /// Règle issue du cahier des charges : si la consommation dépasse de X % celle de la veille, alerte de surconsommation.
    /// La valeur de X n'est pas fixée par le client : elle est donc un paramètre obligatoire.
    /// Sans consommation la veille (0), il n'y a pas de référence : aucune alerte n'est levée.
    /// </remarks>
    /// <param name="consoJourM3">Consommation du jour, en mètres cubes.</param>
    /// <param name="consoVeilleM3">Consommation de la veille, en mètres cubes.</param>
    /// <param name="seuilSurconsommationPourcent">Dépassement toléré par rapport à la veille, en pourcentage (X %).</param>
    /// <returns>Vrai si la consommation du jour est strictement supérieure à la veille majorée du seuil.</returns>
    /// <exception cref="ArgumentException">Si une consommation ou le seuil est négatif ou n'est pas un nombre.</exception>
    public static bool DetecterSurconsommation(
        double consoJourM3, double consoVeilleM3, double seuilSurconsommationPourcent)
    {
        VerificationsEntrees.VerifierPositifOuNul(consoJourM3, nameof(consoJourM3));
        VerificationsEntrees.VerifierPositifOuNul(consoVeilleM3, nameof(consoVeilleM3));
        VerificationsEntrees.VerifierPositifOuNul(seuilSurconsommationPourcent, nameof(seuilSurconsommationPourcent));

        if (consoVeilleM3 == 0)
        {
            return false;
        }

        return consoJourM3 > CalculerConsommationMaximaleTolereeM3(consoVeilleM3, seuilSurconsommationPourcent);
    }

    /// <summary>
    /// Calcule la consommation maximale tolérée : celle de la veille majorée du pourcentage autorisé.
    /// </summary>
    /// <param name="consoVeilleM3">Consommation de la veille, en mètres cubes.</param>
    /// <param name="seuilSurconsommationPourcent">Dépassement toléré, en pourcentage.</param>
    /// <returns>La consommation maximale tolérée sans alerte, en mètres cubes.</returns>
    private static double CalculerConsommationMaximaleTolereeM3(
        double consoVeilleM3, double seuilSurconsommationPourcent)
    {
        return consoVeilleM3 * (1 + seuilSurconsommationPourcent / DiviseurPourcentage);
    }
}
