using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Fait apparaître les mineurs automatiques achetés et les supprime au rebirth.</summary>
public class WorkerManager : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private ClickTarget clickTarget;
    [SerializeField] private WorkerAssignmentManager assignments;

    private readonly Dictionary<string, AutomaticWorker> _workers = new Dictionary<string, AutomaticWorker>();

    /// <param name="assignedCharacter">Nom du personnage sauvegardé pour ce mineur (null = celui par défaut de l'amélioration).</param>
    public void Spawn(ItemData item, string assignedCharacter = null)
    {
        if (item == null || container == null || item.automaticWorkerPrefab == null || _workers.ContainsKey(item.UpgradeName))
            return;

        if (!string.IsNullOrEmpty(assignedCharacter) && assignments != null)
        {
            CharacterData saved = assignments.FindCharacter(assignedCharacter);
            if (saved != null)
                item.automaticWorkerCharacterData = saved;
        }

        GameObject instance = Instantiate(item.automaticWorkerPrefab, container);
        if (instance.TryGetComponent(out RectTransform rect))
            rect.anchoredPosition = item.automaticWorkerSpawnPosition;

        if (item.automaticWorkerFlipX)
            Flip(instance);

        AutomaticWorker worker = instance.GetComponentInChildren<AutomaticWorker>(true);
        worker.Initialize(item, clickTarget);
        instance.SetActive(true);

        _workers.Add(item.UpgradeName, worker);
        if (assignments != null)
            assignments.AddWorker(worker);
    }

    public BigNumber EstimateIncomePerSecond()
    {
        BigNumber total = BigNumber.Zero;
        foreach (AutomaticWorker worker in _workers.Values)
        {
            if (worker != null)
                total += worker.EstimateIncomePerSecond();
        }
        return total;
    }

    public void ClearAll()
    {
        foreach (AutomaticWorker worker in _workers.Values)
        {
            if (worker != null)
                Destroy(worker.gameObject);
        }
        _workers.Clear();
    }

    private static void Flip(GameObject instance)
    {
        Image image = instance.GetComponentInChildren<Image>();
        Transform target = image != null ? image.rectTransform : instance.transform;
        Vector3 scale = target.localScale;
        scale.x *= -1f;
        target.localScale = scale;
    }
}
