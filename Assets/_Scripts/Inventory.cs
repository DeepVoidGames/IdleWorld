using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<string, float> items = new Dictionary<string, float>();

    public void AddItem(string itemName, float amount = 1f)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] += amount;
        }
        else
        {
            items[itemName] = amount;
        }

        Debug.Log("Added " + amount + " of " + itemName + " to inventory. Total: " + items[itemName]);
    }

    public void PrintInventory()
    {
        foreach (KeyValuePair<string, float> item in items)
        {
            Debug.Log(item.Key + ": " + item.Value);
        }
    }

    public void RemoveItem(string itemName, float amount = 1f)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] -= amount;

            if (items[itemName] <= 0)
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
            return items[itemName];
        }
        else
        {
            return 0f;
        }
    }
}
