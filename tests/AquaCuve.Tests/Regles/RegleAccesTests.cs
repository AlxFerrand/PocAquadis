using AquaCuve.Partage.Regles;

namespace AquaCuve.Tests.Regles;

/// <summary>
/// Tests de la règle d'accès aux installations (Admin ou agriculteur propriétaire).
/// </summary>
public class RegleAccesTests
{
    /// <summary>
    /// Vérifie l'accès selon le profil de l'utilisateur et le propriétaire de l'installation.
    /// </summary>
    /// <param name="estAdmin">Vrai si l'utilisateur a le rôle Admin.</param>
    /// <param name="agriculteurIdUtilisateur">Agriculteur rattaché à l'utilisateur, ou null.</param>
    /// <param name="agriculteurIdInstallation">Agriculteur propriétaire de l'installation.</param>
    /// <param name="accesAttendu">Vrai si l'accès doit être autorisé.</param>
    /// <returns>Rien : le test échoue si l'autorisation diffère de l'attendu.</returns>
    [Theory]
    // Admin : accès à tout, qu'il soit rattaché à un agriculteur ou non
    [InlineData(true, null, 1, true)]
    [InlineData(true, 2, 1, true)]
    // Agriculteur : uniquement ses installations
    [InlineData(false, 1, 1, true)]
    [InlineData(false, 2, 1, false)]
    // Compte sans agriculteur rattaché et sans rôle Admin : aucun accès
    [InlineData(false, null, 1, false)]
    public void PeutAccederInstallation_ProfilEtProprietaire_RetourneLAccesAttendu(
        bool estAdmin, int? agriculteurIdUtilisateur, int agriculteurIdInstallation, bool accesAttendu)
    {
        // Agir
        bool resultat = RegleAcces.PeutAccederInstallation(estAdmin, agriculteurIdUtilisateur, agriculteurIdInstallation);

        // Vérifier
        Assert.Equal(accesAttendu, resultat);
    }
}
