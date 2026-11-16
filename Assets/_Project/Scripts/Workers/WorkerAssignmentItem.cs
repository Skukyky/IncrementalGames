using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ligne de l'onglet "Assignation" : un menu déroulant pour choisir quel personnage
/// travaille sur un mineur automatique.
/// Options : le personnage actuel, puis le personnage par défaut, puis la réserve
/// regroupée par nom ("Thomas x2").
/// </summary>
public class WorkerAssignmentItem : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown workerDropdown;
    [FormerlySerializedAs("AssignName")] [SerializeField] private TMP_Text assignName;

    private AutomaticWorker _worker;
    private WorkerAssignmentManager _manager;
    private readonly List<string> _optionNames = new List<string>();

    public string AssignedName =>
        _worker != null && _worker.ItemData != null && _worker.ItemData.automaticWorkerCharacterData != null
            ? _worker.ItemData.automaticWorkerCharacterData.CharacterName
            : null;

    private void Awake()
    {
        workerDropdown.onValueChanged.AddListener(OnValueChanged);
    }

    public void Initialize(AutomaticWorker worker, WorkerAssignmentManager manager)
    {
        _worker = worker;
        _manager = manager;
        if (_worker == null || _worker.ItemData == null)
            return;

        assignName.text = _worker.ItemData.UpgradeName;
        Refresh();
    }

    public void Refresh()
    {
        if (_manager == null)
            return;

        string current = AssignedName;
        _optionNames.Clear();
        workerDropdown.ClearOptions();

        AddOption(current, current);

        CharacterData fallback = _manager.DefaultCharacter;
        if (fallback != null && fallback.CharacterName != current)
            AddOption(fallback.CharacterName, fallback.CharacterName);

        // Réserve regroupée par nom, dans l'ordre d'obtention
        var counts = new Dictionary<string, int>();
        var order = new List<string>();
        foreach (CharacterData character in _manager.Pool)
        {
            if (!counts.ContainsKey(character.CharacterName))
            {
                counts[character.CharacterName] = 0;
                order.Add(character.CharacterName);
            }
            counts[character.CharacterName]++;
        }
        foreach (string name in order)
            AddOption(name, counts[name] > 1 ? name + " x" + counts[name] : name);

        // L'option 0 est toujours le personnage actuellement assigné.
        workerDropdown.SetValueWithoutNotify(0);
        workerDropdown.RefreshShownValue();
    }

    private void AddOption(string characterName, string label)
    {
        _optionNames.Add(characterName);
        workerDropdown.options.Add(new TMP_Dropdown.OptionData(label ?? "-"));
    }

    private void OnValueChanged(int index)
    {
        if (index <= 0 || index >= _optionNames.Count || _manager == null)
            return;

        _manager.Assign(_worker, _optionNames[index]);
    }
}
