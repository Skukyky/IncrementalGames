using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ValueData", menuName = "Data/Value List")]
public class ValueMoney : ScriptableObject
{
    public List<string> items;
}