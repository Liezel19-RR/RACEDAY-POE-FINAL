using System.Globalization;

namespace RaceDay.Mvc.Services;

public static class DisplayFormatters
{
    public static string Date(DateTime value) => value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
    public static string Distance(decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture) + " km";
    public static string Price(decimal value) => value.ToString("C", CultureInfo.GetCultureInfo("en-ZA"));
}
