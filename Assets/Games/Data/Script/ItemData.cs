using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ItemData
{
    public new string UpgradeName = "None";
    public new double2 basePriceUpgrade = 1d;
    public new int nbrAmelioMaxUpgrade = 1;
    public new double2 baseDamageUpgrade =  new double2(0d, 0d);
    public new Sprite iconUpgrade;
    public new GameObject automaticWorkerPrefab;
    public new Vector2 automaticWorkerSpawnPosition;
    public new bool automaticWorkerFlipX;
    public new CharacterData automaticWorkerCharacterData;

    public ItemData Clone()
    {
        return new ItemData
        {
            UpgradeName = UpgradeName,
            basePriceUpgrade = basePriceUpgrade,
            nbrAmelioMaxUpgrade = nbrAmelioMaxUpgrade,
            baseDamageUpgrade = baseDamageUpgrade,
            iconUpgrade = iconUpgrade,
            automaticWorkerPrefab = automaticWorkerPrefab,
            automaticWorkerSpawnPosition = automaticWorkerSpawnPosition,
            automaticWorkerFlipX = automaticWorkerFlipX,
            automaticWorkerCharacterData = automaticWorkerCharacterData != null ? automaticWorkerCharacterData.Clone() : null
        };
    }
}