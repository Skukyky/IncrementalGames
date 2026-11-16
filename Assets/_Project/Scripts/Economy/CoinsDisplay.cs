using TMPro;
using UnityEngine;

/// <summary>Affiche le nombre de pièces et se met à jour tout seul quand il change.</summary>
public class CoinsDisplay : MonoBehaviour
{
    [SerializeField] private CurrencyManager currency;
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        if (currency == null) return;
        currency.CoinsChanged += Refresh;
        Refresh(currency.Coins);
    }

    private void OnDisable()
    {
        if (currency != null)
            currency.CoinsChanged -= Refresh;
    }

    private void Refresh(BigNumber coins)
    {
        if (text != null)
            text.text = MoneyFormatter.Format(coins);
    }
}
