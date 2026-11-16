using System;
using UnityEngine;

/// <summary>
/// Point d'entrée de la partie : initialise les gestionnaires dans le bon ordre,
/// charge la sauvegarde, applique le gain hors-ligne et sauvegarde régulièrement.
///
/// Règle de dépendances du projet :
/// - les objets de la scène se référencent par des champs [SerializeField] ;
/// - les objets créés en jeu (lignes d'UI, mineurs) reçoivent leurs dépendances via Initialize(...).
/// Pas de singleton.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private CurrencyManager currency;
    [SerializeField] private UpgradeManager upgrades;
    [SerializeField] private WorkerManager workers;
    [SerializeField] private WorkerAssignmentManager assignments;
    [SerializeField] private RebirthManager rebirth;
    [SerializeField] private MoneyPopupSpawner popups;

    [Header("Sauvegarde")]
    [SerializeField] private float autoSaveInterval = 30f;

    [Header("Gain hors-ligne")]
    [Tooltip("Durée maximale prise en compte (heures).")]
    [SerializeField] private float maxOfflineHours = 8f;
    [Tooltip("Part du gain normal obtenue hors-ligne (1 = 100 %).")]
    [SerializeField] private float offlineEfficiency = 0.5f;

    private float _nextAutoSave;
    private bool _loaded;

    private void Start()
    {
        assignments.Initialize();

        ItemSaveData save = SaveSystem.Load();
        if (save != null)
            Restore(save);
        else
            upgrades.Initialize();

        _loaded = true;
        _nextAutoSave = Time.unscaledTime + autoSaveInterval;
    }

    private void Update()
    {
        if (_loaded && Time.unscaledTime >= _nextAutoSave)
        {
            Save();
            _nextAutoSave = Time.unscaledTime + autoSaveInterval;
        }
    }

    private void Restore(ItemSaveData save)
    {
        rebirth.SetRebirthCount(save._rebirthCount);
        assignments.Restore(save._minerListIndex);
        upgrades.Initialize(); // après SetRebirthCount : la puissance dépend du nombre de rebirths
        upgrades.Restore(save.upgrades);
        currency.SetCoins(save.money);
        ApplyOfflineEarnings(save.lastSaveUtcTicks);
    }

    private void ApplyOfflineEarnings(long lastSaveUtcTicks)
    {
        if (lastSaveUtcTicks <= 0)
            return;

        double seconds = (DateTime.UtcNow - new DateTime(lastSaveUtcTicks, DateTimeKind.Utc)).TotalSeconds;
        seconds = Math.Min(seconds, maxOfflineHours * 3600d);
        if (seconds < 1d)
            return;

        BigNumber earned = workers.EstimateIncomePerSecond() * (seconds * offlineEfficiency);
        if (earned.IsZero)
            return;

        currency.Add(earned);
        if (popups != null)
            popups.Show(earned, new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
    }

    public void Save()
    {
        if (!_loaded)
            return;

        SaveSystem.Save(new ItemSaveData
        {
            money = currency.Coins,
            _rebirthCount = rebirth.RebirthCount,
            upgrades = upgrades.GetSaveData(),
            _minerListIndex = assignments.GetSaveData(),
            lastSaveUtcTicks = DateTime.UtcNow.Ticks,
        });
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
