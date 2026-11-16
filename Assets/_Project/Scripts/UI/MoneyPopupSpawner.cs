using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Affiche les popups "+argent" en les recyclant (object pool) : aucun Instantiate/Destroy
/// à chaque gain, donc pas de déchets pour le garbage collector sur mobile.
/// </summary>
public class MoneyPopupSpawner : MonoBehaviour
{
    [SerializeField] private MoneyPopup prefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private int maxPooled = 30;

    private ObjectPool<MoneyPopup> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<MoneyPopup>(
            createFunc: () => Instantiate(prefab, container),
            actionOnGet: popup => popup.gameObject.SetActive(true),
            actionOnRelease: popup => popup.gameObject.SetActive(false),
            actionOnDestroy: popup => Destroy(popup.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: maxPooled);
    }

    public void Show(BigNumber amount, Vector2 screenPosition)
    {
        if (prefab == null || container == null) return;

        MoneyPopup popup = _pool.Get();
        popup.transform.SetAsLastSibling();
        popup.Play("+" + MoneyFormatter.Format(amount), screenPosition, _pool.Release);
    }
}
