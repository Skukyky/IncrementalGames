using System;
using UnityEngine;

public class MoneyConvertAndSave : MonoBehaviour
{
    private int factor;
    private string unit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public string ConvertMoneyLogic(double money)
    {
        if (money / (int)Math.Pow(10,factor*3) >= 1000)
        {
            factor++;
        }
        else if (money / (int)Math.Pow(10,factor*3) < 1 && factor > 0)
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
        print("money: " + money + ", factor: " + factor + ", unit: " + unit);
        return (money / (int)Math.Pow(10,factor*3)) + unit;
    }
}
