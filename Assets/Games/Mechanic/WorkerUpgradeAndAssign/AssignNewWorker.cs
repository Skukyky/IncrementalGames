using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssignNewWorker : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown workerDropdown;
    [SerializeField] private TMP_Text AssignName;
    [SerializeField] private CharacterData defaultCharacterData;
    
    private List<CharacterData> dataMinerList = new List<CharacterData>();
    private GameManager gameManager;
    private AnimatedSprite _selectedWorker;
    private GestionModifyWorkerAssignation _workerAssignation;

    void Start()
    {
        workerDropdown.onValueChanged.AddListener(OnValueChanged);
    }
    public void InitializeWorkerDropdown(AnimatedSprite WorkerRef, GestionModifyWorkerAssignation gestionModifyWorkerAssignation, List<CharacterData> minerList)
    {
        _selectedWorker = WorkerRef;
        dataMinerList = minerList ?? new List<CharacterData>();
        _workerAssignation = gestionModifyWorkerAssignation;
        if (_selectedWorker == null || _selectedWorker.itemDataLocal == null)
            return;

        AssignName.text = _selectedWorker.itemDataLocal.UpgradeName;

        workerDropdown.ClearOptions();

        if (_selectedWorker.itemDataLocal.automaticWorkerCharacterData != null)
        {
            workerDropdown.options.Add(new TMP_Dropdown.OptionData(_selectedWorker.itemDataLocal.automaticWorkerCharacterData.CharacterName));
        }

        foreach (CharacterData VARIABLE in dataMinerList)
        {
            if (VARIABLE == null)
                continue;

            workerDropdown.options.Add(new TMP_Dropdown.OptionData(VARIABLE.CharacterName));
        }

        CharacterData currentAssigned = _selectedWorker.itemDataLocal.automaticWorkerCharacterData;
        if (currentAssigned != null)
        {
            _selectedWorker.ModifyCharacterData(currentAssigned);
        }

        workerDropdown.SetValueWithoutNotify(0);
        workerDropdown.RefreshShownValue();
    }
    
    public void OnValueChanged(int newValue)
    {
        if (_selectedWorker == null || _selectedWorker.itemDataLocal == null || newValue <= 0 || newValue > dataMinerList.Count)
            return;

        CharacterData OldCharacter = _selectedWorker.itemDataLocal.automaticWorkerCharacterData;
        CharacterData NewCharacter = dataMinerList[newValue - 1];
        if (NewCharacter == null)
            return;

        _selectedWorker.ModifyCharacterData(NewCharacter);
        _workerAssignation?.UpdateAssignation(this, NewCharacter, OldCharacter);
    }

    public void ReloadDropdownOptions(List<CharacterData> minerList)
    {
        CharacterData currentAssigned = _selectedWorker != null && _selectedWorker.itemDataLocal != null ? _selectedWorker.itemDataLocal.automaticWorkerCharacterData : null;

        dataMinerList = minerList ?? new List<CharacterData>();
        workerDropdown.ClearOptions();

        if (currentAssigned != null)
        {
            workerDropdown.options.Add(new TMP_Dropdown.OptionData(currentAssigned.CharacterName));
        }

        foreach (CharacterData VARIABLE in dataMinerList)
        {
            if (VARIABLE == null)
                continue;

            workerDropdown.options.Add(new TMP_Dropdown.OptionData(VARIABLE.CharacterName));
        }

        int newIndex = currentAssigned != null ? dataMinerList.IndexOf(currentAssigned) : -1;
        if (newIndex >= 0)
        {
            workerDropdown.SetValueWithoutNotify(newIndex);
        }
        
        workerDropdown.RefreshShownValue();
    }

}
