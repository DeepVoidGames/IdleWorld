using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    private string saveFilePath;

    private float _timer = 0f;

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
        LoadGameContent();
        Load();
        // After loading the game
        UISystem.Instance.LoadUI();
        InventorySystem.Instance.UpdateUI();
        MiningSystem.Instance.SpawnRock();
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
            // Weapon
            weaponData = DamageSystem.Instance.GetWeapon(),
            isWeaponEquippedData = DamageSystem.Instance.IsWeaponEquipped,
            weaponButtonData = InventorySystem.Instance.equipedWeaponButton,
            // Mining System
            miningLevelData = MiningSystem.Instance.MiningLevel,
            miningEfficiencyData = MiningSystem.Instance.MiningEfficiency,
            miningExperienceData = MiningSystem.Instance.MiningExperience
        };

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);

        InventoryData inventoryData = new InventoryData
        {
            inventoryData = InventorySystem.Instance.inventory
        };
        json = JsonUtility.ToJson(inventoryData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "inventory.json"), json);
        // Debug.Log("Game saved to " + saveFilePath);
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
            
            // Load weapon
            if (gameData.weaponData != null)
            {
                DamageSystem.Instance.EquipWeapon(gameData.weaponData);
            }

            if (gameData.isWeaponEquippedData)
            {
                DamageSystem.Instance.IsWeaponEquipped = gameData.isWeaponEquippedData;
            }

            if (gameData.weaponButtonData != null)
            {
                InventorySystem.Instance.equipedWeaponButton = gameData.weaponButtonData;
            }

            // Load mining level
            if (gameData.miningLevelData != 0)
            {
                MiningSystem.Instance.SetMiningLevel(gameData.miningLevelData);
            }

            // Load mining efficiency
            if (gameData.miningEfficiencyData != 0)
            {
                MiningSystem.Instance.SetMiningEfficiency(gameData.miningEfficiencyData);
            }
            
            // Load mining experience
            if (gameData.miningExperienceData != 0)
            {
                MiningSystem.Instance.SetMiningExperience(gameData.miningExperienceData);
            }

            Debug.Log("Game loaded from " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + saveFilePath);
        }

        if (File.Exists(Path.Combine(Application.persistentDataPath, "inventory.json")))
        {   
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "inventory.json"));
            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);
            if (inventoryData.inventoryData != null)
            {
                foreach (InventorySlot slot in inventoryData.inventoryData.inventory)
                {
                    Items item = ItemSystem.Instance.ItemsCollection.Find(x => x.id == slot.item.id);
                    if (item == null)
                    {
                        Debug.LogWarning("Item not found: " + slot.item.id);
                        continue;
                    }
                    slot.item.Name = item.Name;
                    slot.item.id = item.id;
                    slot.item.icon = item.icon;
                    slot.item.category = item.category;
                    slot.item.rarity = item.rarity;

                    // Weapon
                    slot.item.damage = item.damage;
                    slot.item.damageBoostPercentage = item.damageBoostPercentage;
                }
                InventorySystem.Instance.inventory = inventoryData.inventoryData;
            }

        }
        else
        {
            Debug.LogWarning("Inventory file not found: " + Path.Combine(Application.persistentDataPath, "inventory.json"));
        }
    
    }

    public void LoadGameContent()
    {
        // Load items
        TextAsset itemsJsonFile = Resources.Load<TextAsset>("GameData/items");
        ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(itemsJsonFile.text);
        List<Items> items = itemDataWrapper.items;
        ItemSystem.Instance.SetItemsCollection(items);

        // Load crafting recipes
        TextAsset craftingRecipesJsonFile = Resources.Load<TextAsset>("GameData/craftingRecipes");
        CraftingRecipesWrapper craftingRecipesWrapper = JsonUtility.FromJson<CraftingRecipesWrapper>(craftingRecipesJsonFile.text);
        List<CraftingRecipe> craftingRecipes = craftingRecipesWrapper.craftingRecipes;
        CraftingSystem.Instance.SetCraftingRecipes(craftingRecipes);
    }

    private void FixedUpdate() {
        _timer += Time.fixedDeltaTime;
        if (_timer >= 5f)
        {
            Save();
            _timer = 0f;
        }
    }
    private void OnApplicationQuit()
    {
        Save();
    }
}

[System.Serializable]
public class GameData
{
    // Gold System
    public float goldData;
    // Monster Kiling System
    public int levelData;
    public int stageData;
    public string currentBiome;
    // Weapon
    public Items weaponData;
    public bool isWeaponEquippedData;
    public UnityEngine.UI.Button weaponButtonData;
    // Mining System
    public float miningLevelData;
    public float miningEfficiencyData;
    public float miningExperienceData;
}

[System.Serializable]
public class InventoryData
{
    public Inventory inventoryData;
}
