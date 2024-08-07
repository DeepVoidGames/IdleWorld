using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem _instance;
    public static SaveSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SaveSystem");
                    _instance = go.AddComponent<SaveSystem>();
                }
            }
            return _instance;
        }
    }
    private string saveFilePath;

    private float _timer = 0f;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    private void Start()
    {   
        Load();
        // After loading the game
        UISystem.Instance.LoadUI();
    }

    public void Save()
    {
        // Save game data
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
            miningExperienceData = MiningSystem.Instance.MiningExperience,

            toolData = MiningSystem.Instance.GetTool(),
            isToolEquippedData = MiningSystem.Instance.IsToolEquipped,
            toolButtonData = InventorySystem.Instance.equipedToolButton,
            currentCave = CaveSystem.Instance.CurrentCave != null ? CaveSystem.Instance.CurrentCave : "Stonecrest Quarry"
        };
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);

        // Save inventory data
        InventoryData inventoryData = new InventoryData
        {
            inventoryData = InventorySystem.Instance.inventory
        };
        json = JsonUtility.ToJson(inventoryData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "inventory.json"), json);
        // Debug.Log("Game saved to " + saveFilePath);

        // Save hero data
        HeroData heroData = new HeroData
        {
            heroes = TavernSystem.Instance.heroes
        };
        json = JsonUtility.ToJson(heroData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "heroes.json"), json);
    
        // Save caves data
        CavesData cavesData = new CavesData
        {
            caves = CaveSystem.Instance.caves
        };
        json = JsonUtility.ToJson(cavesData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "caves.json"), json);

        // Save upgrades
        Upgrades upgrades = new Upgrades
        {
            upgrades = UpgradingSystem.Instance.Upgrades
        };
        json = JsonUtility.ToJson(upgrades, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "upgrades.json"), json);
    }

    public void Load()
    {   
        // Load game data
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
                ContentLocker.Instance.CheckContent(gameData.levelData);
            }

            // Load stage
            if (gameData.stageData != 0)
            {
                LevelSystem.Instance.SetStage(gameData.stageData);
            }

            // Load current biome
            if (gameData.currentBiome != null)
            {
                BiomeSystem.Instance.SetCurrentBiome(gameData.currentBiome);
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

            if (gameData.toolData != null)
            {
                MiningSystem.Instance.EquipTool(gameData.toolData);
            }

            if (gameData.isToolEquippedData)
            {
                MiningSystem.Instance.IsToolEquipped = gameData.isToolEquippedData;
            }

            if (gameData.toolButtonData != null)
            {
                InventorySystem.Instance.equipedToolButton = gameData.toolButtonData;
            }

            if (gameData.currentCave != null)
            {
                CaveSystem.Instance.CurrentCave = gameData.currentCave;
            }

            Debug.Log("Game loaded from " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + saveFilePath);
        }

        // Load inventory data
        if (File.Exists(Path.Combine(Application.persistentDataPath, "inventory.json")))
        {   
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "inventory.json"));
            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);
            if (inventoryData.inventoryData != null)
            {
                InventorySystem.Instance.SetInventory(inventoryData.inventoryData);
            }
        }
        else
        {
            Debug.LogWarning("Inventory file not found: " + Path.Combine(Application.persistentDataPath, "inventory.json"));
        }
    
        // Load hero data
        if (File.Exists(Path.Combine(Application.persistentDataPath, "heroes.json")))
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "heroes.json"));
            HeroData heroData = JsonUtility.FromJson<HeroData>(json);
            if (heroData.heroes != null)
            {
                for (int i = 0; i < heroData.heroes.Count; i++)
                {
                    Hero hero = TavernSystem.Instance.heroes.Find(x => x.name == heroData.heroes[i].name);
                    if (hero == null)
                    {
                        Debug.LogWarning("Hero not found: " + heroData.heroes[i].name);
                        continue;
                    }
                    hero.level = heroData.heroes[i].level;
                    hero.isUnlocked = heroData.heroes[i].isUnlocked;

                    if (hero.isUnlocked)
                    {
                        TavernSystem.Instance.SpawnHero(i);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Heroes file not found: " + Path.Combine(Application.persistentDataPath, "heroes.json"));
        }

        // Load Upgrades
        if (File.Exists(Path.Combine(Application.persistentDataPath, "upgrades.json")))
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "upgrades.json"));
            Upgrades upgrades = JsonUtility.FromJson<Upgrades>(json);
            if (upgrades.upgrades != null)
            {
                UpgradingSystem.Instance.SetUpgrades(upgrades.upgrades);
            }
        }
        else
        {
            Debug.LogWarning("Upgrades file not found: " + Path.Combine(Application.persistentDataPath, "upgrades.json"));
        }
    }
    
    public void LoadCaves()
    {
        // Load caves data
        if (File.Exists(Path.Combine(Application.persistentDataPath, "caves.json")))
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "caves.json"));
            CavesData cavesData = JsonUtility.FromJson<CavesData>(json);
            if (cavesData.caves != null)
            {
                foreach (Cave cave in cavesData.caves)
                {
                    Cave c = CaveSystem.Instance.caves.Find(x => x.name == cave.name);
                    if (c == null)
                    {
                        Debug.LogWarning("Cave not found: " + cave.name);
                        continue;
                    }
                    CaveSystem.Instance.LoadCave(cave.name, cave.isUnlocked);
                }
            }
        }
        else
        {
            Debug.LogWarning("Caves file not found: " + Path.Combine(Application.persistentDataPath, "caves.json"));
        }
    }

    private void FixedUpdate() {
        // Auto save every 5 minutes
        _timer += Time.deltaTime;
        if (_timer >= 300f)
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
    public double goldData;
    // Monster Kiling System
    public int levelData;
    public int stageData;
    public string currentBiome;
    // Weapon
    public Items weaponData;
    public bool isWeaponEquippedData;
    public UnityEngine.UI.Button weaponButtonData;
    // Mining System
    public double miningLevelData;
    public double miningEfficiencyData;
    public double miningExperienceData;
    public Items toolData;
    public bool isToolEquippedData;
    public UnityEngine.UI.Button toolButtonData;
    public string currentCave;
}

public class InventoryData
{
    public Inventory inventoryData;
}

public class HeroData
{
    public List<Hero> heroes;
}

public class CavesData
{
    public List<Cave> caves;
}

public class Upgrades
{
    public List<Upgrade> upgrades;
}

public class LoadGameData : MonoBehaviour
{
    private static LoadGameData _instance;
    public static LoadGameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LoadGameData>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("LoadGameData");
                    _instance = go.AddComponent<LoadGameData>();
                }
            }
            return _instance;
        }
    }

    public void Items()
    {
        // Load items
        TextAsset itemsJsonFile = Resources.Load<TextAsset>("GameData/items");
        ItemDataWrapper itemDataWrapper = JsonUtility.FromJson<ItemDataWrapper>(itemsJsonFile.text);
        List<Items> items = itemDataWrapper.items;
        ItemSystem.Instance.SetItemsCollection(items);
        InventorySystem.Instance.UpdateUI();
    }

    public void CraftingRecipes()
    {
        // Load crafting recipes
        TextAsset craftingRecipesJsonFile = Resources.Load<TextAsset>("GameData/craftingRecipes");
        CraftingRecipesWrapper craftingRecipesWrapper = JsonUtility.FromJson<CraftingRecipesWrapper>(craftingRecipesJsonFile.text);
        List<CraftingRecipe> craftingRecipes = craftingRecipesWrapper.craftingRecipes;
        CraftingSystem.Instance.SetCraftingCollection(craftingRecipes);
    }

    public void Biomes()
    {
        // Load biomes
        TextAsset biomesJsonFile = Resources.Load<TextAsset>("GameData/biomes");
        BiomeDataWrapper biomeDataWrapper = JsonUtility.FromJson<BiomeDataWrapper>(biomesJsonFile.text);
        List<Biomes> biomes = biomeDataWrapper.biomes;
        BiomeSystem.Instance.SetBiomesCollection(biomes);
    }

    public void Caves()
    {
        // Load caves
        TextAsset cavesJsonFile = Resources.Load<TextAsset>("GameData/caves");
        CaveDataWrapper caveDataWrapper = JsonUtility.FromJson<CaveDataWrapper>(cavesJsonFile.text);
        List<Cave> caves = caveDataWrapper.caves;
        CaveSystem.Instance.SetCavesCollection(caves);
    }
}