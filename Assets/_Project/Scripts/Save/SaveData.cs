using System;
using System.Collections.Generic;

[Serializable]
public class UpgradeSaveData
{
    public string upgradeName;
    public int level;
    public bool automaticWorkerSpawned;
}

[Serializable]
public class ItemSaveData
{
    public BigNumber money;
    public int _rebirthCount;
    public UpgradeSaveData[] upgrades;
    public List<int> _minerListIndex;

    // Date de la sauvegarde (UTC, en ticks) pour calculer le gain hors-ligne. 0 = ancienne sauvegarde.
    public long lastSaveUtcTicks;

    // Ancien format de l'argent ({"x": mantisse, "y": exposant}), relu une fois puis abandonné.
    // JsonUtility ignore [FormerlySerializedAs], d'où cette migration explicite.
    public LegacyNumber _money;

    public void MigrateLegacyFields()
    {
        if (money.IsZero && (_money.x != 0d || _money.y != 0d))
            money = new BigNumber(_money.x, (int)_money.y);
        _money = default;
    }
}

[Serializable]
public struct LegacyNumber
{
    public double x;
    public double y;
}
