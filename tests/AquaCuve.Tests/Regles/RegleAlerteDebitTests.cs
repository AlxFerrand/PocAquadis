using AquaCuve.Partage.Regles;

namespace AquaCuve.Tests.Regles;

/// <summary>
/// Tests de la règle d'alerte de défaut de débit (niveau sous 80 % et débit sous 1 m³/h par défaut).
/// </summary>
public class RegleAlerteDebitTests
{
    /// <summary>
    /// Vérifie la détection avec les valeurs par défaut du cahier des charges, y compris aux limites exactes.
    /// </summary>
    /// <param name="pourcentage">Niveau actuel de la cuve, en % de la hauteur.</param>
    /// <param name="debitM3ParHeure">Débit mesuré, en m³/h.</param>
    /// <param name="defautAttendu">Vrai si un défaut de débit doit être signalé.</param>
    /// <returns>Rien : le test échoue si le résultat diffère de l'attendu.</returns>
    [Theory]
    [InlineData(70, 0.5, true)]
    [InlineData(79.9, 0, true)]
    [InlineData(70, 1, false)]
    [InlineData(70, 3, false)]
    [InlineData(80, 0.5, false)]
    [InlineData(95, 0.5, false)]
    public void DetecterDefautDebit_ValeursParDefaut_RetourneLeResultatAttendu(
        double pourcentage, double debitM3ParHeure, bool defautAttendu)
    {
        // Agir
        bool resultat = RegleAlerteDebit.DetecterDefautDebit(pourcentage, debitM3ParHeure);

        // Vérifier
        Assert.Equal(defautAttendu, resultat);
    }

    /// <summary>
    /// Vérifie que les seuils personnalisés remplacent les valeurs par défaut.
    /// </summary>
    /// <returns>Rien : le test échoue si le défaut n'est pas signalé avec les seuils personnalisés.</returns>
    [Fact]
    public void DetecterDefautDebit_SeuilsPersonnalises_UtiliseLesValeursFournies()
    {
        // Organiser
        const double seuilNiveauPourcent = 70;
        const double debitMiniM3ParHeure = 3;

        // Agir
        bool resultat = RegleAlerteDebit.DetecterDefautDebit(60, 2, seuilNiveauPourcent, debitMiniM3ParHeure);

        // Vérifier
        Assert.True(resultat);
    }

    /// <summary>
    /// Vérifie qu'un pourcentage hors plage ou un débit négatif est refusé.
    /// </summary>
    /// <param name="pourcentage">Niveau actuel de la cuve, en % de la hauteur.</param>
    /// <param name="debitM3ParHeure">Débit mesuré, en m³/h.</param>
    /// <returns>Rien : le test échoue si aucune exception n'est levée.</returns>
    [Theory]
    [InlineData(-1, 0.5)]
    [InlineData(101, 0.5)]
    [InlineData(50, -0.1)]
    public void DetecterDefautDebit_ValeurInvalide_LeveUneException(double pourcentage, double debitM3ParHeure)
    {
        // Agir et Vérifier
        Assert.Throws<ArgumentException>(
            () => RegleAlerteDebit.DetecterDefautDebit(pourcentage, debitM3ParHeure));
    }
}
