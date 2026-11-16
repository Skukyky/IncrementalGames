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

        // Toujours arrondi vers le bas : on n'affiche jamais plus que ce qu'on possède.
        // Les prix sont déjà sur cette grille (CeilToDisplay), ils s'affichent donc tels quels.
        BigNumber shown = value.FloorToDisplay();
        int position = Math.Clamp(shown.Exponent, 0, _suffixes.Count - 1);
        return FormatValue(shown) + _suffixes[position];
    }

    private static string FormatValue(BigNumber value)
    {
        double mantissa = value.Mantissa;
        if (value.Exponent == 0 || mantissa >= 100d)
            return Math.Round(mantissa).ToString("0");

        string text = mantissa >= 10d ? mantissa.ToString("0.0") : mantissa.ToString("0.00");
        return text.Replace('.', ',');
    }
}
