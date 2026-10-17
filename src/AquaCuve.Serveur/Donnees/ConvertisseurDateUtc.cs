using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AquaCuve.Serveur.Donnees;

/// <summary>
/// Convertisseur EF Core qui garantit que les dates lues en base (SQLite ne conserve pas le type de date) sont en UTC.
/// À l'écriture, une date locale est convertie en UTC ; une date sans indication de type est supposée locale.
/// </summary>
public sealed class ConvertisseurDateUtc : ValueConverter<DateTime, DateTime>
{
    /// <summary>
    /// Crée le convertisseur : conversion en UTC à l'écriture, marquage UTC à la lecture.
    /// </summary>
    public ConvertisseurDateUtc()
        : base(
            date => date.ToUniversalTime(),
            date => DateTime.SpecifyKind(date, DateTimeKind.Utc))
    {
    }
}
