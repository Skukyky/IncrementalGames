using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int basePrice;
    public int basePower;
    public int nbrAmelioMax;
    public Sprite icon;
    public EnumType.TypeOfUpGrade upgradeType;
}