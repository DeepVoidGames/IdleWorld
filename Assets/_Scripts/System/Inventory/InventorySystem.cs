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

    public Inventory inventory = new Inventory();

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
        foreach (Transform child in inventoryUI.transform)
        {
            Destroy(child.gameObject);
        }

        if (inventory.inventory.Count == 0)
        {
            return;
        }

        if(currentCategory == Category.None)
        {
            categoryUI.SetActive(true);
            return;
        }else
        {
            categoryUI.SetActive(false);
        }

        foreach (InventorySlot slot in inventory.inventory)
        {
            Debug.Log(String.Format("Item: {0}, Quantity: {1}, Category: {2}", slot.item.Name, slot.quantity, slot.item.category));
            if (slot.item.category == currentCategory)
            {
                GameObject go = Instantiate(inventorySlotPrefab, inventoryUI.transform);
                go.transform.Find("Icon").GetComponent<Image>().sprite = ItemSystem.Instance.GetItemIcon(slot.item.id);
                go.transform.Find("Quantity").GetComponent<Text>().text = UISystem.Instance.NumberFormat(slot.quantity);
                go.transform.Find("Title").GetComponent<Text>().text = slot.item.Name;
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
