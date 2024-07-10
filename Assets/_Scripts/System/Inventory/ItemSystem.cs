using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Items
{
    public string Name;
    public int id;
    public InventorySystem.Category category;
    public Rarity rarity;
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    public Sprite icon;
}

public class ItemSystem : MonoBehaviour 
{
    private static ItemSystem _instance;
    public static ItemSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ItemSystem");
                    _instance = go.AddComponent<ItemSystem>();
                }
            }
            return _instance;
        }
    }

    public List<Items> ItemsCollection = new List<Items>();

    public Sprite GetItemIcon(int id)
    {
        return ItemsCollection.Find(x => x.id == id).icon;
    }
}