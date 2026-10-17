namespace AquaCuve.Partage.Modeles;

/// <summary>
/// Paramètres de réglage d'une installation (cuve) saisis lors de la mise en service.
/// Les options d'alerte, le téléphone et l'e-mail seront ajoutés avec la page de paramétrage (étape 7).
/// </summary>
/// <param name="HauteurMiniCm">Hauteur d'eau correspondant à 0 % (fond de cuve), en centimètres.</param>
/// <param name="HauteurMaxiCm">Hauteur d'eau correspondant à 100 % (cuve pleine), en centimètres.</param>
/// <param name="Seuils">Seuils de pilotage du remplissage, en pourcentage de la hauteur.</param>
public sealed record ParametresInstallation(
    double HauteurMiniCm,
    double HauteurMaxiCm,
    SeuilsRemplissage Seuils);
