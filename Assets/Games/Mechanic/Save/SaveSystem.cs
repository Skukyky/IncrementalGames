using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public static class SaveSystem
{
    public const string FILENAME_SAVEDATA = "savedata.json";

    public static void SaveDataSystem()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("SaveSystem: GameManager.Instance is null, cannot save.");
            return;
        }

        List<UpgradeSaveData> upgrades = new List<UpgradeSaveData>();
        if (GameManager.Instance.GetGestionSpawnUpgrade() != null)
        {
            foreach (var spawnedUpgrade in GameManager.Instance.GetGestionSpawnUpgrade().SpawnedItems)
            {
                if (spawnedUpgrade == null)
                    continue;

                UpgradeItemUI upgradeUI = spawnedUpgrade.GetComponent<UpgradeItemUI>();
                if (upgradeUI == null)
                    continue;

                upgrades.Add(upgradeUI.GetSaveData());
            }
        }
        
        List<int> minerListIndex = new List<int>();
        GestionModifyWorkerAssignation WorkerAssignation = GameManager.Instance.gestionModifyWorkerAssignation;
        foreach (var character in WorkerAssignation.GetAvailableCharacters())
        {
            bool found = false;
            for (int i = 0; i < WorkerAssignation.GetDataMinerList().Count; i++)
            {
                if (WorkerAssignation.GetDataMinerList()[i].CharacterName == character.CharacterName)
                {
                    minerListIndex.Add(i);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                minerListIndex.Add(-1);
            }
            
            Debug.Log("Character " + minerListIndex[minerListIndex.Count - 1] + " added to save data.");
        }

        ItemSaveData data = new ItemSaveData
        {
            _money = GameManager.Instance.Coins,
            _rebirthCount = GameManager.Instance._numberOfRebirth,
            upgrades = upgrades.ToArray(),
            _minerListIndex = minerListIndex,
        };
        string path = Path.Combine(Application.persistentDataPath, FILENAME_SAVEDATA);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    public static void LoadDataSystem()
    {
        string path = Path.Combine(Application.persistentDataPath, FILENAME_SAVEDATA);
        if (!File.Exists(path))
        {
            return;
        }
        
        string json = File.ReadAllText(path);
        ItemSaveData loaded = JsonUtility.FromJson<ItemSaveData>(json);
        if (loaded == null)
        {
            return;
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Coins = loaded._money;
            GameManager.Instance._numberOfRebirth = loaded._rebirthCount;
            GameManager.Instance.RestoreUpgradeState(loaded.upgrades);
            GameManager.Instance.gestionModifyWorkerAssignation.LoadWorkerInListSave(loaded._minerListIndex);
        }
    }
}

/*
[Serializable]
public class SaveData
{
    public static ItemSaveData DataSaved()
    {
        ItemSaveData data = new ItemSaveData
        {
            _money = GameManager.Instance.Coins,
            _rebirthCount = GameManager.Instance._numberOfRebirth
        };
        return data;
    }
}
*/