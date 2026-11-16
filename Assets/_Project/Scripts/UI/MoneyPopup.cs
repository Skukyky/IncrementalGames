using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Texte "+X" qui monte et disparaît. Réutilisé par MoneyPopupSpawner (pool) au lieu d'être détruit.
/// </summary>
public class MoneyPopup : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [FormerlySerializedAs("canvas")] [SerializeField] private Image panel;
    [SerializeField] private float riseDistance = 50f;
    [SerializeField] private float duration = 1f;

    private Color _imageColor;
    private Color _textColor;
    private Action<MoneyPopup> _onFinished;

    private void Awake()
    {
        if (image) _imageColor = image.color;
        if (text) _textColor = text.color;

        // Le popup ne doit jamais intercepter les clics destinés au caillou.
        foreach (Graphic graphic in GetComponentsInChildren<Graphic>(true))
            graphic.raycastTarget = false;
    }

    public void Play(string moneyText, Vector2 screenPosition, Action<MoneyPopup> onFinished)
    {
        _onFinished = onFinished;
        if (text) text.text = moneyText;
        transform.position = screenPosition;
        SetAlpha(1f);
        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * riseDistance;
        Vector2 startAnchored = rectTransform ? rectTransform.anchoredPosition : Vector2.zero;
        Vector2 targetAnchored = startAnchored + Vector2.up * riseDistance;

        float total = Mathf.Max(0.01f, duration);
        float elapsed = 0f;
        while (elapsed < total)
        {
            float t = Mathf.Pow(elapsed / total, 0.4f);
            if (rectTransform)
                rectTransform.anchoredPosition = Vector2.Lerp(startAnchored, targetAnchored, t);
            else
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            SetAlpha(1f - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);
        if (_onFinished != null)
            _onFinished(this);
        else
            Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        if (text)
        {
            Color c = _textColor;
            c.a = alpha;
            text.color = c;
        }

        if (image)
        {
            Color c = _imageColor;
            c.a = alpha;
            image.color = c;
            if (panel) panel.color = c;
        }
    }
}
