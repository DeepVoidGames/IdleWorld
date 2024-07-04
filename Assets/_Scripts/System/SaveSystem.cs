using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

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
    }

    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath);
        LoadInventory();
        LoadMiners();
        Debug.Log(savePath);
    }

    public void SaveInventory()
    {
        var json = JsonUtility.ToJson(new Serialization<InventoryItem>(InventorySystem.Instance.items.Values.ToList()), true);
        File.WriteAllText(Path.Combine(savePath, "inventory.json"), json);
        Debug.Log("Inventory saved");
    }

    public void LoadInventory()
    {
        if (File.Exists(Path.Combine(savePath, "inventory.json")))
        {
            var json = File.ReadAllText(Path.Combine(savePath, "inventory.json"));
            var itemsList = JsonUtility.FromJson<Serialization<InventoryItem>>(json).ToList();
            // InventorySystem.Instance.items.Clear();
            foreach (var item in itemsList)
            {
                InventorySystem.Instance.items[item.itemName] = item;
            }
            Debug.Log("Inventory loaded");
        }
    }
    
    public void SaveMiners()
    {
        var json = JsonUtility.ToJson(new Serialization<Miner>(MinerSystem.Instance.minersDict.Values.ToList()), true);
        File.WriteAllText(Path.Combine(savePath, "miners.json"), json);
        Debug.Log("Miners saved");
    }

    public void LoadMiners()
    {
        if (File.Exists(Path.Combine(savePath, "miners.json")))
        {
            var json = File.ReadAllText(Path.Combine(savePath, "miners.json"));
            var minersList = JsonUtility.FromJson<Serialization<Miner>>(json).ToList();
            // MinerSystem.Instance.minersDict.Clear();
            foreach (var miner in minersList)
            {
                MinerSystem.Instance.minersDict[miner.minerName] = miner;
            }
            Debug.Log("Miners loaded");
        }
    }
}

[System.Serializable]
public class Serialization<T>
{
    [SerializeField]
    private List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}