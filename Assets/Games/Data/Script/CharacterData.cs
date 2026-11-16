using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Data/Character Database")]
public class CharacterData : ScriptableObject
{
    public string CharacterName = "None";
    public double FireRate = 1;
    public double CritRate = 0;
    public double CritDamage = 1.5;
    public Sprite icon;
    public Sprite sprite1;
    public Sprite sprite2;

    public CharacterData Clone()
    {
        CharacterData clone = CreateInstance<CharacterData>();
        clone.CharacterName = CharacterName;
        clone.FireRate = FireRate;
        clone.CritRate = CritRate;
        clone.CritDamage = CritDamage;
        clone.icon = icon;
        clone.sprite1 = sprite1;
        clone.sprite2 = sprite2;
        return clone;
    }
}