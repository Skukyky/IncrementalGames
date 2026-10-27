using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Data/Character Database")]
public class CharacterData : ScriptableObject
{
    public new string CharacterName = "None";
    public new double FireRate = 1;
    public new double CritRate = 0;
    public new double CritDamage = 1.5;
    public new Sprite icon;
    public new Sprite sprite1;
    public new Sprite sprite2;

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