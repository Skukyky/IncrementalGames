using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private RectTransform slimeButton; // assign the UI button RectTransform in the inspector
    [SerializeField] private float slimeDuration = 0.6f;
    [SerializeField] private float slimeIntensity = 0.35f; // how strong the stretch/squash is
    [SerializeField] public MoneyConvertAndSave goldConvert;
    [SerializeField] private Transform uiCanvasTransform;
    [SerializeField] private GestionModifyWorkerAssignation gestionModifyWorkerAssignation;
    [SerializeField] private GameObject prefabShowMoney;
    // Expose UI parent for other classes to instantiate UI elements under
    public Transform UiCanvasTransform => uiCanvasTransform;
    
    public double coins;
    private Coroutine _slimeCoroutine;
    private Vector3 _originalScale;
    private string _coinsString;
    Dictionary<string, double> _upgradeClick = new Dictionary<string, double>();
    Dictionary<string, double> _upgradeAutomatic = new Dictionary<string, double>();
    private double _damagePickaxe = 1d;
    private double _damageAutomatic = 1d;
    private bool _firstLaunchDelegate = true;
    
    [SerializeField] private ItemDatabase itemDatabase;

    // Map to keep spawned automatic worker instances keyed by item name
    private Dictionary<string, AnimatedSprite> _spawnedWorkerMap = new Dictionary<string, AnimatedSprite>();
    public IReadOnlyDictionary<string, AnimatedSprite> SpawnedWorkerMap => _spawnedWorkerMap;

    private void Start()
    {
        _originalScale = slimeButton.localScale;
    }

    public void OnClickButton()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        AddCoins(_damagePickaxe, mouseScreenPos);
        PlaySlimeEffect();
    }

    public void AddCoins(double amount, Vector2 startPosition)
    {
        coins += amount;
        CalculateNewMoney();
        ShowMoneyGet(amount, startPosition);
    }

    public void CalculateNewMoney()
    {
        _coinsString = goldConvert != null ? goldConvert.ConvertMoneyLogic(coins) : coins.ToString();
        if (coinsText != null)
        {
            coinsText.text = _coinsString;
        }
    }

    public void PlaySlimeEffect()
    {
        if (slimeButton == null) return;
        if (_slimeCoroutine != null) StopCoroutine(_slimeCoroutine);
        _slimeCoroutine = StartCoroutine(SlimeRoutine());
    }

    private System.Collections.IEnumerator SlimeRoutine()
    {
        float t = 0f;
        float duration = Mathf.Max(0.01f, slimeDuration);
        while (t < duration)
        {
            float p = t / duration; // 0 -> 1
            // damping + oscillation for a slimey rebound
            float damping = Mathf.Exp(-5f * p);
            float oscillation = Mathf.Sin(p * Mathf.PI * 2f * 2f); // two oscillations

            float x = 1f + slimeIntensity * (1f - p) * (1f + 0.6f * oscillation) * damping;
            float y = 1f - slimeIntensity * (1f - p) * (1f + 0.6f * oscillation) * damping;

            slimeButton.localScale = new Vector3(_originalScale.x * x, _originalScale.y * y, _originalScale.z);

            t += Time.deltaTime;
            yield return null;
        }

        slimeButton.localScale = _originalScale;
        _slimeCoroutine = null;
    }

    public void ApplyUpgradePickAxe(string upgradeID, double upgradePower)
    {
        if (_upgradeClick.ContainsKey(upgradeID))
        {
            _upgradeClick[upgradeID] = upgradePower;
        }
        else
        {
            _upgradeClick.Add(upgradeID, upgradePower);
        }

        double newPower = 0d;
        foreach (var pair in _upgradeClick)
        {
            newPower += pair.Value;
        }

        _damagePickaxe = newPower;
    }

    public void ShowMoneyGet(double moneyGet, Vector2 startPosition = default)
    {
        if (prefabShowMoney == null)
        {
            return;
        }

        GameObject moneyObject = Instantiate(prefabShowMoney, uiCanvasTransform);
        ShowMoney showMoney = moneyObject.GetComponent<ShowMoney>();

        if (showMoney != null)
        {
            showMoney.transform.SetAsLastSibling(); // Ensure it appears on top of other UI elements
            showMoney.Initialize(moneyText: "+" + goldConvert.ConvertMoneyLogic(moneyGet), startPosition: startPosition);
        }
    }

    // Spawn a single automatic worker prefab for the given item and store the instance
    public void SpawnAutomaticWorker(ItemData item)
    {
        if (item == null || uiCanvasTransform == null)
        {
            Debug.LogWarning("SpawnAutomaticWorker called with null item");
            return;
        }
        

        if (item.automaticWorkerPrefab == null)
        {
            Debug.LogWarning($"No automaticWorkerPrefab set for item {item.UpgradeName}");
            return;
        }

        if (_spawnedWorkerMap.ContainsKey(item.UpgradeName))
        {
            Debug.Log($"Worker for {item.UpgradeName} already spawned");
            return;
        }

        GameObject instance = GameObject.Instantiate(item.automaticWorkerPrefab, uiCanvasTransform);
        RectTransform rt = instance.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = item.automaticWorkerSpawnPosition;
        }
        
        AnimatedSprite upgradeUI = instance.GetComponent<AnimatedSprite>();
        if (upgradeUI != null)
        {
            upgradeUI.Initialize(item,this);
        }
        
        if (item.automaticWorkerFlipX)
        {
            Image img = instance.GetComponentInChildren<Image>();
            if (img != null)
            {
                Vector3 lsImg = img.rectTransform.localScale;
                lsImg.x *= -1f;
                img.rectTransform.localScale = lsImg;
            }
            else
            {
                // fallback: flip root transform if no Image found
                Vector3 ls = instance.transform.localScale;
                ls.x *= -1f;
                instance.transform.localScale = ls;
            }
        }

        instance.SetActive(true);
        AnimatedSprite animatedSprite = instance.GetComponentInChildren<AnimatedSprite>();
        _spawnedWorkerMap.Add(item.UpgradeName, animatedSprite);
        if (gestionModifyWorkerAssignation != null)
        {
            gestionModifyWorkerAssignation.SpawnNewAssignation(animatedSprite);
        }
        
        

        Debug.Log($"Spawned automatic worker for {item.UpgradeName}");
    }

}
