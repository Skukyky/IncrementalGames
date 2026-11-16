using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Le caillou à miner. Le joueur le tape (OnClick, branché sur le Button) et les mineurs
/// automatiques le frappent aussi (Hit). Chaque coup rapporte de l'argent et affiche un popup.
/// </summary>
public class ClickTarget : MonoBehaviour
{
    [SerializeField] private CurrencyManager currency;
    [SerializeField] private UpgradeManager upgrades;
    [SerializeField] private MoneyPopupSpawner popups;
    [SerializeField] private SquashEffect squash;

    public void OnClick()
    {
        // Pointer.current couvre souris ET écran tactile (Mouse.current est null sur mobile)
        Vector2 screenPosition = Pointer.current != null
            ? Pointer.current.position.ReadValue()
            : (Vector2)transform.position;
        Hit(upgrades.ClickPower, screenPosition, playSquash: true);
    }

    public void Hit(BigNumber amount, Vector2 screenPosition, bool playSquash)
    {
        // Gain arrondi à la précision affichée (entier sous 1000) : le "+X" du popup est exactement ce qui est ajouté.
        BigNumber gain = amount.FloorToDisplay();
        if (gain.IsZero && amount > BigNumber.Zero)
            gain = BigNumber.One;
        amount = gain;
        currency.Add(amount);

        if (popups != null)
            popups.Show(amount, screenPosition);

        if (playSquash && squash != null)
            squash.Play();
    }
}
