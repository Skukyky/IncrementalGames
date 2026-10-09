using UnityEngine;

public class SwitchLayer : MonoBehaviour
{
    [SerializeField] private Canvas UpgradeCanvas;
    [SerializeField] private Canvas ShopCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpgradeCanvas.enabled = true;
        ShopCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickUpgrade()
    {
        UpgradeCanvas.enabled = true;
        ShopCanvas.enabled = false;
    }
    
    public void OnClickShop()
    {
        ShopCanvas.enabled = true;
        UpgradeCanvas.enabled = false;
    }
}
