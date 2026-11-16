using System.Collections;
using UnityEngine;

/// <summary>Effet "slime" : écrase et étire l'élément puis le fait rebondir.</summary>
[RequireComponent(typeof(RectTransform))]
public class SquashEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private float intensity = 0.35f;

    private RectTransform _rect;
    private Vector3 _originalScale;
    private Coroutine _routine;

    private void Awake()
    {
        _rect = (RectTransform)transform;
        _originalScale = _rect.localScale;
    }

    public void Play()
    {
        if (!isActiveAndEnabled) return;
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        float t = 0f;
        float total = Mathf.Max(0.01f, duration);
        while (t < total)
        {
            float p = t / total;
            float damping = Mathf.Exp(-5f * p);
            float oscillation = Mathf.Sin(p * Mathf.PI * 4f); // deux oscillations
            float stretch = intensity * (1f - p) * (1f + 0.6f * oscillation) * damping;

            _rect.localScale = new Vector3(_originalScale.x * (1f + stretch), _originalScale.y * (1f - stretch), _originalScale.z);

            t += Time.deltaTime;
            yield return null;
        }

        _rect.localScale = _originalScale;
        _routine = null;
    }
}
