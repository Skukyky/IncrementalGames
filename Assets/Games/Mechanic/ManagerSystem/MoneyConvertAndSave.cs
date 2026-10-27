using System;
using UnityEngine;

public class MoneyConvertAndSave : MonoBehaviour
{
    public enum MoneyScale
    {
        None,
        K,
        M,
        Md
    }

    private static MoneyScale GetScale(double money)
    {
        double absoluteValue = Math.Abs(money);
        if (absoluteValue >= 1_000_000_000d)
        {
            return MoneyScale.Md;
        }
        
        if (absoluteValue >= 1_000_000d)
        {
            return MoneyScale.M;
        }

        if (absoluteValue >= 1_000d)
        {
            return MoneyScale.K;
        }

        return MoneyScale.None;
    }

    private static double GetScaledValue(double money, MoneyScale scale)
    {
        switch (scale)
        {
            case MoneyScale.K:
                return money / 1_000d;
            case MoneyScale.M:
                return money / 1_000_000d;
            case MoneyScale.Md:
                return money / 1_000_000_000d;
            default:
                return money;
        }
    }

    private static string FormatValue(double value)
    {
        if (value >= 100d)
        {
            return ((long)Math.Floor(value)).ToString();
        }

        if (value >= 10d)
        {
            double truncated = Math.Floor(value * 10d) / 10d;
            return truncated.ToString("0.0").Replace('.', ',');
        }

        double tinyTruncated = Math.Floor(value * 100d) / 100d;
        return tinyTruncated.ToString("0.00").Replace('.', ',').TrimEnd('0').TrimEnd(',');
    }

    public string ConvertMoneyLogic(double money)
    {
        if (double.IsNaN(money) || double.IsInfinity(money))
        {
            return "0";
        }

        bool isNegative = money < 0d;
        double absoluteValue = Math.Abs(money);
        MoneyScale scale = GetScale(absoluteValue);
        double scaledValue = GetScaledValue(absoluteValue, scale);
        string suffix = scale == MoneyScale.K ? "K" : scale == MoneyScale.M ? "M" : scale == MoneyScale.Md ? "Md" : string.Empty;
        string valueText = FormatValue(scaledValue);

        if (isNegative)
        {
            valueText = "-" + valueText;
        }

        return valueText + suffix;
    }
}
