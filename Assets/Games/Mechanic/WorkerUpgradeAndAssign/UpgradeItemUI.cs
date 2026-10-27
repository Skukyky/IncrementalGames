using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameManager gameManagerInstance;
    [SerializeField] private ItemData currentItemDataNew;
    [SerializeField] private bool hasInitializedManual = false;
    [SerializeField] private GestionSpawnUpgrade gestionSpawnUpgradeInstance;
    private new ItemData currentItemData;
    private double currentPrice;
    private int Level = 1;
    private bool localHasTriggeredGlobalSpawn = false;
    
    private void Start()
    {
        if (currentItemDataNew != null && gameManagerInstance != null && hasInitializedManual)
        {
            Initialize(currentItemDataNew, gameManagerInstance, gestionSpawnUpgradeInstance);
        }
    }
    
    public void Initialize(ItemData itemData, GameManager gameManager,GestionSpawnUpgrade parentRef)
    {
        currentItemData = itemData != null ? itemData.Clone() : null;
        gameManagerInstance = gameManager;
        gestionSpawnUpgradeInstance = parentRef;
        print("Initialized " + currentItemData.baseDamageUpgrade);
        
        if (itemNameText != null)
            itemNameText.text = currentItemData.UpgradeName;

        if (priceText != null)
            priceText.text = gameManagerInstance != null ? gameManagerInstance.goldConvert.ConvertMoneyLogic(currentItemData.basePriceUpgrade) : currentItemData.basePriceUpgrade.ToString();

        if (iconImage != null && currentItemData.iconUpgrade != null)
            iconImage.sprite = currentItemData.iconUpgrade;
        
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        CalculateNewPrice();
    }

    private void OnUpgradeButtonClicked()
    {
        if (Level < currentItemData.nbrAmelioMaxUpgrade && gameManagerInstance.coins >= currentPrice)
        {
            Level++;
            if (Level == 2)
            {
                gestionSpawnUpgradeInstance.NewSpawn();
            }
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
                gameManagerInstance.ApplyUpgradePickAxe(currentItemData.UpgradeName, ChangePower());
            }
                     
            currentItemData.baseDamageUpgrade = (int)ChangePower();
            print("New Power: " + currentItemData.baseDamageUpgrade);
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
        RefreshPriceText(gameManagerInstance != null ? gameManagerInstance.goldConvert.ConvertMoneyLogic(currentPrice) : currentPrice.ToString());
    }

    private double ChangePower()
    {
        return currentItemData.baseDamageUpgrade * (Level - 1);
    }
}
