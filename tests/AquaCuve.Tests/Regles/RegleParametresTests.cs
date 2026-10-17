using AquaCuve.Partage.Modeles;
using AquaCuve.Partage.Regles;

namespace AquaCuve.Tests.Regles;

/// <summary>
/// Tests des règles de cohérence des paramètres d'une installation.
/// </summary>
public class RegleParametresTests
{
    /// <summary>
    /// Vérifie que des paramètres cohérents sont acceptés sans exception.
    /// </summary>
    /// <returns>Rien : le test échoue si une exception est levée.</returns>
    [Fact]
    public void Verifier_ParametresCoherents_NeLevePasDException()
    {
        // Organiser
        ParametresInstallation parametres = new(0, 3500, new SeuilsRemplissage(90, 50, 10));

        // Agir
        Exception? exception = Record.Exception(() => RegleParametres.Verifier(parametres));

        // Vérifier
        Assert.Null(exception);
    }

    /// <summary>
    /// Vérifie que le seuil forage à 100 % est accepté (bornes incluses).
    /// </summary>
    /// <returns>Rien : le test échoue si une exception est levée.</returns>
    [Fact]
    public void VerifierSeuils_ForageA100Pourcent_NeLevePasDException()
    {
        // Organiser
        SeuilsRemplissage seuils = new(SeuilForagePourcent: 100, SeuilVillePourcent: 50, SeuilSecuritePourcent: 0);

        // Agir
        Exception? exception = Record.Exception(() => RegleParametres.VerifierSeuils(seuils));

        // Vérifier
        Assert.Null(exception);
    }

    /// <summary>
    /// Vérifie que des hauteurs incohérentes (maxi inférieure ou égale à mini) sont refusées.
    /// </summary>
    /// <param name="hauteurMiniCm">Hauteur correspondant à 0 %, en centimètres.</param>
    /// <param name="hauteurMaxiCm">Hauteur correspondant à 100 %, en centimètres.</param>
    /// <returns>Rien : le test échoue si aucune exception n'est levée.</returns>
    [Theory]
    [InlineData(100, 100)]
    [InlineData(300, 100)]
    [InlineData(0, double.NaN)]
    public void VerifierHauteursCoherentes_HauteursIncoherentes_LeveUneException(
        double hauteurMiniCm, double hauteurMaxiCm)
    {
        // Agir et Vérifier
        Assert.Throws<ArgumentException>(
            () => RegleParametres.VerifierHauteursCoherentes(hauteurMiniCm, hauteurMaxiCm));
    }

    /// <summary>
    /// Vérifie que des seuils hors de 0 à 100 ou mal ordonnés sont refusés.
    /// </summary>
    /// <param name="seuilForagePourcent">Seuil forage, en % de la hauteur.</param>
    /// <param name="seuilVillePourcent">Seuil ville, en % de la hauteur.</param>
    /// <param name="seuilSecuritePourcent">Seuil sécurité, en % de la hauteur.</param>
    /// <returns>Rien : le test échoue si aucune exception n'est levée.</returns>
    [Theory]
    [InlineData(90, 50, 50)]
    [InlineData(50, 50, 10)]
    [InlineData(10, 50, 90)]
    [InlineData(101, 50, 10)]
    [InlineData(90, 50, -1)]
    public void VerifierSeuils_SeuilsInvalides_LeveUneException(
        double seuilForagePourcent, double seuilVillePourcent, double seuilSecuritePourcent)
    {
        // Organiser
        SeuilsRemplissage seuils = new(seuilForagePourcent, seuilVillePourcent, seuilSecuritePourcent);

        // Agir et Vérifier
        Assert.Throws<ArgumentException>(() => RegleParametres.VerifierSeuils(seuils));
    }

    /// <summary>
    /// Vérifie que la vérification globale refuse des seuils invalides même avec des hauteurs correctes.
    /// </summary>
    /// <returns>Rien : le test échoue si aucune exception n'est levée.</returns>
    [Fact]
    public void Verifier_SeuilsInvalides_LeveUneException()
    {
        // Organiser
        ParametresInstallation parametres = new(0, 3500, new SeuilsRemplissage(10, 50, 90));

        // Agir et Vérifier
        Assert.Throws<ArgumentException>(() => RegleParametres.Verifier(parametres));
    }
}
