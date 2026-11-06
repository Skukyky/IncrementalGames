using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class GachaAndRebirth : MonoBehaviour
{
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private double2 basePrice;
    [SerializeField] private MoneyConvertAndSave moneyConvert;
    public double2 price;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        price = basePrice;
        priceText.text = moneyConvert.MoneyConvert(price);
    }

    // Update is called once per frame

    public void CalculateNewPriceRebirth(GameManager gameManager)
    {
        if (gameManager)
        {
            // Calculate the new price for rebirth
            price.x = Math.Pow(gameManager._numberOfRebirth * basePrice.x, 1.2);
            while (price.x > 1000)
            {
                price.x /= 1000;
                price.y += 1;
            }
        }
        priceText.text = moneyConvert.MoneyConvert(price);
    }
}
