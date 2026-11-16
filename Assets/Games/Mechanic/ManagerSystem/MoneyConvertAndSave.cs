using System;
using Unity.Mathematics;
using UnityEngine;

public class MoneyConvertAndSave : MonoBehaviour
{
    [SerializeField] private ValueMoney valueMoney;
    
    double2 value;

    public string MoneyConvert(double2 valueConvert)
    {
        value = valueConvert;

        if (!valueMoney || valueMoney.items == null || valueMoney.items.Count == 0)
        {
            return "0";
        }

        int position = (int)value.y;
        if (position < 0)
        {
            position = 0;
        }
        else if (position >= valueMoney.items.Count)
        {
            position = valueMoney.items.Count - 1;
        }

        string result = FormatValue(value) + valueMoney.items[position];
        return result;
    }
    
    private static string FormatValue(double2 value)
    {
        if (value.y >= 1)
        {
            if (value.x >= 100d)
            {
                return ((long)Math.Floor(value.x)).ToString();
            }

            else if (value.x >= 10d)
            {
                double truncated = Math.Floor(value.x * 10d) / 10d;
                return truncated.ToString("0.0").Replace('.', ',');
            }

            else
            {
                double truncated = Math.Floor(value.x * 100d) / 100d;
                return truncated.ToString("0.00").Replace('.', ',');
            }
        }
        return ((long)Math.Ceiling(value.x)).ToString();
    }
}
