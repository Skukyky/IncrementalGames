using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Une ligne de la liste d'améliorations : nom, icône, prix et bouton d'achat.
/// Une amélioration est soit une pioche (augmente le clic), soit un mineur automatique.
/// </summary>
public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button upgradeButton;

    private UpgradeManager _owner;
    private CurrencyManager _currency;
    private ItemData _item;
    private BigNumber _basePower;
    private BigNumber _currentPrice;
    private int _level;
    private int _rebirthCount;
    private bool _workerSpawned;

    public string UpgradeName => _item != null ? _item.UpgradeName : string.Empty;

    private bool IsWorkerUpgrade => _item.automaticWorkerPrefab != null || _item.automaticWorkerCharacterData != null;

    public void Initialize(ItemData itemData, UpgradeManager owner, CurrencyManager currency)
    {
        _item = itemData != null ? itemData.Clone() : null;
        _owner = owner;
        _currency = currency;
        _rebirthCount = owner.RebirthCount;
        if (_item == null) return;

        if (itemNameText != null)
            itemNameText.text = _item.UpgradeName;

        if (iconImage != null && _item.iconUpgrade != null)
            iconImage.sprite = _item.iconUpgrade;

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        _basePower = _item.baseDamageUpgrade;
        _item.baseDamageUpgrade = ComputePower();

        if (!IsWorkerUpgrade)
            _owner.SetClickBonus(_item.UpgradeName, _item.baseDamageUpgrade);

        RefreshPrice();
    }

    public UpgradeSaveData GetSaveData()
    {
        return new UpgradeSaveData
        {
            upgradeName = UpgradeName,
            level = _level,
            automaticWorkerSpawned = _workerSpawned,
            // _item est partagé avec le mineur : il contient le personnage actuellement assigné.
            assignedCharacter = _workerSpawned && _item.automaticWorkerCharacterData != null
                ? _item.automaticWorkerCharacterData.CharacterName
                : null,
        };
    }

    public void RestoreState(UpgradeSaveData save)
    {
        if (save == null || _item == null)
            return;

        _level = save.level;
        _workerSpawned = save.automaticWorkerSpawned;
        _item.baseDamageUpgrade = ComputePower();

        if (_level > 0)
        {
            if (IsWorkerUpgrade)
            {
                if (_workerSpawned)
                    _owner.SpawnWorker(_item, save.assignedCharacter);
            }
            else
            {
                _owner.SetClickBonus(_item.UpgradeName, _item.baseDamageUpgrade);
            }
        }

        RefreshPrice();
    }

    private void OnUpgradeButtonClicked()
    {
        if (_level >= _item.nbrAmelioMaxUpgrade || !_currency.TrySpend(_currentPrice))
            return;

        _level++;
        if (_level == 1)
            _owner.SpawnNext();

        RefreshPrice();

        // Le mineur garde une référence sur _item : modifier sa puissance ci-dessous
        // augmente aussi ce que rapporte le mineur déjà placé.
        _item.baseDamageUpgrade = ComputePower();

        if (!_workerSpawned && _item.automaticWorkerPrefab != null)
        {
            _workerSpawned = true;
            _owner.SpawnWorker(_item);
        }
        else if (!IsWorkerUpgrade)
        {
            _owner.SetClickBonus(_item.UpgradeName, _item.baseDamageUpgrade);
        }
    }

    private void RefreshPrice()
    {
        bool isMaxLevel = _level >= _item.nbrAmelioMaxUpgrade;
        if (!isMaxLevel)
        {
            float curve = _owner.PriceCurve.Evaluate(_level / 100f);
            _currentPrice = _item.basePriceUpgrade * (curve * 1000d + 1d);
        }

        if (priceText != null)
            priceText.text = isMaxLevel ? "Max Level" : MoneyFormatter.Format(_currentPrice);
    }

    private BigNumber ComputePower()
    {
        float curve = _owner.PowerCurve.Evaluate(_level / 100f);
        return (_basePower * ((curve * 100d + 1d) * _rebirthCount)).Round(3);
    }
}
