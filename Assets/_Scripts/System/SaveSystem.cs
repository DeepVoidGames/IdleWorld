using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    private string saveFilePath;
    
    private void Awake()
    {
         if (Instance == null)
        {
            Instance = this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    void Start()
    {
        Load();
    }

    public void Save()
    {
        GameData gameData = new GameData
        {
            inventoryData = InventorySystem.Instance.items.Select(kv => kv.Value).ToArray(),
            minersData = MinerSystem.Instance.minersDict.Select(kv => kv.Value).ToArray(),
            moneyData = MoneySystem.Instance.Money
        };

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved to " + saveFilePath);
    }

    public void Load()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            // Load inventory data
            if (gameData.inventoryData != null)
            {
                foreach (InventoryItem item in gameData.inventoryData)
                {
                    InventorySystem.Instance.items[item.itemName] = item;
                }
            }

            // Load miners data
            if (gameData.minersData != null)
            {
                foreach (Miner miner in gameData.minersData)
                {
                    MinerSystem.Instance.minersDict[miner.minerName] = miner;
                }
            }

            // Load money data
            if (gameData.moneyData != 0)
            {
                MoneySystem.Instance.SetMoney(gameData.moneyData);
            }
            Debug.Log("Game loaded from " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + saveFilePath);
        }
    
    }
}

[System.Serializable]
public class GameData
{
    public InventoryItem[] inventoryData;
    public Miner[] minersData;
    public float moneyData;
}
