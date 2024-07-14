using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Rocks
{
    public string Name;
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
    public int ID;
    public float chance;
    public float min;
    public float max;
}

public class RocksDataWrapper
{
    public List<Rocks> rocks = new List<Rocks>();
}

public class MiningSystem : MonoBehaviour
{
    private static MiningSystem _instance;
    public static MiningSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MiningSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MiningSystem");
                    _instance = go.AddComponent<MiningSystem>();
                }
            }
            return _instance;
        }
    }

    public List<Rocks> rocks = new List<Rocks>();

    public GameObject rockParent;
    private GameObject currentRock;

    private double miningLevel = 1;
    private double miningExperience = 0;
    private double miningEfficiency = 1;

    private bool isToolEquiped = false;
    private Items tool;

    public double MiningLevel { get { return miningLevel; } }
    public double MiningExperience { get { return miningExperience; } }
    public double MiningEfficiency { get { return miningEfficiency; } }
    public bool IsToolEquipped { get { return isToolEquiped; } set { isToolEquiped = value; } }

    public void SpawnRock()
    {
        if (currentRock != null)
        {
            Destroy(currentRock);
        }

        foreach (Rocks rock in rocks)
        {
            float chance = UnityEngine.Random.Range(0f, 1f);
            Debug.Log(String.Format("Chance: {0}, Rock Chance: {1}", chance, rock.chance));
            if (chance <= rock.chance)
            {
                currentRock = Instantiate(rock.prefab, new Vector3(rockParent.transform.position.x, rockParent.transform.position.y, rockParent.transform.position.z), Quaternion.identity);
                currentRock.transform.SetParent(rockParent.transform);
                RockObject rockObject = currentRock.GetComponent<RockObject>();
                rockObject.Health = rock.Health;
                rockObject.MaxHealth = rock.Health;
                rockObject.drops = rock.drops;
                return;
            }
        }
        
        currentRock = Instantiate(rocks[0].prefab, new Vector3(rockParent.transform.position.x, rockParent.transform.position.y, rockParent.transform.position.z), Quaternion.identity);
        currentRock.transform.SetParent(rockParent.transform);
        RockObject rockObjectDefault = currentRock.GetComponent<RockObject>();
        rockObjectDefault.Health = rocks[0].Health;
        rockObjectDefault.MaxHealth = rocks[0].Health;
        rockObjectDefault.drops = rocks[0].drops;  
    }

    public void DestroyRock(RockObject rockObject)
    {
        foreach (Drop drop in rockObject.drops)
        {
            float chance = UnityEngine.Random.Range(0f, 1f);
            if (chance <= drop.chance)
            {
                InventorySystem.Instance.AddItem(drop.ID, DifficultySystem.Instance.GetRockDrop(rockObject.MaxHealth, drop.min));
            }
        }
        Destroy(rockObject.gameObject);
        currentRock = null;
        AddMiningExperience(DifficultySystem.Instance.GetMiningExperience(rockObject.MaxHealth));
        UISystem.Instance.UpdateMiningUI();
    }

    public void AddMiningExperience(double value)
    {
        miningExperience += value;
        if (miningExperience >= DifficultySystem.Instance.GetMiningExperienceNeeded())
        {
            miningExperience -= DifficultySystem.Instance.GetMiningExperienceNeeded();
            miningLevel++;
            OverflowMiningExperience();
            UISystem.Instance.UpdateMiningUI();
        }
    }

    private void OverflowMiningExperience()
    {
        if (miningExperience >= DifficultySystem.Instance.GetMiningExperienceNeeded())
        {
            miningExperience -= DifficultySystem.Instance.GetMiningExperienceNeeded();
            miningLevel++;
            UISystem.Instance.UpdateMiningUI();
        }
    }

    public void AddMiningEfficiency(double value)
    {
        miningEfficiency += value;
    }

    public void SetMiningEfficiency(double value)
    {
        miningEfficiency = value;
    }

    public void SetMiningLevel(double value)
    {
        miningLevel = value;
    }

    public void SetMiningExperience(double value)
    {
        miningExperience = value;
    }

    public void EquipTool(Items item)
    {
        this.tool = item;
        miningEfficiency = item.miningEfficiency;
        isToolEquiped = true;
        UISystem.Instance.UpdateMiningUI();
    }

    public Items GetTool()
    {
        return tool;
    }

    public void SetRocks(List<Rocks> rocks)
    {
        foreach (Rocks rock in rocks)
        {
            rock.prefab = Resources.Load<GameObject>($"Prefabs/Rocks/{rock.Name}");
        }
        this.rocks = rocks;
    }

    private void Update() 
    {
        if(currentRock != null)
        {
            RockObject rockObject = currentRock.GetComponent<RockObject>();
            if (rockObject.Health <= 0)
            {
                currentRock = null;
            }
        }
        else
        {
            SpawnRock();
        }
    }

    private void Start() {
        LoadGameData.Instance.Rocks();
    }
}
