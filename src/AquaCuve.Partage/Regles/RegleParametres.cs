using AquaCuve.Partage.Modeles;

namespace AquaCuve.Partage.Regles;

/// <summary>
/// Règles de cohérence des paramètres d'une installation (utilisées par la page de paramétrage et le simulateur).
/// </summary>
public static class RegleParametres
{
    /// <summary>
    /// Vérifie la cohérence de l'ensemble des paramètres d'une installation.
    /// </summary>
    /// <param name="parametres">Paramètres à contrôler (hauteurs mini et maxi, seuils de remplissage).</param>
    /// <returns>Rien : la méthode lève une exception si un paramètre est incohérent.</returns>
    /// <exception cref="ArgumentException">Si les hauteurs ou les seuils sont incohérents.</exception>
    public static void Verifier(ParametresInstallation parametres)
    {
        VerifierHauteursCoherentes(parametres.HauteurMiniCm, parametres.HauteurMaxiCm);
        VerifierSeuils(parametres.Seuils);
    }

    /// <summary>
    /// Vérifie que la hauteur maxi est strictement supérieure à la hauteur mini.
    /// </summary>
    /// <param name="hauteurMiniCm">Hauteur correspondant à 0 %, en centimètres.</param>
    /// <param name="hauteurMaxiCm">Hauteur correspondant à 100 %, en centimètres.</param>
    /// <returns>Rien : la méthode lève une exception si les hauteurs sont incohérentes.</returns>
    /// <exception cref="ArgumentException">Si hauteurMaxiCm est inférieure ou égale à hauteurMiniCm, ou si une valeur n'est pas un nombre.</exception>
    public static void VerifierHauteursCoherentes(double hauteurMiniCm, double hauteurMaxiCm)
    {
        // La forme « !(a > b) » rejette aussi les valeurs qui ne sont pas des nombres (NaN).
        if (!(hauteurMaxiCm > hauteurMiniCm))
        {
            throw new ArgumentException(
                "La hauteur maxi doit être strictement supérieure à la hauteur mini.");
        }
    }

    /// <summary>
    /// Vérifie que les seuils sont des pourcentages valides et rangés dans l'ordre sécurité &lt; ville &lt; forage.
    /// </summary>
    /// <param name="seuils">Seuils de remplissage à contrôler, en pourcentage de la hauteur.</param>
    /// <returns>Rien : la méthode lève une exception si un seuil est invalide ou mal ordonné.</returns>
    /// <exception cref="ArgumentException">Si un seuil sort de l'intervalle de 0 à 100 ou si l'ordre n'est pas respecté.</exception>
    public static void VerifierSeuils(SeuilsRemplissage seuils)
    {
        VerificationsEntrees.VerifierPourcentage(seuils.SeuilSecuritePourcent, nameof(seuils.SeuilSecuritePourcent));
        VerificationsEntrees.VerifierPourcentage(seuils.SeuilVillePourcent, nameof(seuils.SeuilVillePourcent));
        VerificationsEntrees.VerifierPourcentage(seuils.SeuilForagePourcent, nameof(seuils.SeuilForagePourcent));
        VerifierOrdreDesSeuils(seuils);
    }

    /// <summary>
    /// Vérifie que le seuil de sécurité est inférieur au seuil ville, lui-même inférieur au seuil forage.
    /// </summary>
    /// <param name="seuils">Seuils de remplissage dont l'ordre est contrôlé.</param>
    /// <returns>Rien : la méthode lève une exception si l'ordre n'est pas respecté.</returns>
    /// <exception cref="ArgumentException">Si les seuils ne sont pas strictement croissants de la sécurité vers le forage.</exception>
    private static void VerifierOrdreDesSeuils(SeuilsRemplissage seuils)
    {
        bool ordreRespecte = seuils.SeuilSecuritePourcent < seuils.SeuilVillePourcent
            && seuils.SeuilVillePourcent < seuils.SeuilForagePourcent;

        if (!ordreRespecte)
        {
            throw new ArgumentException(
                "Les seuils doivent respecter l'ordre : sécurité inférieure à ville, ville inférieure à forage.");
        }
    }
}
