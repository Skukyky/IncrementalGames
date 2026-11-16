using UnityEngine;

/// <summary>
/// À placer sur le Canvas racine. Au démarrage, crée un enfant "SafeArea" qui épouse
/// Screen.safeArea (encoche, coins arrondis, barre de navigation) et y déplace tous les
/// enfants existants. Se met à jour si la safe area change (rotation, split-screen...).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform _safeRect;
    private Rect _lastSafeArea;
    private Vector2Int _lastScreenSize;

    private void Awake()
    {
        var go = new GameObject("SafeArea", typeof(RectTransform));
        go.layer = gameObject.layer;
        _safeRect = (RectTransform)go.transform;
        _safeRect.SetParent(transform, false);
        _safeRect.anchorMin = Vector2.zero;
        _safeRect.anchorMax = Vector2.one;
        _safeRect.offsetMin = Vector2.zero;
        _safeRect.offsetMax = Vector2.zero;

        // Déplace les enfants existants (sauf le nouveau) sous la safe area, en gardant l'ordre.
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child == _safeRect) continue;
            child.SetParent(_safeRect, false);
            child.SetAsFirstSibling();
        }

        Apply();
    }

    private void Update()
    {
        if (Screen.safeArea != _lastSafeArea || Screen.width != _lastScreenSize.x || Screen.height != _lastScreenSize.y)
        {
            Apply();
        }
    }

    private void Apply()
    {
        Rect safe = Screen.safeArea;
        _lastSafeArea = safe;
        _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        if (Screen.width <= 0 || Screen.height <= 0) return;

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;
        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;

        _safeRect.anchorMin = min;
        _safeRect.anchorMax = max;
    }
}
