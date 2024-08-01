using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Cave
{
    public string name;
    public List<Rocks> rocks;
    public double costToEnter;
    public string resourceRequiredToEnter;
    public bool isUnlocked;
}

[System.Serializable]
public class Rocks
{
    public string name;
    public double baseHealth;
    public float chance;
    public GameObject prefab;
    public List<Drop> drops;
    public double Health
    {
        get
        {
            return DifficultySystem.Instance.GetRockHealth(baseHealth);
        }
    }
}

[System.Serializable]
public class Drop
{
    public int resourceName;
    public float chance;
}

public class CaveSystem : MonoBehaviour 
{
    private static CaveSystem _instance;
    public static CaveSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CaveSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CaveSystem");
                    _instance = go.AddComponent<CaveSystem>();
                }
            }
            return _instance;
        }
    }

    public List<Cave> caves = new List<Cave>();

    [Header("Rocks")]
    private GameObject currentRock;
    [SerializeField] private GameObject rockParent;
    
    private string currentCave;

    public string CurrentCave
    {
        get
        {
            return currentCave;
        }
        set
        {
            currentCave = value;
            UpdateUI();
            SpawnRock();
        }
    }

    [Header("UI Elements")]
    [SerializeField] private Image background;
    
    public delegate void CaveLoadedEventHandler();
    public event CaveLoadedEventHandler OnCaveChanged;

    public void SpawnRock()
    {
        if (currentRock != null)
        {
            Destroy(currentRock);
        }

        Cave currentCaveData = GetCave(currentCave);
        if (currentCaveData != null)
        {
            Rocks selectedRock = GetRandomRock(currentCaveData);
            if (selectedRock != null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Rocks/" + selectedRock.name);
                currentRock = Instantiate(prefab, rockParent.transform.position, Quaternion.identity);
                currentRock.transform.SetParent(rockParent.transform);
                RockObject rockObject = currentRock.GetComponent<RockObject>();
                rockObject.Health = selectedRock.Health;
                rockObject.MaxHealth = selectedRock.Health;
                rockObject.drops = selectedRock.drops;
            }
        }
    }

    private Rocks GetRandomRock(Cave cave)
    {
        foreach (Rocks rock in cave.rocks)
        {
            float r = Random.Range(0f, 100f);
            if (r <= rock.chance)
            {
                return rock;
            }
        }
        return cave.rocks[0];
    }

    public void DestroyRock(RockObject rockObject)
    {
        foreach (Drop drop in rockObject.drops)
        {
            float chance = Random.Range(0f, 1f);
            if (chance <= drop.chance)
            {
                InventorySystem.Instance.AddItem(drop.resourceName, DifficultySystem.Instance.GetRockDrop(rockObject.MaxHealth, drop.resourceName));
            }
        }
        Destroy(rockObject.gameObject);
        currentRock = null;
        MiningSystem.Instance.AddMiningExperience(DifficultySystem.Instance.GetMiningExperience(rockObject.MaxHealth));
        UISystem.Instance.UpdateMiningUI();
    }

    private void UpdateUI()
    {
        background.sprite = Resources.Load<Sprite>("Sprites/Caves/" + currentCave);
    }

    public void UpdateCurrentCave(string name)
    {
        currentCave = name;
        OnCaveChanged?.Invoke();
        UpdateUI();
        SpawnRock();
    }

    public void UnlockCave(string name)
    {
        foreach (Cave cave in caves)
        {
            if (cave.name == name)
            {
                cave.isUnlocked = true;
            }
        }
        UpdateCurrentCave(name);
    }
    
    public void LoadCave(string name, bool isUnlocked)
    {
        foreach (Cave cave in caves)
        {
            if (cave.name == name)
            {
                cave.isUnlocked = isUnlocked;
            }
        }
        OnCaveChanged?.Invoke();
    }

    public Cave GetCave(string name)
    {
        foreach (Cave cave in caves)
        {
            if (cave.name == name)
            {
                return cave;
            }
        }
        return null;
    }

    private void Awake() 
    {
        LoadGameData.Instance.Caves();    
    }

    private void Start() 
    {
        if(currentCave == "" || currentCave == null)
        {
            currentCave = caves[0].name;
        }
        Debug.Log($"Current Cave: {currentCave}");

        UpdateUI();
        SpawnRock();
    }

    private void Update() 
    {
        if (currentRock == null)
        {
            SpawnRock();
        }
    }
}

public class CaveDataWrapper
{
    public List<Cave> caves = new List<Cave>();
}