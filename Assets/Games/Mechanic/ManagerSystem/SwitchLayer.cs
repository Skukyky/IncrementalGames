using UnityEngine;

public class SwitchLayer : MonoBehaviour
{
    [SerializeField] private Canvas UpgradeCanvas;
    [SerializeField] private Canvas ShopCanvas;
    [SerializeField] private Canvas GachaCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpgradeCanvas.enabled = true;
        ShopCanvas.enabled = false;
        GachaCanvas.enabled = false;
    }

    public void OnClickUpgrade()
    {
        UpgradeCanvas.enabled = true;
        ShopCanvas.enabled = false;
        GachaCanvas.enabled = false;
    }
    
    public void OnClickShop()
    {
        ShopCanvas.enabled = true;
        UpgradeCanvas.enabled = false;
        GachaCanvas.enabled = false;
    }

    public void OnClickGacha()
    {
        GachaCanvas.enabled = true;
        UpgradeCanvas.enabled = false;
        ShopCanvas.enabled = false;
    }
}
