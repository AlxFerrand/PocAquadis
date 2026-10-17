using AquaCuve.Partage.Regles;

namespace AquaCuve.Tests.Regles;

/// <summary>
/// Tests de la règle d'alerte de fuite (surconsommation par rapport à la veille).
/// </summary>
public class RegleAlerteFuiteTests
{
    /// <summary>
    /// Vérifie la détection de surconsommation avec une veille à 100 m³ et un seuil de 50 %.
    /// </summary>
    /// <param name="consoJourM3">Consommation du jour, en mètres cubes.</param>
    /// <param name="alerteAttendue">Vrai si une alerte doit être levée.</param>
    /// <returns>Rien : le test échoue si le résultat diffère de l'attendu.</returns>
    [Theory]
    [InlineData(80, false)]
    [InlineData(100, false)]
    [InlineData(150, false)]
    [InlineData(151, true)]
    [InlineData(400, true)]
    public void DetecterSurconsommation_VeilleA100EtSeuilA50_RetourneLAlerteAttendue(
        double consoJourM3, bool alerteAttendue)
    {
        // Organiser
        const double consoVeilleM3 = 100;
        const double seuilSurconsommationPourcent = 50;

        // Agir
        bool resultat = RegleAlerteFuite.DetecterSurconsommation(consoJourM3, consoVeilleM3, seuilSurconsommationPourcent);

        // Vérifier
        Assert.Equal(alerteAttendue, resultat);
    }

    /// <summary>
    /// Vérifie qu'aucune alerte n'est levée quand la veille n'a pas de consommation (pas de référence).
    /// </summary>
    /// <returns>Rien : le test échoue si une alerte est levée.</returns>
    [Fact]
    public void DetecterSurconsommation_VeilleSansConsommation_NeLevePasDAlerte()
    {
        // Organiser
        const double consoJourM3 = 5;

        // Agir
        bool resultat = RegleAlerteFuite.DetecterSurconsommation(consoJourM3, 0, 50);

        // Vérifier
        Assert.False(resultat);
    }

    /// <summary>
    /// Vérifie qu'une consommation ou un seuil négatif est refusé.
    /// </summary>
    /// <param name="consoJourM3">Consommation du jour, en mètres cubes.</param>
    /// <param name="consoVeilleM3">Consommation de la veille, en mètres cubes.</param>
    /// <param name="seuilPourcent">Seuil de surconsommation, en pourcentage.</param>
    /// <returns>Rien : le test échoue si aucune exception n'est levée.</returns>
    [Theory]
    [InlineData(-1, 10, 50)]
    [InlineData(10, -1, 50)]
    [InlineData(10, 10, -1)]
    [InlineData(double.NaN, 10, 50)]
    public void DetecterSurconsommation_ValeurInvalide_LeveUneException(
        double consoJourM3, double consoVeilleM3, double seuilPourcent)
    {
        // Agir et Vérifier
        Assert.Throws<ArgumentException>(
            () => RegleAlerteFuite.DetecterSurconsommation(consoJourM3, consoVeilleM3, seuilPourcent));
    }
}
