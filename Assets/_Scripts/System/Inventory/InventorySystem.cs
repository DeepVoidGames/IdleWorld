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
        Debug.Log("Added " + quantity + " " + id);
        UpdateUI();
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

    private void UpdateUI()
    {
        foreach (Transform child in inventoryUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (InventorySlot slot in inventory.inventory)
        {
            GameObject go = Instantiate(inventorySlotPrefab, inventoryUI.transform);
            go.GetComponent<RectTransform>().localPosition = new Vector3(0, -100 * inventory.inventory.IndexOf(slot), 0);
            go.transform.Find("Icon").GetComponent<Image>().sprite = slot.item.icon;
            go.transform.Find("Quantity").GetComponent<Text>().text = slot.quantity.ToString();
        }
    }

    private void Start()
    {
        UpdateUI();
    }
}
