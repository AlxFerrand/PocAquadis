namespace AquaCuve.Partage.Regles;

/// <summary>
/// Règles de calcul liées au niveau d'eau d'une cuve.
/// </summary>
public static class RegleNiveauCuve
{
    /// <summary>
    /// Pourcentage correspondant à une cuve vide.
    /// </summary>
    public const double PourcentageMinimum = 0;

    /// <summary>
    /// Pourcentage correspondant à une cuve pleine.
    /// </summary>
    public const double PourcentageMaximum = 100;

    private const double FacteurPourcentage = 100;

    /// <summary>
    /// Calcule le pourcentage de remplissage d'une cuve à partir de la hauteur d'eau mesurée.
    /// </summary>
    /// <param name="hauteurMesureeCm">Hauteur d'eau mesurée par le capteur, en centimètres.</param>
    /// <param name="hauteurMiniCm">Hauteur correspondant à 0 % (fond de cuve), en centimètres.</param>
    /// <param name="hauteurMaxiCm">Hauteur correspondant à 100 % (cuve pleine), en centimètres.</param>
    /// <returns>Le pourcentage de remplissage, borné entre 0 et 100.</returns>
    /// <exception cref="ArgumentException">Si la hauteur maxi n'est pas supérieure à la hauteur mini.</exception>
    public static double CalculerPourcentageRemplissage(
        double hauteurMesureeCm, double hauteurMiniCm, double hauteurMaxiCm)
    {
        RegleParametres.VerifierHauteursCoherentes(hauteurMiniCm, hauteurMaxiCm);

        double plageUtileCm = hauteurMaxiCm - hauteurMiniCm;
        double pourcentageBrut = (hauteurMesureeCm - hauteurMiniCm) / plageUtileCm * FacteurPourcentage;

        return BornerPourcentage(pourcentageBrut);
    }

    /// <summary>
    /// Ramène un pourcentage dans l'intervalle autorisé de 0 à 100.
    /// </summary>
    /// <param name="pourcentage">Pourcentage à borner, pouvant dépasser les limites en cas de mesure aberrante.</param>
    /// <returns>Le pourcentage, ramené à 0 s'il est inférieur à 0, ou à 100 s'il dépasse 100.</returns>
    private static double BornerPourcentage(double pourcentage)
    {
        return Math.Clamp(pourcentage, PourcentageMinimum, PourcentageMaximum);
    }
}
