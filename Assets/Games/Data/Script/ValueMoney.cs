using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabaseList", menuName = "Data/Value List")]
public class ValueMoney : ScriptableObject
{
    public new List<string> items;
}