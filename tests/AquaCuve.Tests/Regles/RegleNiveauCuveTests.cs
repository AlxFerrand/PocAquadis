using AquaCuve.Partage.Regles;

namespace AquaCuve.Tests.Regles;

/// <summary>
/// Tests de la règle de calcul du pourcentage de remplissage d'une cuve.
/// </summary>
public class RegleNiveauCuveTests
{
    /// <summary>
    /// Vérifie le pourcentage calculé pour une cuve de 0 à 3500 cm, y compris les mesures hors plage (bornées).
    /// </summary>
    /// <param name="hauteurMesureeCm">Hauteur d'eau mesurée, en centimètres.</param>
    /// <param name="pourcentageAttendu">Pourcentage de remplissage attendu, de 0 à 100.</param>
    /// <returns>Rien : le test échoue si le pourcentage calculé diffère de l'attendu.</returns>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(875, 25)]
    [InlineData(1750, 50)]
    [InlineData(3500, 100)]
    [InlineData(-100, 0)]
    [InlineData(4000, 100)]
    public void CalculerPourcentageRemplissage_HauteurMesuree_RetourneLePourcentageBorne(
        double hauteurMesureeCm, double pourcentageAttendu)
    {
        // Organiser
        const double hauteurMiniCm = 0;
        const double hauteurMaxiCm = 3500;

        // Agir
        double resultat = RegleNiveauCuve.CalculerPourcentageRemplissage(hauteurMesureeCm, hauteurMiniCm, hauteurMaxiCm);

        // Vérifier
        Assert.Equal(pourcentageAttendu, resultat, precision: 6);
    }

    /// <summary>
    /// Vérifie qu'une hauteur mini non nulle décale bien la plage utile (100 à 300 cm, mesure à 200 cm : 50 %).
    /// </summary>
    /// <returns>Rien : le test échoue si le pourcentage n'est pas 50.</returns>
    [Fact]
    public void CalculerPourcentageRemplissage_HauteurMiniNonNulle_DecaleLaPlageUtile()
    {
        // Organiser
        const double hauteurMesureeCm = 200;

        // Agir
        double resultat = RegleNiveauCuve.CalculerPourcentageRemplissage(hauteurMesureeCm, 100, 300);

        // Vérifier
        Assert.Equal(50, resultat, precision: 6);
    }

    /// <summary>
    /// Vérifie qu'une hauteur maxi inférieure ou égale à la hauteur mini est refusée.
    /// </summary>
    /// <param name="hauteurMiniCm">Hauteur correspondant à 0 %, en centimètres.</param>
    /// <param name="hauteurMaxiCm">Hauteur correspondant à 100 %, en centimètres.</param>
    /// <returns>Rien : le test échoue si aucune exception n'est levée.</returns>
    [Theory]
    [InlineData(100, 100)]
    [InlineData(300, 100)]
    public void CalculerPourcentageRemplissage_HauteursIncoherentes_LeveUneException(
        double hauteurMiniCm, double hauteurMaxiCm)
    {
        // Organiser
        const double hauteurMesureeCm = 150;

        // Agir et Vérifier
        Assert.Throws<ArgumentException>(
            () => RegleNiveauCuve.CalculerPourcentageRemplissage(hauteurMesureeCm, hauteurMiniCm, hauteurMaxiCm));
    }
}
