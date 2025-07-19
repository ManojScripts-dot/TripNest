using System.Globalization;

namespace TripNest.Services
{
    public static class CurrencyHelper
    {
        public static string FormatCurrency(this decimal amount)
        {
            var culture = new CultureInfo("en-US");
            return amount.ToString("C", culture);
        }

        public static string FormatCurrency(this double amount)
        {
            var culture = new CultureInfo("en-US");
            return amount.ToString("C", culture);
        }

        public static string FormatCurrency(this int amount)
        {
            var culture = new CultureInfo("en-US");
            return amount.ToString("C", culture);
        }

        public static string FormatUSD(this decimal amount)
        {
            return $"${amount:N2}";
        }

        public static string FormatUSD(this double amount)
        {
            return $"${amount:N2}";
        }

        public static string FormatUSD(this int amount)
        {
            return $"${amount:N2}";
        }
    }
}