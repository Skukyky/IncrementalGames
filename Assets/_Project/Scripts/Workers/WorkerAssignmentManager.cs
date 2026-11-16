using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Onglet "Assignation" : tient la réserve de personnages disponibles et une ligne par mineur.
/// Changer le personnage d'un mineur remet l'ancien dans la réserve.
/// </summary>
public class WorkerAssignmentManager : MonoBehaviour
{
    [FormerlySerializedAs("_dataMinerList")] [SerializeField] private DataMinerList minerDatabase;
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject prefabAssign;
    [SerializeField] private CharacterData defaultCharacterData;

    private readonly List<CharacterData> _available = new List<CharacterData>();
    private readonly List<WorkerAssignmentItem> _items = new List<WorkerAssignmentItem>();

    /// <summary>Réserve de départ : une copie de chaque personnage de la base.</summary>
    public void Initialize()
    {
        ClearItems();
        _available.Clear();
        if (minerDatabase == null || minerDatabase.MinerList == null)
            return;

        foreach (CharacterData character in minerDatabase.MinerList)
        {
            if (character != null)
                _available.Add(character.Clone());
        }
    }

    public void AddWorker(AutomaticWorker worker)
    {
        GameObject instance = Instantiate(prefabAssign, scrollContent);
        WorkerAssignmentItem item = instance.GetComponentInChildren<WorkerAssignmentItem>(true);
        _items.Add(item);
        item.Initialize(worker, this, _available);
    }

    public void SwapAssignment(CharacterData newCharacter, CharacterData oldCharacter)
    {
        if (newCharacter == null || oldCharacter == null)
            return;

        int index = _available.FindIndex(c => c.CharacterName == newCharacter.CharacterName);
        if (index >= 0)
            _available[index] = oldCharacter;

        foreach (WorkerAssignmentItem item in _items)
            item.ReloadOptions(_available);
    }

    public void ResetForRebirth()
    {
        Initialize();
    }

    /// <summary>Index de chaque personnage de la réserve dans la base (-1 = personnage par défaut).</summary>
    public List<int> GetSaveData()
    {
        var indices = new List<int>();
        List<CharacterData> database = minerDatabase != null ? minerDatabase.MinerList : null;
        foreach (CharacterData character in _available)
        {
            int index = database != null ? database.FindIndex(c => c != null && c.CharacterName == character.CharacterName) : -1;
            indices.Add(index);
        }
        return indices;
    }

    /// <summary>Copie du personnage portant ce nom (base des mineurs ou personnage par défaut), sinon null.</summary>
    public CharacterData FindCharacter(string characterName)
    {
        if (minerDatabase != null && minerDatabase.MinerList != null)
        {
            CharacterData match = minerDatabase.MinerList.Find(c => c != null && c.CharacterName == characterName);
            if (match != null)
                return match.Clone();
        }

        if (defaultCharacterData != null && defaultCharacterData.CharacterName == characterName)
            return defaultCharacterData.Clone();

        return null;
    }

    public void Restore(List<int> savedIndices)
    {
        if (savedIndices == null)
            return;

        _available.Clear();
        foreach (int index in savedIndices)
        {
            if (index >= 0 && index < minerDatabase.MinerList.Count)
                _available.Add(minerDatabase.MinerList[index].Clone());
            else if (defaultCharacterData != null)
                _available.Add(defaultCharacterData.Clone());
        }
    }

    private void ClearItems()
    {
        foreach (WorkerAssignmentItem item in _items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        _items.Clear();
    }
}
