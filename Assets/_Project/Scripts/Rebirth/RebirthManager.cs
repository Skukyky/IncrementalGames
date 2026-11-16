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
    [Tooltip("Libellé de la ligne : affiche les gemmes que rapportera le prochain rebirth.")]
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private BigNumber basePrice;
    [Tooltip("Multiplicateur du prix à chaque rebirth. Doit dépasser le bonus (×nombre de rebirths) pour que chaque cycle dure un peu plus.")]
    [SerializeField] private double priceGrowth = 3d;
    [SerializeField] private CurrencyManager currency;
    [SerializeField] private UpgradeManager upgrades;
    [SerializeField] private WorkerManager workers;
    [SerializeField] private WorkerAssignmentManager assignments;
    [SerializeField] private GachaManager gacha;
    [Tooltip("Gemmes gagnées par rebirth, multipliées par le numéro du rebirth (1, puis 2, puis 3...).")]
    [SerializeField] private int gemsPerRebirth = 1;

    public int RebirthCount { get; private set; } = 1;

    /// <summary>Gemmes que rapportera le prochain rebirth.</summary>
    public int NextGemReward => gemsPerRebirth * RebirthCount;
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

        int gemReward = NextGemReward;
        currency.SetCoins(BigNumber.Zero);
        RebirthCount++;
        RefreshPrice();
        if (gacha != null)
            gacha.AddGems(gemReward);

        workers.ClearAll();
        assignments.ResetForRebirth();
        upgrades.ResetForRebirth();
    }

    private void RefreshPrice()
    {
        // Prix = prix de base × croissance^(rebirths effectués), arrondi au-dessus à la précision affichée.
        Price = (basePrice * Math.Pow(priceGrowth, RebirthCount - 1)).CeilToDisplay();
        if (priceText != null)
            priceText.text = MoneyFormatter.Format(Price);
        if (labelText != null)
            labelText.text = "Rebirth (+" + NextGemReward + (NextGemReward > 1 ? " gemmes)" : " gemme)");
    }
}
