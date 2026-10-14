using System;
using UnityEngine;

public class MoneyConvertAndSave : MonoBehaviour
{
    private int factor = 0;
    private string unit;

    public string ConvertMoneyLogic(double money)
    {
        if (money / Math.Pow(10, factor * 3) >= 1000)
        {
            factor++;
        }
        else if (money / Math.Pow(10, factor * 3) < 1 && factor > 0)
        {
            factor--;
        }
        switch (factor)
        {
            case 0:
                unit = "";
                break;
            case 1:
                unit = " K";
                break;
            case 2:
                unit = " M";
                break;
        }
        double value = money / Math.Pow(10, factor * 3);
        // Arrondir à une décimale, mais n'afficher la décimale que si nécessaire (12 au lieu de 12,0)
        double rounded = Math.Round(value, 1);
        if (Math.Abs(rounded - Math.Truncate(rounded)) < 0.000001)
        {
            return ((long)rounded).ToString() + unit;
        }
        return rounded.ToString("F1") + unit;
    }
}
