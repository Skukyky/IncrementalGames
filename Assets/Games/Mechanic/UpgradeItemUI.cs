using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour, IUpgradeUI
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button upgradeButton;

    
    private ItemData currentItemData;
    private GameManager gameManagerInstance;
    private int currentPrice;
    private int Level = 1;

    public void Initialize(ItemData itemData, GameManager gameManager)
    {
        currentItemData = itemData;
        gameManagerInstance = gameManager;
        print("Initialized " + itemData.itemName);
        
        if (itemNameText != null)
            itemNameText.text = itemData.itemName;

        if (priceText != null)
            priceText.text = itemData.basePrice.ToString();

        if (iconImage != null && itemData.icon != null)
            iconImage.sprite = itemData.icon;
        
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        CalculateNewPrice();
    }

    private void OnUpgradeButtonClicked()
    {
        if (Level < currentItemData.nbrAmelioMax && gameManagerInstance.coins >= currentPrice)
        {
            Level++;
            gameManagerInstance.coins -= currentPrice;
            gameManagerInstance.CalculateNewMoney();
            CalculateNewPrice();
            switch (currentItemData.upgradeType)
            {
                case EnumType.TypeOfUpGrade.Click:
                    gameManagerInstance.ApplyUpgradePickAxe(currentItemData.itemName, ChangePower());
                    break;
                case EnumType.TypeOfUpGrade.Automatic:

                    break;
            }
        }
        
    }

    private void RefreshPriceText(String price)
    {
        priceText.text = price;
    }

    private void CalculateNewPrice()
    {
        if (Level >= currentItemData.nbrAmelioMax)
        {
            RefreshPriceText("Max Level");
            return;
        }
        currentPrice = currentItemData.basePrice * Level;
        RefreshPriceText(currentPrice.ToString());
    }

    private int ChangePower()
    {
        int Power = currentItemData.basePower * Level;
        return Power;
    }
}
