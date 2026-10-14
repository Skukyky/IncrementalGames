using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ItemData
{
    public string UpgradeName = "None";
    public int basePriceUpgrade = 1;
    public int nbrAmelioMaxUpgrade = 1;
    public int baseDamageUpgrade = 1;
    public Sprite iconUpgrade;
    public GameObject automaticWorkerPrefab;
    public Vector2 automaticWorkerSpawnPosition;
    public bool automaticWorkerFlipX;
    public CharacterData automaticWorkerCharacterData;
}