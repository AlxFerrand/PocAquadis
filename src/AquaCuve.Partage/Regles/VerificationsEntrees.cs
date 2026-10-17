namespace AquaCuve.Partage.Regles;

/// <summary>
/// Vérifications d'entrées communes aux règles métier (usage interne à AquaCuve.Partage).
/// </summary>
internal static class VerificationsEntrees
{
    /// <summary>
    /// Vérifie qu'une valeur est un pourcentage valide, compris entre 0 et 100.
    /// </summary>
    /// <param name="valeur">Valeur à contrôler, en pourcentage.</param>
    /// <param name="nomParametre">Nom du paramètre contrôlé, repris dans le message d'erreur.</param>
    /// <returns>Rien : la méthode lève une exception si la valeur n'est pas un pourcentage valide.</returns>
    /// <exception cref="ArgumentException">Si la valeur n'est pas un nombre ou sort de l'intervalle de 0 à 100.</exception>
    internal static void VerifierPourcentage(double valeur, string nomParametre)
    {
        bool estValide = valeur >= RegleNiveauCuve.PourcentageMinimum
            && valeur <= RegleNiveauCuve.PourcentageMaximum;

        if (!estValide)
        {
            throw new ArgumentException(
                $"Le paramètre « {nomParametre} » doit être un pourcentage compris entre 0 et 100.",
                nomParametre);
        }
    }

    /// <summary>
    /// Vérifie qu'une valeur est un nombre fini, positif ou nul.
    /// </summary>
    /// <param name="valeur">Valeur à contrôler (volume, débit, seuil…).</param>
    /// <param name="nomParametre">Nom du paramètre contrôlé, repris dans le message d'erreur.</param>
    /// <returns>Rien : la méthode lève une exception si la valeur est négative ou n'est pas un nombre fini.</returns>
    /// <exception cref="ArgumentException">Si la valeur est négative, infinie ou n'est pas un nombre.</exception>
    internal static void VerifierPositifOuNul(double valeur, string nomParametre)
    {
        bool estValide = double.IsFinite(valeur) && valeur >= 0;

        if (!estValide)
        {
            throw new ArgumentException(
                $"Le paramètre « {nomParametre} » doit être un nombre positif ou nul.",
                nomParametre);
        }
    }
}
