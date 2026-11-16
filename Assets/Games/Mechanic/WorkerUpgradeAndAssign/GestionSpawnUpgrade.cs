using UnityEngine;
using System.Collections.Generic;

public class GestionSpawnUpgrade : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject prefabUpgrade;
    [SerializeField] private GameManager gameManager;

    private int IndexAllReadySpawnedItems = 0;
    private List<GameObject> _spawnedItems = new List<GameObject>();

    public List<GameObject> SpawnedItems
    {
        get => _spawnedItems;
        set => _spawnedItems = value;
    }

    private void Start()
    {
        if (gameManager)
        {
            gameManager.GetGestionSpawn(this);
        }
        NewSpawn();
    }

    public void SpawnUpgrades()
    {
        if (SpawnedItems != null)
        {
            ClearSpawnedItems();
        }
        
        foreach (ItemData item in itemDatabase.items)
        {
            GameObject newUpgrade = Instantiate(prefabUpgrade, scrollContent);
            SpawnedItems.Add(newUpgrade);
            
            // Récupérer le composant UI pour le configurer avec les données de l'item
            UpgradeItemUI upgradeUI = newUpgrade.GetComponent<UpgradeItemUI>();
            if (upgradeUI != null)
            {
                upgradeUI.Initialize(item,gameManager, this);
            }
        }
    }

    public void SetItemData(ItemDatabase item)
    {
        itemDatabase = item;
    }

    public void ClearSpawnedItems()
    {
        foreach (GameObject item in SpawnedItems)
        {
            Destroy(item);
        }
        SpawnedItems.Clear();
        IndexAllReadySpawnedItems = 0;
        NewSpawn();
    }

    public void NewSpawn()
    {
        if (IndexAllReadySpawnedItems < itemDatabase.items.Count)
        {
            
            ItemData item = itemDatabase.items[IndexAllReadySpawnedItems];
            GameObject newUpgrade = Instantiate(prefabUpgrade, scrollContent);
            SpawnedItems.Add(newUpgrade);
            UpgradeItemUI upgradeUI = newUpgrade.GetComponent<UpgradeItemUI>();
            if (upgradeUI != null)
            {
                upgradeUI.Initialize(item,gameManager, this);
            }
            IndexAllReadySpawnedItems++;
        }
    }
}
