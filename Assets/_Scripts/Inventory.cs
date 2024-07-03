using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public float amount;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] // Umożliwia wyświetlanie w inspektorze Unity
    private List<InventoryItem> startingItems = new List<InventoryItem>(); // Lista początkowych itemów

    public Dictionary<string, InventoryItem> items = new Dictionary<string, InventoryItem>();

    void Start()
    {
        // Dodawanie początkowych itemów do słownika
        foreach (InventoryItem item in startingItems)
        {
            items[item.itemName] = item;
        }
        SaveSystem.LoadInventory(this);
    }

    public void AddItem(string itemName, float amount = 1f)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName].amount += amount;
        }
        else
        {
            Debug.Log("Item " + itemName + " not found in inventory.");
        }
        SaveSystem.SaveInventory(this);
        // Debug.Log("Added " + amount + " of " + itemName + " to inventory. Total: " + items[itemName].amount);
    }

    public void PrintInventory()
    {
        foreach (KeyValuePair<string, InventoryItem> item in items)
        {
            Debug.Log(item.Key + ": " + item.Value.amount);
        }
    }

    public void RemoveItem(string itemName, float amount = 1f)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName].amount -= amount;

            if (items[itemName].amount <= 0)
            {
                items.Remove(itemName);
            }

            Debug.Log("Removed " + amount + " of " + itemName + " from inventory.");
        }
        else
        {
            Debug.Log("Item " + itemName + " not found in inventory.");
        }
    }

    public bool HasItem(string itemName)
    {
        return items.ContainsKey(itemName);
    }

    public float GetItemCount(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            return items[itemName].amount;
        }
        else
        {
            return 0f;
        }
    }
}
