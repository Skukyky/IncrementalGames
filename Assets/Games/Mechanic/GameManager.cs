using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private RectTransform slimeButton; // assign the UI button RectTransform in the inspector
    [SerializeField] private float slimeDuration = 0.6f;
    [SerializeField] private float slimeIntensity = 0.35f; // how strong the stretch/squash is
    [SerializeField] private MoneyConvertAndSave goldConvert;
    
    
    public int coins; 
    private Coroutine _slimeCoroutine;
    private Vector3 _originalScale;
    private string _coinsString;
    Dictionary<string, double> _upgradeClick = new Dictionary<string, double>();
    Dictionary<string, double> _upgradeAutomatic = new Dictionary<string, double>();
    private int _damagePickaxe = 1;
    
    private void Start()
    {
        _originalScale = slimeButton.localScale;
    }

    public void OnClickButton()
    {
        coins += _damagePickaxe;
        CalculateNewMoney();
        PlaySlimeEffect();
    }

    public void CalculateNewMoney()
    {
        _coinsString = goldConvert.ConvertMoneyLogic(coins);
        coinsText.text = _coinsString;
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
        double newPower = 0;
        foreach (var pair in _upgradeClick)
        {
            newPower += pair.Value;
        }
        _damagePickaxe = (int)newPower;
    }
    
}
