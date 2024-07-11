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

    // Weapon
    public float damage;
    public float damageBoostPercentage;

    // Tools
    public float miningEfficiency;
}

public class ItemDataWrapper
{
    public List<Items> items;
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

    public void SetItemsCollection(List<Items> items)
    {
        foreach (Items item in items)
        {
            if (item.category == InventorySystem.Category.Material)
                item.icon = Resources.Load<Sprite>("Icons/Materials/" + item.Name);
            else if (item.category == InventorySystem.Category.Weapon)
                item.icon = Resources.Load<Sprite>("Icons/Weapons/" + item.Name);
            else if (item.category == InventorySystem.Category.Tools)
                item.icon = Resources.Load<Sprite>("Icons/Tools/" + item.Name);
            ItemsCollection.Add(item);
        }
    }
}