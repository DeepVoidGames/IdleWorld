using System.Collections.Generic;
using System.IO;
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

    private const string SaveFileName = "save.json";
    private const string InventoryFileName = "inventory.json";
    private const string HeroesFileName = "heroes.json";
    private const string CavesFileName = "caves.json";
    private const string UpgradesFileName = "upgrades.json";
    private const string MagicFileName = "magic.json";

    private const string PlantingFileName = "planting.json";

    private string saveFilePath;
    private float _timer = 0f;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    private void Start()
    {
        Load();
        UISystem.Instance.LoadUI();
    }

    public void Save()
    {
        SaveGameData();
        SaveInventoryData();
        SaveHeroData();
        SaveCavesData();
        SaveUpgradesData();
        SaveMagicData();
        SavePlantingData();
    }

    private void SaveGameData()
    {
        GameData gameData = new GameData
        {
            goldData = GoldSystem.Instance.Gold,
            levelData = LevelSystem.Instance.Level,
            stageData = LevelSystem.Instance.Stage,
            weaponData = DamageSystem.Instance.GetWeapon(),
            isWeaponEquippedData = DamageSystem.Instance.IsWeaponEquipped,
            weaponButtonData = InventorySystem.Instance.equipedWeaponButton,
            miningLevelData = MiningSystem.Instance.MiningLevel,
            miningEfficiencyData = MiningSystem.Instance.MiningEfficiency,
            miningExperienceData = MiningSystem.Instance.MiningExperience,
            toolData = MiningSystem.Instance.GetTool(),
            isToolEquippedData = MiningSystem.Instance.IsToolEquipped,
            toolButtonData = InventorySystem.Instance.equipedToolButton,
            currentCave = CaveSystem.Instance.CurrentCave ?? "Stonecrest Quarry",
            prestigeLevel = LevelSystem.Instance.PrestigeLevel
        };
        WriteToFile(saveFilePath, gameData);
    }

    private void SaveInventoryData()
    {
        InventoryData inventoryData = new InventoryData
        {
            inventoryData = InventorySystem.Instance.inventory
        };
        WriteToFile(Path.Combine(Application.persistentDataPath, InventoryFileName), inventoryData);
    }

    private void SaveHeroData()
    {
        HeroData heroData = new HeroData
        {
            heroes = TavernSystem.Instance.heroes
        };
        WriteToFile(Path.Combine(Application.persistentDataPath, HeroesFileName), heroData);
    }

    private void SaveCavesData()
    {
        CavesData cavesData = new CavesData
        {
            caves = CaveSystem.Instance.caves
        };
        WriteToFile(Path.Combine(Application.persistentDataPath, CavesFileName), cavesData);
    }

    private void SaveUpgradesData()
    {
        Upgrades upgrades = new Upgrades
        {
            upgrades = UpgradingSystem.Instance.Upgrades
        };
        WriteToFile(Path.Combine(Application.persistentDataPath, UpgradesFileName), upgrades);
    }

    private void SaveMagicData()
    {
        MagicData magicData = new MagicData
        {
            manaData = ManaSystem.Instance.GetMana(),
            manaPerHourData = ManaSystem.Instance.GetManaPerHour()
        };
        WriteToFile(Path.Combine(Application.persistentDataPath, MagicFileName), magicData);
    }

    private void SavePlantingData()
    {
        PlantingData plantingData = new PlantingData
        {
            plantIndex = PlantsSystem.Instance.SavePlantData(),
            plantingLevel = PlantingSystem.Instance.PlantingLevel
        };
        WriteToFile(Path.Combine(Application.persistentDataPath, PlantingFileName), plantingData);
    }

    public void Load()
    {
        LoadInventoryData();
        LoadHeroData();
        LoadUpgradesData();
        LoadGameData();
        LoadMagicData();
    }

    private void LoadInventoryData()
    {
        InventoryData inventoryData = ReadFromFile<InventoryData>(Path.Combine(Application.persistentDataPath, InventoryFileName));
        if (inventoryData?.inventoryData != null)
        {
            InventorySystem.Instance.SetInventory(inventoryData.inventoryData);
        }
    }

    private void LoadHeroData()
    {
        HeroData heroData = ReadFromFile<HeroData>(Path.Combine(Application.persistentDataPath, HeroesFileName));
        if (heroData?.heroes != null)
        {
            foreach (var hero in heroData.heroes)
            {
                Hero existingHero = TavernSystem.Instance.heroes.Find(x => x.name == hero.name);
                if (existingHero != null)
                {
                    existingHero.level = hero.level;
                    existingHero.isUnlocked = hero.isUnlocked;
                    if (hero.isUnlocked)
                    {
                        TavernSystem.Instance.SpawnHero(TavernSystem.Instance.heroes.IndexOf(existingHero));
                    }
                }
                else
                {
                    Debug.LogWarning($"Hero not found: {hero.name}");
                }
            }
        }
    }

    private void LoadUpgradesData()
    {
        Upgrades upgrades = ReadFromFile<Upgrades>(Path.Combine(Application.persistentDataPath, UpgradesFileName));
        if (upgrades?.upgrades != null)
        {
            UpgradingSystem.Instance.SetUpgrades(upgrades.upgrades);
        }
    }

    private void LoadGameData()
    {
        GameData gameData = ReadFromFile<GameData>(saveFilePath);
        if (gameData != null)
        {
            GoldSystem.Instance.SetGold(gameData.goldData);
            LevelSystem.Instance.SetLevel(gameData.levelData);
            ContentLocker.Instance.CheckContent(gameData.levelData);
            BiomeSystem.Instance.UpdateBiome();
            LevelSystem.Instance.SetStage(gameData.stageData);
            DamageSystem.Instance.EquipWeapon(gameData.weaponData);
            DamageSystem.Instance.IsWeaponEquipped = gameData.isWeaponEquippedData;
            InventorySystem.Instance.equipedWeaponButton = gameData.weaponButtonData;
            MiningSystem.Instance.SetMiningLevel(gameData.miningLevelData);
            MiningSystem.Instance.SetMiningEfficiency(gameData.miningEfficiencyData);
            MiningSystem.Instance.SetMiningExperience(gameData.miningExperienceData);
            MiningSystem.Instance.EquipTool(gameData.toolData);
            MiningSystem.Instance.IsToolEquipped = gameData.isToolEquippedData;
            InventorySystem.Instance.equipedToolButton = gameData.toolButtonData;
            CaveSystem.Instance.CurrentCave = gameData.currentCave;
            LevelSystem.Instance.SetPrestigeLevel(gameData.prestigeLevel);
        }
    }

    private void LoadMagicData()
    {
        MagicData magicData = ReadFromFile<MagicData>(Path.Combine(Application.persistentDataPath, MagicFileName));
        if (magicData != null)
        {
            ManaSystem.Instance.AddMana(magicData.manaData);
            ManaSystem.Instance.SetManaPerHour(magicData.manaPerHourData);
        }
    }

    public void LoadCaves()
    {
        CavesData cavesData = ReadFromFile<CavesData>(Path.Combine(Application.persistentDataPath, CavesFileName));
        if (cavesData?.caves != null)
        {
            foreach (var cave in cavesData.caves)
            {
                Cave existingCave = CaveSystem.Instance.caves.Find(x => x.name == cave.name);
                if (existingCave != null)
                {
                    CaveSystem.Instance.LoadCave(cave.name, cave.isUnlocked);
                }
                else
                {
                    Debug.LogWarning($"Cave not found: {cave.name}");
                }
            }
        }
    }

    public void LoadPlantingData()
    {
        PlantingData plantingData = ReadFromFile<PlantingData>(Path.Combine(Application.persistentDataPath, PlantingFileName));
        if (plantingData != null)
        {
            PlantsSystem.Instance.LoadPlantData(plantingData.plantIndex);
            PlantingSystem.Instance.PlantingLevel = plantingData.plantingLevel;
        }
    }

    // !
    private void FixedUpdate()
    {
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

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Save();
        }
    }

    private void WriteToFile<T>(string filePath, T data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to write to file {filePath}: {ex.Message}");
        }
    }

    private T ReadFromFile<T>(string filePath) where T : class
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(json);
            }
            else
            {
                Debug.LogWarning($"File not found: {filePath}");
                return null;
            }
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to read from file {filePath}: {ex.Message}");
            return null;
        }
    }
}

[System.Serializable]
public class GameData
{
    public double goldData;
    public int levelData;
    public int stageData;
    public Items weaponData;
    public bool isWeaponEquippedData;
    public UnityEngine.UI.Button weaponButtonData;
    public double miningLevelData;
    public double miningEfficiencyData;
    public double miningExperienceData;
    public Items toolData;
    public bool isToolEquippedData;
    public UnityEngine.UI.Button toolButtonData;
    public string currentCave;
    public int prestigeLevel;
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

public class MagicData
{
    public double manaData;
    public double manaPerHourData;
}

[System.Serializable]
public class PlantingData
{
    public List<PlantData> plantIndex;
    public int plantingLevel;
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