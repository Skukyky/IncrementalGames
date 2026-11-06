using System;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
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
    private AnimationCurve priceCurve;
    private AnimationCurve powerCurve;
    private new ItemData currentItemData;
    private double2 currentPrice;
    private double2 basePower;
    private int Level = 0;
    private int nbrRebirth;
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
        nbrRebirth = gameManager._numberOfRebirth;
        priceCurve = gameManager.priceCurve;
        
        if (itemNameText != null)
            itemNameText.text = currentItemData.UpgradeName;

        if (priceText != null)
            priceText.text = gameManagerInstance.goldConvert.MoneyConvert(currentItemData.basePriceUpgrade);

        if (iconImage != null && currentItemData.iconUpgrade != null)
            iconImage.sprite = currentItemData.iconUpgrade;
        
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        powerCurve = gameManager.powerCurve;
        basePower = currentItemData.baseDamageUpgrade;
        currentItemData.baseDamageUpgrade = ChangePower();
        if (hasInitializedManual)
        {
            gameManagerInstance.ApplyUpgradePickAxe(currentItemData.UpgradeName, currentItemData.baseDamageUpgrade,this);
        }
        CalculateNewPrice();
    }

    private void OnUpgradeButtonClicked()
    {
        if (Level < currentItemData.nbrAmelioMaxUpgrade && gameManagerInstance.VerifyCanPurchase(currentPrice))
        {
            Level++;
            if (Level == 1)
            {
                gestionSpawnUpgradeInstance.NewSpawn();
            }
            gameManagerInstance.CalculNewMoney(currentPrice, false);
            gameManagerInstance.CalculateNewMoney();
            CalculateNewPrice();
            if (!localHasTriggeredGlobalSpawn && currentItemData.automaticWorkerPrefab != null &&  !hasInitializedManual)
            {
                localHasTriggeredGlobalSpawn = true;
                gameManagerInstance.SpawnAutomaticWorker(currentItemData);
            }
            else if (hasInitializedManual)
            {
                gameManagerInstance.ApplyUpgradePickAxe(currentItemData.UpgradeName, ChangePower(),this);
                
            }
            
            currentItemData.baseDamageUpgrade = ChangePower();
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

        print("Curve: " + priceCurve.Evaluate((float)Level/100)*1000);
        currentPrice = new double2(currentItemData.basePriceUpgrade.x * (priceCurve.Evaluate((float)Level/100)*1000 + 1), currentItemData.basePriceUpgrade.y);
        while (currentPrice.x > 1000)
        {
            currentPrice.x /= 1000;
            currentPrice.y += 1;
        }
        
        RefreshPriceText(gameManagerInstance.goldConvert.MoneyConvert(currentPrice));
    }

    private double2 ChangePower()
    {
        double2 newPower = basePower;
        newPower.x *= (powerCurve.Evaluate((float)Level/100)*100 + 1) * nbrRebirth ;
        if (newPower.x > 1000)
        {
            newPower.x /= 1000;
            newPower.y += 1;
        }
        newPower.x = Math.Round(newPower.x, 3);
        return newPower;
    }

    public void Reset()
    {
        Level = 0;
        nbrRebirth = gameManagerInstance._numberOfRebirth;
        gameManagerInstance.ApplyUpgradePickAxe(currentItemData.UpgradeName, ChangePower(),this);
        CalculateNewPrice();
    }
    
}
