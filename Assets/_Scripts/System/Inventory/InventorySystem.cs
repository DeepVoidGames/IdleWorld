using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public Items item;
    public double quantity;
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
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject inventoryUI;
    public GameObject InventoryPanel { get { return inventoryPanel; } }
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject inventoryIndicator;
    [SerializeField] private GameObject categoryUI;
    [SerializeField] private Category currentCategory;

    [SerializeField] private GameObject inventoryContent;

    public Button equipedWeaponButton;
    public Button equipedToolButton;

    public enum Category
    {
        None,      
        Material,
        Weapon,
        Tools,
    }

    public void AddItem(int id, double quantity)
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
        inventoryIndicator.GetComponent<MessageSpawner>().SpawnMessage($"{slot.item.Name} +{UISystem.Instance.NumberFormat(quantity)}", ItemSystem.Instance.GetItemIcon(id));
        UpdateUI();
    }

    public void AddItemByName(string resourceName, double quantity)
    {
        InventorySlot slot = inventory.inventory.Find(x => x.item.Name == resourceName);
        if (slot == null)
        {
            slot = new InventorySlot();
            slot.item = ItemSystem.Instance.ItemsCollection.Find(x => x.Name == resourceName);
            slot.quantity = 0;
            inventory.inventory.Add(slot);
        }
        
        slot.quantity += quantity;
        inventoryIndicator.GetComponent<MessageSpawner>().SpawnMessage($"{slot.item.Name} +{UISystem.Instance.NumberFormat(quantity)}", ItemSystem.Instance.GetItemIcon(slot.item.id));
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

    public double GetResourceByName(string resourceName)
    {
        InventorySlot slot = inventory.inventory.Find(x => x.item.Name == resourceName);
        if (slot != null)
        {
            return slot.quantity;
        }
        return 0;
    }

    public void RemoveItemByName(string resourceName, double quantity)
    {
        InventorySlot slot = inventory.inventory.Find(x => x.item.Name == resourceName);
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

    public double GetQuantity(int id)
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
        
        // Sort by rarity if the category is Material
        if (currentCategory == Category.Material)
        {
            // Sort by rarity
            for (int i = 0; i < inventory.inventory.Count; i++)
            {
                for (int j = i + 1; j < inventory.inventory.Count; j++)
                {
                    if (inventory.inventory[i].item.rarity < inventory.inventory[j].item.rarity)
                    {
                        InventorySlot temp = inventory.inventory[i];
                        inventory.inventory[i] = inventory.inventory[j];
                        inventory.inventory[j] = temp;
                    }
                }
            }
        }

        // Sort by mining efficiency if the category is Tools
        if (currentCategory == Category.Tools)
        {
            // Sort by mining efficiency
            for (int i = 0; i < inventory.inventory.Count; i++)
            {
                for (int j = i + 1; j < inventory.inventory.Count; j++)
                {
                    if (inventory.inventory[i].item.miningEfficiency < inventory.inventory[j].item.miningEfficiency)
                    {
                        InventorySlot temp = inventory.inventory[i];
                        inventory.inventory[i] = inventory.inventory[j];
                        inventory.inventory[j] = temp;
                    }
                }
            }
        }

        // Show the inventory UI
        Vector3 pos = new Vector3(178.75f, -48, 0);
        

        //Count all the items in the inventory with the current category
        int itemCount = 0;
        foreach (InventorySlot slot in inventory.inventory)
        {
            if (slot.item.category == currentCategory)
            {
                itemCount++;
            }
        } 

        RectTransform rt = inventoryContent.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, Mathf.Abs(110 * itemCount));
        rt.position = new Vector3(rt.position.x, rt.position.y, rt.position.z);  
        

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
                    onClick.AddListener(() => CraftingSystem.Instance.SetCraftSlot(slot.item));
                    button.onClick = onClick;
                }
                
                // If the category is Weapon
                if (slot.item.category == Category.Weapon)
                {
                    go.transform.Find("Quantity").GetComponent<Text>().text = String.Format("Damage: {0}\nDamage Bonus: {1}%", UISystem.Instance.NumberFormat(slot.item.damage), UISystem.Instance.NumberFormat((slot.item.damageBoostPercentage)));
                    go.transform.Find("Equip").gameObject.SetActive(true);
                    Button button = go.transform.Find("Equip").GetComponent<Button>();

                    if (DamageSystem.Instance.GetWeapon() != null && DamageSystem.Instance.GetWeapon().id == slot.item.id)
                    {
                        button.transform.Find("Text").GetComponent<Text>().text = "Equiped";
                        equipedWeaponButton = button;
                    }
                    else
                    {
                        button.transform.Find("Text").GetComponent<Text>().text = "Equip";
                    }

                    var onClick = new Button.ButtonClickedEvent();
                    onClick.AddListener(() => {
                        DamageSystem.Instance.EquipWeapon(slot.item);
                        button.transform.Find("Text").GetComponent<Text>().text = "Equiped";
                        if (equipedWeaponButton != null && equipedWeaponButton != button)
                        {
                            equipedWeaponButton.transform.Find("Text").GetComponent<Text>().text = "Equip";
                        }
                        equipedWeaponButton = button;
                    });
                    button.onClick = onClick;
                }                


                if (slot.item.category == Category.Tools)
                {
                    go.transform.Find("Quantity").GetComponent<Text>().text = String.Format("Mining Efficiency: {0}", UISystem.Instance.NumberFormat(slot.item.miningEfficiency));
                    go.transform.Find("Equip").gameObject.SetActive(true);
                    Button button = go.transform.Find("Equip").GetComponent<Button>();

                    if (MiningSystem.Instance.GetTool() != null && MiningSystem.Instance.GetTool().id == slot.item.id)
                    {
                        button.transform.Find("Text").GetComponent<Text>().text = "Equiped";
                        equipedToolButton = button;
                    }
                    else
                    {
                        button.transform.Find("Text").GetComponent<Text>().text = "Equip";
                    }

                    var onClick = new Button.ButtonClickedEvent();
                    onClick.AddListener(() => {
                        MiningSystem.Instance.EquipTool(slot.item);
                        button.transform.Find("Text").GetComponent<Text>().text = "Equiped";
                        if (equipedToolButton != null && equipedToolButton != button)
                        {
                            equipedToolButton.transform.Find("Text").GetComponent<Text>().text = "Equip";
                        }
                        equipedToolButton = button;
                    });
                    button.onClick = onClick;
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

    private void Start() {
        if (equipedWeaponButton != null)
        {
            equipedWeaponButton.transform.Find("Text").GetComponent<Text>().text = "Equipped";
        }
    }
}
