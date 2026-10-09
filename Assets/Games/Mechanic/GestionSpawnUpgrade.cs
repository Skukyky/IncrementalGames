using UnityEngine;
using System.Collections.Generic;

public class GestionSpawnUpgrade : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject prefabUpgrade;
    [SerializeField] private GameManager gameManager;

    
    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Start()
    {
        if (itemDatabase == null || scrollContent == null || prefabUpgrade == null)
        {
            Debug.LogError("GestionSpawnUpgrade: Veuillez assigner itemDatabase, scrollContent et prefabUpgrade dans l'inspecteur");
            return;
        }
        
        SpawnUpgrades();
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
            IUpgradeUI upgradeUI = newUpgrade.GetComponent<IUpgradeUI>();
            if (upgradeUI != null)
            {
                upgradeUI.Initialize(item,gameManager);
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

    public void RefreshSpawns()
    {
        SpawnUpgrades();
    }
}
