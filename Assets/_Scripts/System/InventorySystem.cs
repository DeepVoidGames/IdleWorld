using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public float amount;
}

public class InventorySystem : MonoBehaviour
{
    private static InventorySystem _instance;
    public static InventorySystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventorySystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("InventorySystem");
                    _instance = go.AddComponent<InventorySystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] // Umożliwia wyświetlanie w inspektorze Unity
    private List<InventoryItem> startingItems = new List<InventoryItem>(); // Lista początkowych itemów

    public Dictionary<string, InventoryItem> items = new Dictionary<string, InventoryItem>();

    private void Awake()
    {
        // Dodawanie początkowych itemów do słownika
        foreach (InventoryItem item in startingItems)
        {
            items[item.itemName] = item;
        }
    }

    public void AddItem(string itemName, float amount = 1f)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName].amount += amount;
            SaveSystem.Instance.SaveInventory();
        }
        else
        {
            Debug.Log("Item " + itemName + " not found in inventory.");
        }
    }

    public void RemoveItem(string itemName, float amount = 1f)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName].amount -= amount;
            SaveSystem.Instance.SaveInventory();
        }
        else
        {
            Debug.Log("Item " + itemName + " not found in inventory.");
        }
    }

    public void GetItem(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            Debug.Log("Amount of " + itemName + " in inventory: " + items[itemName].amount);
        }
        else
        {
            Debug.Log("Item " + itemName + " not found in inventory.");
        }
    }
}
