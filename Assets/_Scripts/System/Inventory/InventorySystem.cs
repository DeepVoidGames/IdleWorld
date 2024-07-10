using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public Items item;
    public float quantity;
}

[System.Serializable]
public class Inventory
{
    public List<InventorySlot> inventory = new List<InventorySlot>();
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

    [Header("Inventory")]
    public Inventory inventory = new Inventory();

    [Header("UI")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject inventorySlotPrefab;

    [SerializeField] private GameObject categoryUI;
    [SerializeField] private Category currentCategory;

    public enum Category
    {
        None,      
        Material,
        Weapon,
        Tools,
    }

    public void AddItem(int id, float quantity)
    {
        InventorySlot slot = inventory.inventory.Find(x => x.item.id == id);
        if (slot == null)
        {
            slot = new InventorySlot();
            slot.item = ItemSystem.Instance.ItemsCollection.Find(x => x.id == id);
            slot.quantity = 0;
            inventory.inventory.Add(slot);
        }
        slot.quantity += quantity;
        UpdateUI();
        SaveSystem.Instance.Save();
    }

    public void RemoveItem(int id, float quantity)
    {
        InventorySlot slot =  inventory.inventory.Find(x => x.item.id == id);
        if (slot != null)
        {
            slot.quantity -= quantity;
            if (slot.quantity <= 0)
            {
                inventory.inventory.Remove(slot);
            }
        }
        UpdateUI();
        SaveSystem.Instance.Save();
    }

    public float GetQuantity(int id)
    {
        InventorySlot slot = inventory.inventory.Find(x => x.item.id == id);
        if (slot != null)
        {
            return slot.quantity;
        }
        return 0;
    }

    public void UpdateUI()
    {
        //  Clear the inventory UI
        foreach (Transform child in inventoryUI.transform)
        {
            Destroy(child.gameObject);
        }

        // If the inventory is empty, return
        if (inventory.inventory.Count == 0)
        {
            return;
        }

        // If the category is None, show the category UI
        if(currentCategory == Category.None)
        {
            categoryUI.SetActive(true);
            return;
        }
        else
        {
            categoryUI.SetActive(false);
        }

        // Sort by damage if the category is Weapon
        if (currentCategory == Category.Weapon)
        {
            // Sort by damage
            for (int i = 0; i < inventory.inventory.Count; i++)
            {
                for (int j = i + 1; j < inventory.inventory.Count; j++)
                {
                    if (inventory.inventory[i].item.damage < inventory.inventory[j].item.damage)
                    {
                        InventorySlot temp = inventory.inventory[i];
                        inventory.inventory[i] = inventory.inventory[j];
                        inventory.inventory[j] = temp;
                    }
                }
            }
    
        }
        
        // Show the inventory UI
        Vector3 pos = new Vector3(178.75f, -45.95f, 0);
        foreach (InventorySlot slot in inventory.inventory)
        {
            if (slot.item.category == currentCategory)
            {
                GameObject go = Instantiate(inventorySlotPrefab, inventoryUI.transform);
                go.transform.localPosition = pos;
                go.transform.Find("Icon").GetComponent<Image>().sprite = ItemSystem.Instance.GetItemIcon(slot.item.id);
                go.transform.Find("Quantity").GetComponent<Text>().text = UISystem.Instance.NumberFormat(slot.quantity);
                go.transform.Find("Rarity").GetComponent<Text>().text = slot.item.rarity.ToString();
                go.transform.Find("Rarity").GetComponent<Text>().color = UISystem.Instance.GetRarityColor(slot.item.rarity);
                go.transform.Find("Title").GetComponent<Text>().text = slot.item.Name;

                // If the category is Material 
                if (slot.item.category == Category.Material)
                {
                    Button button = go.GetComponent<Button>();
                    var onClick = new Button.ButtonClickedEvent();
                    onClick.AddListener(() => CraftingSystem.Instance.SetCraftSlot(slot.item.id));
                    button.onClick = onClick;
                }
                
                // If the category is Weapon
                if (slot.item.category == Category.Weapon)
                {
                    go.transform.Find("Quantity").GetComponent<Text>().text = String.Format("Damage: {0}\nDamage Bonus: {1}%", slot.item.damage, slot.item.damageBoostPercentage);
                    go.transform.Find("Equip").gameObject.SetActive(true);
                }                

                pos.y -= 100;
            }
        }
    }

    public void SetCategory(int category)
    {
        currentCategory = (Category)category;
        UpdateUI();
    }

    public void OpenCategory()
    {
        currentCategory = Category.None;
        UpdateUI();
    }
}
