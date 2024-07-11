using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Rocks
{
    public string Name;
    public float baseHealth;
    public float chance;
    public GameObject prefab;
    public List<Drop> drops;
    public float Health
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

    private float miningLevel = 1;
    private float miningExperience = 0;
    private float miningEfficiency = 1;
    private float baseMiningEfficiency = 1;

    private bool isToolEquiped = false;
    private Items tool;

    public float MiningLevel { get { return miningLevel; } }
    public float MiningExperience { get { return miningExperience; } }
    public float MiningEfficiency { get { return baseMiningEfficiency + miningEfficiency; } }
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
        AddMiningExperience(DifficultySystem.Instance.GetMiningExperience(rockObject.MaxHealth));
        UISystem.Instance.UpdateMiningUI();
    }

    public void AddMiningExperience(float value)
    {
        miningExperience += value;
        if (miningExperience >= DifficultySystem.Instance.GetMiningExperienceNeeded())
        {
            miningExperience -= DifficultySystem.Instance.GetMiningExperienceNeeded();
            miningLevel++;
            miningEfficiency = DifficultySystem.Instance.GetBaseMiningEfficiency();
            UISystem.Instance.UpdateMiningUI();
            AddMiningExperience(miningExperience);
        }
    }

    public void AddMiningEfficiency(float value)
    {
        miningEfficiency += value;
    }

    public void SetMiningEfficiency(float value)
    {
        miningEfficiency = DifficultySystem.Instance.GetMiningEfficiency(value);
    }

    public void SetMiningLevel(float value)
    {
        miningLevel = value;
    }

    public void SetMiningExperience(float value)
    {
        miningExperience = value;
    }

    public void EquipTool(Items item)
    {
        this.tool = item;
        baseMiningEfficiency = item.miningEfficiency;
        isToolEquiped = true;
        UISystem.Instance.UpdateMiningUI();
    }

    public Items GetTool()
    {
        return tool;
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
}
