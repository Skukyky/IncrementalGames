using TMPro;
using UnityEngine;

/// <summary>Affiche le nombre de gemmes et se met à jour tout seul.</summary>
public class GemsDisplay : MonoBehaviour
{
    [SerializeField] private GachaManager gacha;
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        if (gacha == null) return;
        gacha.GemsChanged += Refresh;
        Refresh(gacha.Gems);
    }

    private void OnDisable()
    {
        if (gacha != null)
            gacha.GemsChanged -= Refresh;
    }

    private void Refresh(int gems)
    {
        if (text != null)
            text.text = gems.ToString();
    }
}
