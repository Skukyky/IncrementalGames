using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabaseList", menuName = "Data/Item Database List")]
public class ItemDatabase : ScriptableObject
{
    public new List<ItemData> items;
}