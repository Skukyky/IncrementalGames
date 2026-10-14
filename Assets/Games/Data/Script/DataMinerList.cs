using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DataMinerList", menuName = "Data/Data Miner List")]
public class DataMinerList : ScriptableObject
{
    public List<CharacterData> items;
}