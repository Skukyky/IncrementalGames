using System;
using System.Collections.Generic;
using Unity.Mathematics;

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
    public double2 _money;
    public int _rebirthCount;
    public UpgradeSaveData[] upgrades;
    public List<int> _minerListIndex;
}