using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowMoney : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;

    public void Initialize(float riseDistance = 30f, float duration = 0.5f, string moneyText = "+1", Vector2 startPosition = default)
    {
        if (text != null)
        {
            text.text = moneyText;
        }
        this.transform.position = startPosition ;

        StartCoroutine(DespawnRoutine(riseDistance, duration));
        
    }


    private System.Collections.IEnumerator DespawnRoutine(float riseDistance, float duration)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * riseDistance;

        Vector2 startAnchoredPosition = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;
        Vector2 targetAnchoredPosition = startAnchoredPosition + Vector2.up * riseDistance;


        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startAnchoredPosition, targetAnchoredPosition, t);
            }
            else
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            }

            if (text != null)
            {
                Color color = text.color;
                color.a = Mathf.Lerp(1f, 0f, t);
                text.color = color;
            }
            
            if (image != null)
            {
                Color color = image.color;
                color.a = Mathf.Lerp(1f, 0f, t);
                image.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = targetAnchoredPosition;
        }
        else
        {
            transform.position = targetPosition;
        }

        if (text != null)
        {
            Color color = text.color;
            color.a = 0f;
            text.color = color;
        }
        
        if (image != null)
        {
            Color color = image.color;
            color.a = 0f;
            image.color = color;
        }

        Destroy(gameObject);
    }
}
