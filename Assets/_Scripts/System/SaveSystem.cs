using System.IO;
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

    private void Start()
    {
        Load();
        UISystem.Instance.LoadUI();
    }

    public void Save()
    {
        GameData gameData = new GameData
        {
            // Gold System
            goldData = GoldSystem.Instance.Gold,
            // Monster Kiling System
            levelData = LevelSystem.Instance.Level,
            stageData = LevelSystem.Instance.Stage,
            currentBiome = BiomeSystem.Instance.CurrentBiome,
            // Inventory System
            inventoryData = InventorySystem.Instance.inventory,
            // Mining System
            miningLevelData = MiningSystem.Instance.miningLevel,
            miningEfficiencyData = MiningSystem.Instance.miningEfficiency
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

            // Load gold data
            if (gameData.goldData != 0)
            {
                GoldSystem.Instance.SetGold(gameData.goldData);
            }

            // Load level
            if (gameData.levelData != 0)
            {
                LevelSystem.Instance.SetLevel(gameData.levelData);
            }

            // Load stage
            if (gameData.stageData != 0)
            {
                LevelSystem.Instance.SetStage(gameData.stageData);
            }

            // Load current biome
            if (gameData.currentBiome != null)
            {
                BiomeSystem.Instance.CurrentBiome = gameData.currentBiome;
            }
            
            // Load inventory
            if (gameData.inventoryData != null)
            {
                InventorySystem.Instance.inventory = gameData.inventoryData;
            }

            // Load mining level
            if (gameData.miningLevelData != 0)
            {
                MiningSystem.Instance.miningLevel = gameData.miningLevelData;
            }

            // Load mining efficiency
            if (gameData.miningEfficiencyData != 0)
            {
                MiningSystem.Instance.miningEfficiency = gameData.miningEfficiencyData;
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
    public float goldData;
    public int levelData;
    public int stageData;
    public string currentBiome;
    public Inventory inventoryData;
    public float miningLevelData;
    public float miningEfficiencyData;
}
