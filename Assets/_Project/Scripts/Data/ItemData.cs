using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string UpgradeName = "None";
    public BigNumber basePriceUpgrade = BigNumber.One;
    public int nbrAmelioMaxUpgrade = 1;
    public BigNumber baseDamageUpgrade = BigNumber.Zero;
    public Sprite iconUpgrade;
    public GameObject automaticWorkerPrefab;
    public Vector2 automaticWorkerSpawnPosition;
    public bool automaticWorkerFlipX;
    public CharacterData automaticWorkerCharacterData;

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