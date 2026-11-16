using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Rebirth : contre une somme qui augmente à chaque fois, remet la partie à zéro
/// et multiplie la puissance de toutes les améliorations par le nombre de rebirths.
/// </summary>
public class RebirthManager : MonoBehaviour
{
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private BigNumber basePrice;
    [SerializeField] private CurrencyManager currency;
    [SerializeField] private UpgradeManager upgrades;
    [SerializeField] private WorkerManager workers;
    [SerializeField] private WorkerAssignmentManager assignments;

    public int RebirthCount { get; private set; } = 1;
    public BigNumber Price { get; private set; }

    private void Start()
    {
        RefreshPrice();
    }

    public void SetRebirthCount(int count)
    {
        RebirthCount = Math.Max(1, count);
        RefreshPrice();
    }

    /// <summary>Branché sur le bouton de l'onglet Gacha.</summary>
    public void Rebirth()
    {
        if (!currency.CanAfford(Price))
            return;

        currency.SetCoins(BigNumber.Zero);
        RebirthCount++;
        RefreshPrice();

        workers.ClearAll();
        assignments.ResetForRebirth();
        upgrades.ResetForRebirth();
    }

    private void RefreshPrice()
    {
        // Le prix repart toujours du prix de base (l'ancien calcul cumulait l'exposant à chaque rebirth).
        Price = new BigNumber(Math.Pow(RebirthCount * basePrice.Mantissa, 1.2), basePrice.Exponent);
        if (priceText != null)
            priceText.text = MoneyFormatter.Format(Price);
    }
}
