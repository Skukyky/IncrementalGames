using UnityEngine;
using System.Collections.Generic;

public class GestionSpawnUpgrade : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject prefabUpgrade;
    [SerializeField] private GameManager gameManager;

    private int IndexAllReadySpawnedItems = 0;
    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Start()
    {
       
        
    }

    public void SpawnUpgrades()
    {
        if (spawnedItems != null)
        {
            ClearSpawnedItems();
        }
        
        foreach (ItemData item in itemDatabase.items)
        {
            GameObject newUpgrade = Instantiate(prefabUpgrade, scrollContent);
            spawnedItems.Add(newUpgrade);
            
            // Récupérer le composant UI pour le configurer avec les données de l'item
            UpgradeItemUI upgradeUI = newUpgrade.GetComponent<UpgradeItemUI>();
            if (upgradeUI != null)
            {
                upgradeUI.Initialize(item,gameManager, this);
            }
        }
    }

    private void ClearSpawnedItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();
    }

    public void NewSpawn()
    {
        if (IndexAllReadySpawnedItems < itemDatabase.items.Count)
        {
            
            ItemData item = itemDatabase.items[IndexAllReadySpawnedItems];
            GameObject newUpgrade = Instantiate(prefabUpgrade, scrollContent);
            spawnedItems.Add(newUpgrade);
            UpgradeItemUI upgradeUI = newUpgrade.GetComponent<UpgradeItemUI>();
            if (upgradeUI != null)
            {
                upgradeUI.Initialize(item,gameManager, this);
            }
            IndexAllReadySpawnedItems++;
        }
    }
}
