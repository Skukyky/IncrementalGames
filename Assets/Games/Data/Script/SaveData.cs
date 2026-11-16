using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class UpgradeSaveData
{
    public string upgradeName;
    public int level;
    public bool automaticWorkerSpawned;
    public int workerIndex;
}

[Serializable]
public class ItemSaveData
{
    public double2 _money;
    public int _rebirthCount;
    public UpgradeSaveData[] upgrades;
    public List<int> _minerListIndex;    
    public ItemDatabase _dataUpgrade;
}