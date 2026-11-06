using System;
using UnityEngine;
using System.Collections.Generic;

public class GestionModifyWorkerAssignation : MonoBehaviour
{
    [SerializeField] private DataMinerList _dataMinerList;
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject prefabAssign;
    [SerializeField] private GameManager gameManager;

    private List<CharacterData> _availableCharacters = new List<CharacterData>();
    private List<AssignNewWorker> spawnedItems = new List<AssignNewWorker>();

    private void Start()
    {
        // Create a copy of the original miner list so we don't modify the asset referenced in the inspector
        if (_dataMinerList != null && _dataMinerList.MinerList != null)
        {
            _availableCharacters = new List<CharacterData>();
            foreach (CharacterData character in _dataMinerList.MinerList)
            {
                if (character != null)
                    _availableCharacters.Add(character.Clone());
            }
        }
        else
            _availableCharacters = new List<CharacterData>();
    }

    public void SpawnNewAssignation(AnimatedSprite animatedSprite)
    {
        GameObject newAssign = Instantiate(prefabAssign, scrollContent);
        if (newAssign != null)
        {
            AssignNewWorker assignNewWorker = newAssign.GetComponentInChildren<AssignNewWorker>();
            spawnedItems.Add(assignNewWorker);
            assignNewWorker.InitializeWorkerDropdown(animatedSprite, this, _availableCharacters);
        }
    }
    
    public void UpdateAssignation(AssignNewWorker worker, CharacterData newCharacterData, CharacterData oldCharacterData)
    {
        if (worker != null && newCharacterData != null &&  oldCharacterData != null)
        {
            int newIndex = 0;
            foreach (CharacterData VARIABLE in _availableCharacters)
            {
                if (VARIABLE.CharacterName.CompareTo(newCharacterData.CharacterName) == 0)
                {
                    break;
                }
                newIndex++;
            }
            if (newIndex < _availableCharacters.Count)
            {
                _availableCharacters[newIndex] = oldCharacterData;
            }
            
            foreach (AssignNewWorker assignNewWorker in spawnedItems)
            {
                assignNewWorker.ReloadDropdownOptions(_availableCharacters);
            }
        }
    }

    public void DestroyAssignation()
    {
        foreach (AssignNewWorker assignNewWorker in spawnedItems)
        {
            Destroy(assignNewWorker.gameObject);
        }
        spawnedItems.Clear();
    }
}