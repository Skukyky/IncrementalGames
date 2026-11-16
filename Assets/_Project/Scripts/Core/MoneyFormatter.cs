using System;
using System.Collections.Generic;

/// <summary>
/// Transforme un BigNumber en texte (ex. "12,5K"). Les suffixes viennent de l'asset ValueMoney,
/// injecté une fois au démarrage par le CurrencyManager.
/// </summary>
public static class MoneyFormatter
{
    private static IReadOnlyList<string> _suffixes = Array.Empty<string>();

    public static void SetSuffixes(IReadOnlyList<string> suffixes)
    {
        _suffixes = suffixes ?? Array.Empty<string>();
    }

    public static string Format(BigNumber value)
    {
        if (_suffixes.Count == 0)
        {
            return "0";
        }

        int position = Math.Clamp(value.Exponent, 0, _suffixes.Count - 1);
        return FormatValue(value) + _suffixes[position];
    }

    private static string FormatValue(BigNumber value)
    {
        double mantissa = value.Mantissa;
        if (value.Exponent >= 1)
        {
            if (mantissa >= 100d)
            {
                return ((long)Math.Floor(mantissa)).ToString();
            }

            if (mantissa >= 10d)
            {
                double truncated = Math.Floor(mantissa * 10d) / 10d;
                return truncated.ToString("0.0").Replace('.', ',');
            }

            double smallTruncated = Math.Floor(mantissa * 100d) / 100d;
            return smallTruncated.ToString("0.00").Replace('.', ',');
        }

        return ((long)Math.Ceiling(mantissa)).ToString();
    }
}
