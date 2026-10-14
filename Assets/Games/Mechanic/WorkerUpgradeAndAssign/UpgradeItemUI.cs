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
    [SerializeField] private GameManager gameManagerInstance;
    [SerializeField] private ItemData currentItemData;
    [SerializeField] private bool hasInitializedManual = false;
    private int currentPrice;
    private int Level = 1;
    private bool localHasTriggeredGlobalSpawn = false;
    
    

    public void Initialize(ItemData itemData, GameManager gameManager)
    {
        currentItemData = itemData;
        gameManagerInstance = gameManager;
        print("Initialized " + itemData.UpgradeName);
        
        if (itemNameText != null)
            itemNameText.text = itemData.UpgradeName;

        if (priceText != null)
            priceText.text = itemData.basePriceUpgrade.ToString();

        if (iconImage != null && itemData.iconUpgrade != null)
            iconImage.sprite = itemData.iconUpgrade;
        
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        CalculateNewPrice();
    }

    private void Start()
    {
        if (currentItemData != null && gameManagerInstance != null && hasInitializedManual)
        {
            Initialize(currentItemData, gameManagerInstance);
        }
    }

    private void OnUpgradeButtonClicked()
    {
        if (Level < currentItemData.nbrAmelioMaxUpgrade && gameManagerInstance.coins >= currentPrice)
        {
            Level++;
            gameManagerInstance.coins -= currentPrice;
            gameManagerInstance.CalculateNewMoney();
            CalculateNewPrice();
            if (!localHasTriggeredGlobalSpawn && currentItemData.automaticWorkerPrefab != null &&  !hasInitializedManual)
            {
                localHasTriggeredGlobalSpawn = true;
                gameManagerInstance.SpawnAutomaticWorker(currentItemData);
            }
            else if (hasInitializedManual)
            {
                gameManagerInstance.ApplyUpgradePickAxe(currentItemData.UpgradeName, (int)ChangePower());
            }
                    
            currentItemData.baseDamageUpgrade = (int)ChangePower();
        }
        
    }

    private void RefreshPriceText(String price)
    {
        priceText.text = price;
    }

    private void CalculateNewPrice()
    {
        if (Level >= currentItemData.nbrAmelioMaxUpgrade)
        {
            RefreshPriceText("Max Level");
            return;
        }
        currentPrice = currentItemData.basePriceUpgrade * Level;
        RefreshPriceText(currentPrice.ToString());
    }

    private double ChangePower()
    {
        //return currentItemData.basePower * Level;
        
        return Math.Pow(2, Level - 1); // Example: exponential growth based on level
    }
}
