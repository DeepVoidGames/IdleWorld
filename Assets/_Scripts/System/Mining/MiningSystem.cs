using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rocks
{
    public string Name;
    public float baseHealth;
    public GameObject prefab;
    public List<Drop> drops;
    public float Health
    {
        get
        {
            return DifficultySystem.Instance.GetBossHealth(baseHealth);
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

    public float miningLevel = 1;
    public float miningEfficiency = 1;

    public void SpawnRock()
    {
        if (currentRock != null)
        {
            Destroy(currentRock);
        }

        Rocks rock = rocks[Random.Range(0, rocks.Count)];
        currentRock = Instantiate(rock.prefab, new Vector3(rockParent.transform.position.x, rockParent.transform.position.y, rockParent.transform.position.z), Quaternion.identity);
        currentRock.transform.SetParent(rockParent.transform);
        RockObject rockObject = currentRock.GetComponent<RockObject>();
        rockObject.Health = rock.Health;
        rockObject.MaxHealth = rock.Health;
        rockObject.drops = rock.drops;
    }

    public void DestroyRock(RockObject rockObject)
    {
        foreach (Drop drop in rockObject.drops)
        {
            float chance = Random.Range(0f, 1f);
            if (chance <= drop.chance)
            {
                float quantity = Random.Range(drop.min, drop.max);
                InventorySystem.Instance.AddItem(drop.ID, quantity);
            }
        }
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

    void Start()
    {
        SpawnRock();
    }
}
