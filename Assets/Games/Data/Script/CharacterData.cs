using UnityEngine;
using UnityEngine.Serialization;

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
}