using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary
{
    public List<string> keys;
    public List<InventoryItem> values;

    public SerializableDictionary(Dictionary<string, InventoryItem> dict)
    {
        keys = new List<string>(dict.Keys);
        values = new List<InventoryItem>(dict.Values);
    }

    public Dictionary<string, InventoryItem> ToDictionary()
    {
        Dictionary<string, InventoryItem> dict = new Dictionary<string, InventoryItem>();

        for (int i = 0; i < keys.Count; i++)
        {
            dict[keys[i]] = values[i];
        }

        return dict;
    }
}

public class SaveSystem : MonoBehaviour
{
    public static void SaveInventory(Inventory inventory)
    {
        SerializableDictionary serializableDict = new SerializableDictionary(inventory.items);
        string json = JsonUtility.ToJson(serializableDict, true);
        PlayerPrefs.SetString("inventory", json);
        // Debug.Log("Inventory saved: " + json);
    }

    public static void LoadInventory(Inventory inventory)
    {
        if (!PlayerPrefs.HasKey("inventory"))
        {
            Debug.Log("No inventory data found.");
            return;
        }
        string json = PlayerPrefs.GetString("inventory");
        if (!string.IsNullOrEmpty(json))
        {
            SerializableDictionary serializableDict = JsonUtility.FromJson<SerializableDictionary>(json);
            inventory.items = serializableDict.ToDictionary();
            // Debug.Log("Inventory loaded: " + json);
        }
        else
        {
            Debug.Log("No inventory data found.");
        }
    }
}
