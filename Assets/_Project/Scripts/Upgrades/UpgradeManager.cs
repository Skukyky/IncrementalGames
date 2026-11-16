using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère la liste des améliorations : les fait apparaître une par une (la suivante se débloque
/// quand on achète la précédente), calcule la puissance du clic et sauvegarde / restaure leur état.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject prefabUpgrade;
    [SerializeField] private CurrencyManager currency;
    [SerializeField] private WorkerManager workers;
    [SerializeField] private RebirthManager rebirth;

    private readonly List<UpgradeItemUI> _items = new List<UpgradeItemUI>();
    private readonly Dictionary<string, BigNumber> _clickBonuses = new Dictionary<string, BigNumber>();

    /// <summary>Argent gagné par clic du joueur (somme des améliorations de type pioche).</summary>
    public BigNumber ClickPower { get; private set; } = BigNumber.One;

    public int RebirthCount => rebirth != null ? rebirth.RebirthCount : 1;

    /// <summary>Appelé par le GameManager au démarrage (ordre maîtrisé, pas de dépendance à Start).</summary>
    public void Initialize()
    {
        Clear();
        SpawnNext();
    }

    public void SpawnNext()
    {
        if (itemDatabase == null || _items.Count >= itemDatabase.items.Count)
            return;

        ItemData item = itemDatabase.items[_items.Count];
        GameObject instance = Instantiate(prefabUpgrade, scrollContent);
        UpgradeItemUI ui = instance.GetComponent<UpgradeItemUI>();
        _items.Add(ui);
        ui.Initialize(item, this, currency);
    }

    public void SetClickBonus(string upgradeName, BigNumber power)
    {
        _clickBonuses[upgradeName] = power;

        BigNumber total = BigNumber.Zero;
        foreach (BigNumber bonus in _clickBonuses.Values)
            total += bonus;
        ClickPower = total;
    }

    public void SpawnWorker(ItemData item, string assignedCharacter = null)
    {
        if (workers != null)
            workers.Spawn(item, assignedCharacter);
    }

    public UpgradeSaveData[] GetSaveData()
    {
        var result = new UpgradeSaveData[_items.Count];
        for (int i = 0; i < _items.Count; i++)
            result[i] = _items[i].GetSaveData();
        return result;
    }

    /// <summary>
    /// Restaure les niveaux sauvegardés. Chaque amélioration achetée débloque la suivante,
    /// exactement comme en jeu : on ne dépend ni de l'ordre ni du nombre d'entrées du fichier.
    /// </summary>
    public void Restore(UpgradeSaveData[] saved)
    {
        if (saved == null) return;

        var byName = new Dictionary<string, UpgradeSaveData>();
        foreach (UpgradeSaveData entry in saved)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.upgradeName))
                byName[entry.upgradeName] = entry;
        }

        for (int i = 0; i < _items.Count; i++)
        {
            UpgradeItemUI ui = _items[i];
            if (!byName.TryGetValue(ui.UpgradeName, out UpgradeSaveData entry))
                continue;

            ui.RestoreState(entry);
            if (entry.level > 0 && i == _items.Count - 1)
                SpawnNext();
        }
    }

    public void ResetForRebirth()
    {
        Initialize();
    }

    private void Clear()
    {
        foreach (UpgradeItemUI item in _items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        _items.Clear();
        _clickBonuses.Clear();
        ClickPower = BigNumber.One;
    }
}
