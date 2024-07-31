using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Cave
{
    public string name;
    public List<Rocks> rocks;
    public double costToEnter;
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

    public void SpawnRock()
    {
        if (currentRock != null)
        {
            Destroy(currentRock);
        }

        foreach (Cave cave in caves)
        {
            if (cave.name == currentCave)
            {
                foreach (Rocks rock in cave.rocks)
                {
                    float chance = Random.Range(0f, 1f);
                    if (chance <= rock.chance)
                    {
                        GameObject prefab = Resources.Load<GameObject>("Prefabs/Rocks/" + rock.name);
                        currentRock = Instantiate(prefab, rockParent.transform.position, Quaternion.identity);
                        currentRock.transform.SetParent(rockParent.transform);
                        RockObject rockObject = currentRock.GetComponent<RockObject>();
                        rockObject.Health = rock.Health;
                        rockObject.MaxHealth = rock.Health;
                        rockObject.drops = rock.drops;
                        return;
                    }
                }
            }
        }

        // foreach (Rocks rock in rocks)
        // {
        //     float chance = UnityEngine.Random.Range(0f, 1f);
        //     Debug.Log(String.Format("Chance: {0}, Rock Chance: {1}", chance, rock.chance));
        //     if (chance <= rock.chance)
        //     {
        //         currentRock = Instantiate(rock.prefab, new Vector3(rockParent.transform.position.x, rockParent.transform.position.y, rockParent.transform.position.z), Quaternion.identity);
        //         currentRock.transform.SetParent(rockParent.transform);
        //         RockObject rockObject = currentRock.GetComponent<RockObject>();
        //         rockObject.Health = rock.Health;
        //         rockObject.MaxHealth = rock.Health;
        //         rockObject.drops = rock.drops;
        //         return;
        //     }
        // }
        
        // currentRock = Instantiate(rocks[0].prefab, new Vector3(rockParent.transform.position.x, rockParent.transform.position.y, rockParent.transform.position.z), Quaternion.identity);
        // currentRock.transform.SetParent(rockParent.transform);
        // RockObject rockObjectDefault = currentRock.GetComponent<RockObject>();
        // rockObjectDefault.Health = rocks[0].Health;
        // rockObjectDefault.MaxHealth = rocks[0].Health;
        // rockObjectDefault.drops = rocks[0].drops;  
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
        SpawnRock();
    }

    private void UpdateUI()
    {
        background.sprite = Resources.Load<Sprite>("Sprites/Caves/" + currentCave);
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
}

public class CaveDataWrapper
{
    public List<Cave> caves = new List<Cave>();
}