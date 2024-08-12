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
        Legendary,
        Mythical,
    }
    public Sprite icon;

    // Weapon
    public float baseDamage;
    public float Damage 
    { 
        get => baseDamage + UpgradingSystem.Instance.GetBonus(Name, baseDamage);
    }
    public float damageBoostPercentage;

    // Tools
    public float baseMiningEfficiency;

    public float miningEfficiency 
    {
        get => baseMiningEfficiency + UpgradingSystem.Instance.GetBonus(Name, baseMiningEfficiency);
    }

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

    public Items GetItemByName(string name)
    {
        return ItemsCollection.Find(x => x.Name == name);
    }

    public Sprite GetItemIconByName(string name)
    {
        return ItemsCollection.Find(x => x.Name == name).icon;
    }

    public void SetItemsCollection(List<Items> items)
    {
        List<Items> _items = new List<Items>();
        _items.AddRange(items);
        ItemsCollection.Clear();
        foreach (Items item in _items)
        {
            if (item.category == InventorySystem.Category.Material)
                item.icon = Resources.Load<Sprite>("Sprites/Icons/Materials/" + item.Name);
            else if (item.category == InventorySystem.Category.Weapon)
                item.icon = Resources.Load<Sprite>("Sprites/Icons/Weapons/" + item.Name);
            else if (item.category == InventorySystem.Category.Tools)
                item.icon = Resources.Load<Sprite>("Sprites/Icons/Tools/" + item.Name);
            ItemsCollection.Add(item);
        }
        _items.Clear();
    }

    private void Start() {
        LoadGameData.Instance.Items();
    }
}