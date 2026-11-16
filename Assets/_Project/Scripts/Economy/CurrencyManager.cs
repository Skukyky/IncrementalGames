using System;
using UnityEngine;

/// <summary>
/// Possède l'argent du joueur. Seul endroit où les pièces sont ajoutées ou dépensées.
/// L'UI s'abonne à CoinsChanged au lieu d'être mise à jour à la main.
/// </summary>
[DefaultExecutionOrder(-100)]
public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private ValueMoney moneySuffixes;

    public event Action<BigNumber> CoinsChanged;

    private BigNumber _coins;
    public BigNumber Coins => _coins;

    private void Awake()
    {
        MoneyFormatter.SetSuffixes(moneySuffixes != null ? moneySuffixes.items : null);
    }

    public void Add(BigNumber amount)
    {
        _coins += amount;
        CoinsChanged?.Invoke(_coins);
    }

    public bool CanAfford(BigNumber price) => _coins >= price;

    public bool TrySpend(BigNumber price)
    {
        if (!CanAfford(price))
            return false;

        _coins -= price;
        if (_coins < BigNumber.Zero)
            _coins = BigNumber.Zero;
        CoinsChanged?.Invoke(_coins);
        return true;
    }

    public void SetCoins(BigNumber value)
    {
        _coins = value;
        CoinsChanged?.Invoke(_coins);
    }
}
