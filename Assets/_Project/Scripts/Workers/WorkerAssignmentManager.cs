using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Onglet "Assignation" : tient la réserve des personnages possédés (obtenus au gacha, doublons
/// compris) et une ligne par mineur. Le personnage par défaut (Dummy) est toujours disponible
/// en quantité illimitée : il n'entre jamais dans la réserve.
/// La réserve est conservée au rebirth.
/// </summary>
public class WorkerAssignmentManager : MonoBehaviour
{
    [FormerlySerializedAs("_dataMinerList")] [SerializeField] private DataMinerList minerDatabase;
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject prefabAssign;
    [SerializeField] private CharacterData defaultCharacterData;

    private readonly List<CharacterData> _pool = new List<CharacterData>();
    private readonly List<WorkerAssignmentItem> _items = new List<WorkerAssignmentItem>();

    /// <summary>Personnages possédés et libres (un élément par exemplaire).</summary>
    public IReadOnlyList<CharacterData> Pool => _pool;
    public CharacterData DefaultCharacter => defaultCharacterData;

    /// <summary>Nouvelle partie : réserve vide, seul le personnage par défaut est disponible.</summary>
    public void Initialize()
    {
        ClearItems();
        _pool.Clear();
    }

    public void AddWorker(AutomaticWorker worker)
    {
        GameObject instance = Instantiate(prefabAssign, scrollContent);
        WorkerAssignmentItem item = instance.GetComponentInChildren<WorkerAssignmentItem>(true);
        _items.Add(item);
        item.Initialize(worker, this);
    }

    public bool IsDefault(string characterName) =>
        defaultCharacterData != null && characterName == defaultCharacterData.CharacterName;

    /// <summary>Ajoute un exemplaire à la réserve (gacha, ou retour d'un mineur). Ignore le personnage par défaut.</summary>
    public void ReturnToPool(CharacterData character)
    {
        if (character == null || IsDefault(character.CharacterName))
            return;

        _pool.Add(character);
        RefreshItems();
    }

    /// <summary>Vrai si le joueur possède déjà ce personnage (dans la réserve ou sur un mineur).</summary>
    public bool Owns(string characterName)
    {
        if (_pool.Exists(c => c.CharacterName == characterName))
            return true;

        foreach (WorkerAssignmentItem item in _items)
        {
            if (item != null && item.AssignedName == characterName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Met ce personnage sur le mineur : il sort de la réserve, et l'ancien y retourne
    /// (sauf le personnage par défaut, toujours disponible).
    /// </summary>
    public void Assign(AutomaticWorker worker, string characterName)
    {
        if (worker == null || worker.ItemData == null)
            return;

        CharacterData current = worker.ItemData.automaticWorkerCharacterData;
        if (current != null && current.CharacterName == characterName)
            return;

        CharacterData next;
        if (IsDefault(characterName))
        {
            next = defaultCharacterData.Clone();
        }
        else
        {
            int index = _pool.FindIndex(c => c.CharacterName == characterName);
            if (index < 0)
                return;
            next = _pool[index];
            _pool.RemoveAt(index);
        }

        worker.ModifyCharacterData(next);
        if (current != null && !IsDefault(current.CharacterName))
            _pool.Add(current);

        RefreshItems();
    }

    /// <summary>Rebirth : les lignes disparaissent avec les mineurs, la réserve reste.</summary>
    public void ResetForRebirth()
    {
        ClearItems();
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

        if (IsDefault(characterName))
            return defaultCharacterData.Clone();

        return null;
    }

    /// <summary>Index de chaque exemplaire de la réserve dans la base des mineurs.</summary>
    public List<int> GetSaveData()
    {
        var indices = new List<int>();
        List<CharacterData> database = minerDatabase != null ? minerDatabase.MinerList : null;
        if (database == null)
            return indices;

        foreach (CharacterData character in _pool)
        {
            int index = database.FindIndex(c => c != null && c.CharacterName == character.CharacterName);
            if (index >= 0)
                indices.Add(index);
        }
        return indices;
    }

    /// <summary>Recharge la réserve. Les -1 des anciennes sauvegardes (personnage par défaut) sont ignorés.</summary>
    public void Restore(List<int> savedIndices)
    {
        _pool.Clear();
        if (savedIndices == null || minerDatabase == null || minerDatabase.MinerList == null)
            return;

        foreach (int index in savedIndices)
        {
            if (index >= 0 && index < minerDatabase.MinerList.Count && minerDatabase.MinerList[index] != null)
                _pool.Add(minerDatabase.MinerList[index].Clone());
        }
        RefreshItems();
    }

    private void RefreshItems()
    {
        foreach (WorkerAssignmentItem item in _items)
        {
            if (item != null)
                item.Refresh();
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
