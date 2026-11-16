using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ligne de l'onglet "Assignation" : un menu déroulant pour choisir quel personnage
/// travaille sur un mineur automatique.
/// </summary>
public class WorkerAssignmentItem : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown workerDropdown;
    [FormerlySerializedAs("AssignName")] [SerializeField] private TMP_Text assignName;

    private List<CharacterData> _available = new List<CharacterData>();
    private AutomaticWorker _worker;
    private WorkerAssignmentManager _manager;

    private void Awake()
    {
        workerDropdown.onValueChanged.AddListener(OnValueChanged);
    }

    public void Initialize(AutomaticWorker worker, WorkerAssignmentManager manager, List<CharacterData> available)
    {
        _worker = worker;
        _manager = manager;
        _available = available ?? new List<CharacterData>();
        if (_worker == null || _worker.ItemData == null)
            return;

        assignName.text = _worker.ItemData.UpgradeName;

        CharacterData current = _worker.ItemData.automaticWorkerCharacterData;
        if (current != null)
            _worker.ModifyCharacterData(current);

        FillOptions(current);
        workerDropdown.SetValueWithoutNotify(0);
        workerDropdown.RefreshShownValue();
    }

    public void ReloadOptions(List<CharacterData> available)
    {
        CharacterData current = _worker != null && _worker.ItemData != null ? _worker.ItemData.automaticWorkerCharacterData : null;
        _available = available ?? new List<CharacterData>();
        FillOptions(current);

        // L'option 0 est toujours le personnage actuellement assigné.
        workerDropdown.SetValueWithoutNotify(0);
        workerDropdown.RefreshShownValue();
    }

    private void FillOptions(CharacterData current)
    {
        workerDropdown.ClearOptions();
        if (current != null)
            workerDropdown.options.Add(new TMP_Dropdown.OptionData(current.CharacterName));

        foreach (CharacterData character in _available)
        {
            if (character != null)
                workerDropdown.options.Add(new TMP_Dropdown.OptionData(character.CharacterName));
        }
    }

    private void OnValueChanged(int index)
    {
        if (_worker == null || _worker.ItemData == null || index <= 0 || index > _available.Count)
            return;

        CharacterData oldCharacter = _worker.ItemData.automaticWorkerCharacterData;
        CharacterData newCharacter = _available[index - 1];
        if (newCharacter == null)
            return;

        _worker.ModifyCharacterData(newCharacter);
        if (_manager != null)
            _manager.SwapAssignment(newCharacter, oldCharacter);
    }
}
