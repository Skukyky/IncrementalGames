using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    [SerializeField] private GachaAndRebirth gachaAndRebirth;
    [SerializeField] public AnimationCurve priceCurve;
    [SerializeField] public AnimationCurve powerCurve;
    public Transform UiCanvasTransform => uiCanvasTransform;
    private GestionSpawnUpgrade _gestionSpawnUpgrade;
    
    public double2 Coins =  new double2(0, 0);
    private Coroutine _slimeCoroutine;
    private Vector3 _originalScale;
    private string _coinsString;
    Dictionary<string, double2> _upgradeClick = new Dictionary<string, double2>();
    Dictionary<string, double2> _upgradeAutomatic = new Dictionary<string, double2>();
    private double2 _damagePickaxe = new double2(1d, 0);
    private double2 _damageAutomatic = new double2(1d, 0);
    private bool _firstLaunchDelegate = true;
    private UpgradeItemUI _PickaxeUpgrade;

    public int _numberOfRebirth = 1;
    
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
        PlaySlimeEffect();
        AddCoins(_damagePickaxe, mouseScreenPos);
    }

    public void AddCoins(double2 amount, Vector2 startPosition)
    {
        amount.x = Math.Round(amount.x, 3);
        CalculNewMoney(amount);
        CalculateNewMoney();
        ShowMoneyGet(amount, startPosition);
    }

    public void CalculateNewMoney()
    {
        if (goldConvert)
        {
            _coinsString = goldConvert.MoneyConvert(Coins);
        }
        if (coinsText)
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

    public void ApplyUpgradePickAxe(string upgradeID, double2 upgradePower, UpgradeItemUI pickaxeUpgrade)
    {
        if (!_PickaxeUpgrade)
        {
            _PickaxeUpgrade = pickaxeUpgrade;
        }
        if (_upgradeClick.ContainsKey(upgradeID))
        {
            _upgradeClick[upgradeID] = upgradePower;
        }
        else
        {
            _upgradeClick.Add(upgradeID, upgradePower);
        }

        double2 newPower = 0d;
        foreach (var pair in _upgradeClick)
        {
            newPower += pair.Value;
        }

        _damagePickaxe = newPower;
    }

    public void ShowMoneyGet(double2 moneyGet = default, Vector2 startPosition = default)
    {
        if (!prefabShowMoney)
        {
            return;
        }

        GameObject moneyObject = Instantiate(prefabShowMoney, uiCanvasTransform);
        ShowMoney showMoney = moneyObject.GetComponent<ShowMoney>();

        if (showMoney)
        {
            showMoney.transform.SetAsLastSibling(); // Ensure it appears on top of other UI elements
            showMoney.Initialize(moneyText: "+" + goldConvert.MoneyConvert(moneyGet), startPosition: startPosition);
        }
    }

    // Spawn a single automatic worker prefab for the given item and store the instance
    public void SpawnAutomaticWorker(ItemData item)
    {
        if (item == null || uiCanvasTransform == null || _spawnedWorkerMap.ContainsKey(item.UpgradeName) || item.automaticWorkerPrefab == null)
        {
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
    }

    public void CalculNewMoney(double2 changedMoney, bool isAdd = true)
    {
        double2 HighterValue = 0;
        double2 LowerValue = 0;
        switch (Coins.y.CompareTo(changedMoney.y))
        {
            case 1: // Coins.y > changedMoney.y
                HighterValue = Coins;
                LowerValue = changedMoney;
                break;
            case -1: // Coins.y < changedMoney.y
                HighterValue = changedMoney;
                LowerValue = Coins;
                break;
            case 0: // Coins.y == changedMoney.y
                if (Coins.x >= changedMoney.x)
                {
                    HighterValue = Coins;
                    LowerValue = changedMoney;
                }
                else
                {
                    HighterValue = changedMoney;
                    LowerValue = Coins;
                }
                break;
        }

        if (Math.Abs(HighterValue.y - LowerValue.y) > 1)
        {
            Coins = HighterValue;
            return;
        }
        if (isAdd)
        {
            HighterValue.x += LowerValue.x / Math.Pow(10,3*Math.Abs(HighterValue.y - LowerValue.y));
            if (HighterValue.x >= 1000)
            {
                HighterValue.x /= 1000;
                HighterValue.y += 1;
            }
        }
        else
        {
            HighterValue.x -= LowerValue.x / Math.Pow(10,3*Math.Abs(HighterValue.y - LowerValue.y));
            if (HighterValue.x < 1 && HighterValue.y > 0)
            {
                HighterValue.x *= 1000;
                HighterValue.y -= 1;
            }
        }
        Coins = HighterValue;
        return;
    }
    
    public bool VerifyCanPurchase(double2 price)
    {
        bool canPurchase = false;
        switch (Coins.y.CompareTo(price.y))
        {
            case 1: // Coins.y > price.y
                canPurchase = true;
                break;
            case -1: // Coins.y < price.y
                canPurchase = false;
                break;
            case 0: // Coins.y == price.y
                if (Coins.x >= price.x)
                {
                    canPurchase = true;
                }
                else
                {
                    canPurchase = false;
                }
                break;
        }
        return canPurchase;
    }
    
    public void Rebirth()
    {
        if (!VerifyCanPurchase(gachaAndRebirth.price))
        {
            print("not enough money");
            return;
        }
        Coins = new double2(0, 0);
        _numberOfRebirth++;
        gachaAndRebirth.CalculateNewPriceRebirth(this);
        gestionModifyWorkerAssignation.DestroyAssignation();
        if (_PickaxeUpgrade)
        {
            _PickaxeUpgrade.Reset();
            _upgradeClick.Clear();
        }

        if (_gestionSpawnUpgrade)
        {
            _gestionSpawnUpgrade.ClearSpawnedItems();
        }
        CalculateNewMoney();

        foreach (AnimatedSprite sprite in _spawnedWorkerMap.Values)
        {
            Destroy(sprite.gameObject);
        }
        _spawnedWorkerMap.Clear();
    }

    public void GetGestionSpawn(GestionSpawnUpgrade gestionSpawnUpgrade)
    {
        _gestionSpawnUpgrade = gestionSpawnUpgrade;
    }
  

}
