using System.Globalization;

namespace AlSoque.Data.Extensions;

public static class DateHelper
{
    private static readonly HijriCalendar HijriCalendar = new();

    public static int? ToGregorian(this int? hijriYear)
    {
        if (hijriYear is null or < 1)
        {
            return null;
        }
        try
        {
            return HijriCalendar.ToDateTime(hijriYear.Value, 1, 1, 0, 0, 0, 0).Year;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    public static int? ToHijri(this int? gregorianYear)
    {
        if (gregorianYear is null or < 1)
        {
            return null;
        }
        try
        {
            return HijriCalendar.GetYear(new DateTime(gregorianYear.Value, 1, 1));
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }
}
