using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public string name;
    public Items item { get { return ItemSystem.Instance.ItemsCollection.Find(x => x.Name == name); } }
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

    [SerializeField] private List<AnimationClip> animations;
    // 178.75 + 75.3500 = 254.1 
    // -150.7
    // -75.35001
    private Vector3 basePos = new Vector3(254.1f, 77.35f, 0);
    private Vector3 pos;

    public Button equipedWeaponButton;
    public Button equipedToolButton;

    //! EVENT
    public event Action<string> OnItemChanged;

    public enum Category
    {
        None,      
        Material,
        Weapon,
        Tools,
        Potion
    }

    public void AddItem(int id, double quantity)
    {
        InventorySlot slot = inventory.inventory.Find(x => x.item.id == id);
        if (slot == null)
        {
            slot = new InventorySlot
            {
                name = ItemSystem.Instance.ItemsCollection.Find(x => x.id == id).Name,
                quantity = 0
            };
            inventory.inventory.Add(slot);
        }
        
        slot.quantity += quantity;
        OnItemChanged?.Invoke(slot.item.Name);
        inventoryIndicator.GetComponent<MessageSpawner>().SpawnMessage($"{slot.item.Name} +{UISystem.Instance.NumberFormat(quantity)}", ItemSystem.Instance.GetItemIcon(id));
        ClearDuplicates();
        UpdateUI();
    }

    private void ClearDuplicates()
    {
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
            for (int j = i + 1; j < inventory.inventory.Count; j++)
            {
                if (inventory.inventory[i].item.id == inventory.inventory[j].item.id)
                {
                    inventory.inventory[i].quantity += inventory.inventory[j].quantity;
                    inventory.inventory.RemoveAt(j);
                    j--;
                }
            }
        }
    }

    public void AddItemByName(string name, double quantity)
    {
        int id = ItemSystem.Instance.ItemsCollection.Find(x => x.Name == name).id;
        InventorySlot slot = inventory.inventory.Find(x => x.item.id == id);
        if (slot == null)
        {
            slot = new InventorySlot()
            {
                name = name,
                quantity = 0
            };
            inventory.inventory.Add(slot);
        }
        
        slot.quantity += quantity;
        OnItemChanged?.Invoke(slot.item.Name);
        inventoryIndicator.GetComponent<MessageSpawner>().SpawnMessage($"{slot.item.Name} +{UISystem.Instance.NumberFormat(quantity)}", ItemSystem.Instance.GetItemIcon(slot.item.id));
        ClearDuplicates();
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
        OnItemChanged?.Invoke(slot.item.Name);
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

    public bool IsItemInInventory(string itemName)
    {
        return inventory.inventory.Exists(x => x.item.Name == itemName);
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
        OnItemChanged?.Invoke(slot.item.Name);
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
        ClearInventoryUI();
        
        if (inventory.inventory.Count == 0)
        {
            return;
        }
        
        if (currentCategory == Category.None)
        {
            ShowCategoryUI();
            return;
        }
        else
        {
            HideCategoryUI();
        }
        SortInventoryByCategory();
        SetInventoryContentSize();
        pos = basePos;
        foreach (InventorySlot slot in inventory.inventory)
        {
            if (slot.item.category != currentCategory)
                continue;
            
            CreateInventorySlotUI(slot);
        }
    }

    private void ClearInventoryUI()
    {
        foreach (Transform child in inventoryUI.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ShowCategoryUI()
    {
        categoryUI.SetActive(true);
    }

    private void HideCategoryUI()
    {
        categoryUI.SetActive(false);
    }

    private void SortInventoryByCategory()
    {
        if (currentCategory == Category.Weapon)
        {
            SortByDamage();
        }
        else if (currentCategory == Category.Material)
        {
            SortByRarity();
        }
        else if (currentCategory == Category.Tools)
        {
            SortByMiningEfficiency();
        }
        else if (currentCategory == Category.Potion)
        {
            SortByRarity();
        }
    }

    private void SortByDamage()
    {
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
            for (int j = i + 1; j < inventory.inventory.Count; j++)
            {
                if (inventory.inventory[i].item.Damage < inventory.inventory[j].item.Damage)
                {
                    SwapInventorySlots(i, j);
                }
            }
        }
    }

    private void SortByRarity()
    {
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
            for (int j = i + 1; j < inventory.inventory.Count; j++)
            {
                if (inventory.inventory[i].item.rarity < inventory.inventory[j].item.rarity)
                {
                    SwapInventorySlots(i, j);
                }
            }
        }
    }

    private void SortByMiningEfficiency()
    {
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
            for (int j = i + 1; j < inventory.inventory.Count; j++)
            {
                if (inventory.inventory[i].item.miningEfficiency < inventory.inventory[j].item.miningEfficiency)
                {
                    SwapInventorySlots(i, j);
                }
            }
        }
    }

    private void SwapInventorySlots(int index1, int index2)
    {
        InventorySlot temp = inventory.inventory[index1];
        inventory.inventory[index1] = inventory.inventory[index2];
        inventory.inventory[index2] = temp;
    }

    private void SetInventoryContentSize()
    {
        int itemCount = CountItemsInCategory();
        RectTransform rt = inventoryContent.GetComponent<RectTransform>();
        float slotHeight = 134;
        float padding = 10;
        rt.sizeDelta = new Vector2(0, Mathf.Abs((slotHeight + padding) * itemCount));
        rt.position = new Vector3(rt.position.x, rt.position.y, rt.position.z);
    }

    private int CountItemsInCategory()
    {
        int itemCount = 0;
        foreach (InventorySlot slot in inventory.inventory)
        {
            if (slot.item.category == currentCategory)
            {
                itemCount++;
            }
        }
        return itemCount;
    }

    private void CreateInventorySlotUI(InventorySlot slot)
    {
        GameObject go = Instantiate(inventorySlotPrefab, inventoryUI.transform);
        go.transform.localPosition = GetNextSlotPosition();
        go.transform.Find("Icon").GetComponent<Image>().sprite = ItemSystem.Instance.GetItemIcon(slot.item.id);
        go.transform.Find("Quantity").GetComponent<Text>().text = UISystem.Instance.NumberFormat(slot.quantity);
        go.transform.Find("Rarity").GetComponent<Text>().text = slot.item.rarity.ToString();
        go.transform.Find("Rarity").GetComponent<Text>().color = UISystem.Instance.GetRarityColor(slot.item.rarity);
        go.transform.Find("Title").GetComponent<Text>().text = slot.item.Name;
        GameObject effect = go.transform.Find("Effect").gameObject;

        if (slot.item.category == Category.Material)
        {
            SetMaterialCategoryButton(go, slot.item);
        }
        else if (slot.item.category == Category.Weapon)
        {
            SetWeaponCategoryButton(go, slot.item);
        }
        else if (slot.item.category == Category.Tools)
        {
            SetToolsCategoryButton(go, slot.item);
        }
        else if (slot.item.category == Category.Potion)
        {
            SetPotionCategoryButton(go, slot.item, slot);
        }

        int divineLevel = UpgradingSystem.Instance.GetDivineLevel(slot.item.Name);
        SetEffectVisibility(effect, divineLevel);
    }

    private Vector3 GetNextSlotPosition()
    {
        float padding = 10;
        float slotHeight = 134;
        pos.y -= slotHeight + padding;
        return pos;
    }

    private void SetMaterialCategoryButton(GameObject go, Items item)
    {
        Button button = go.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        var onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(() => CraftingSystem.Instance.SetCraftSlot(item));
        button.onClick = onClick;
    }

    private void SetWeaponCategoryButton(GameObject go, Items item)
    {
        Button button = go.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        var onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(() => UpgradingSystem.Instance.SetUpgradeSlot(item));
        button.onClick = onClick;

        int upgradeLevel = UpgradingSystem.Instance.GetLevel(item.Name);
        if (upgradeLevel > 0)
            go.transform.Find("Title").GetComponent<Text>().text = $"{item.Name} +{upgradeLevel}";

        go.transform.Find("Quantity").GetComponent<Text>().text = String.Format("Damage: {0}\nDamage Bonus: {1}%", UISystem.Instance.NumberFormat(item.Damage), UISystem.Instance.NumberFormat((item.damageBoostPercentage)));
        go.transform.Find("Equip").gameObject.SetActive(true);
        Button equipButton = go.transform.Find("Equip").GetComponent<Button>();

        if (DamageSystem.Instance.GetWeapon() != null && DamageSystem.Instance.GetWeapon().id == item.id)
        {
            equipButton.transform.Find("Text").GetComponent<Text>().text = "Equiped";
            equipedWeaponButton = equipButton;
        }
        else
        {
            equipButton.transform.Find("Text").GetComponent<Text>().text = "Equip";
        }

        var equipOnClick = new Button.ButtonClickedEvent();
        equipOnClick.AddListener(() => {
            DamageSystem.Instance.EquipWeapon(item);
            equipButton.transform.Find("Text").GetComponent<Text>().text = "Equiped";
            if (equipedWeaponButton != null && equipedWeaponButton != equipButton)
            {
                equipedWeaponButton.transform.Find("Text").GetComponent<Text>().text = "Equip";
            }
            equipedWeaponButton = equipButton;
        });
        equipButton.onClick = equipOnClick;
    }

    private void SetToolsCategoryButton(GameObject go, Items item)
    {
        Button button = go.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        var onClick = new Button.ButtonClickedEvent();
        onClick.AddListener(() => UpgradingSystem.Instance.SetUpgradeSlot(item));
        button.onClick = onClick;

        int upgradeLevel = UpgradingSystem.Instance.GetLevel(item.Name);
        if (upgradeLevel > 0)
            go.transform.Find("Title").GetComponent<Text>().text = $"{item.Name} +{upgradeLevel}";

        go.transform.Find("Quantity").GetComponent<Text>().text = String.Format("Mining Efficiency: {0}", UISystem.Instance.NumberFormat(item.miningEfficiency));
        go.transform.Find("Equip").gameObject.SetActive(true);
        Button equipButton = go.transform.Find("Equip").GetComponent<Button>();

        if (MiningSystem.Instance.GetTool() != null && MiningSystem.Instance.GetTool().id == item.id)
        {
            equipButton.transform.Find("Text").GetComponent<Text>().text = "Equiped";
            equipedToolButton = equipButton;
        }
        else
        {
            equipButton.transform.Find("Text").GetComponent<Text>().text = "Equip";
        }

        var equipOnClick = new Button.ButtonClickedEvent();
        equipOnClick.AddListener(() => {
            MiningSystem.Instance.EquipTool(item);
            equipButton.transform.Find("Text").GetComponent<Text>().text = "Equiped";
            if (equipedToolButton != null && equipedToolButton != equipButton)
            {
                equipedToolButton.transform.Find("Text").GetComponent<Text>().text = "Equip";
            }
            equipedToolButton = equipButton;
        });
        equipButton.onClick = equipOnClick;
    }

    private void SetPotionCategoryButton(GameObject go, Items item, InventorySlot slot)
    {
        if (PotionsSystem.Instance.currentPotion == item)
        {
            go.transform.Find("Title").GetComponent<Text>().text = $"{item.Name} (Active)";
        }

        go.transform.Find("Quantity").GetComponent<Text>().text = String.Format(item.description, item.potionType != Items.PotionType.Health ? item.potionValue * 100 : item.potionValue, item.potionDuration >= 60 ? item.potionDuration / 60  : item.potionDuration, slot.quantity >= 1000 ? UISystem.Instance.NumberFormat(slot.quantity) : slot.quantity.ToString());

        Button use = go.transform.Find("Equip").GetComponent<Button>();
        use.transform.Find("Text").GetComponent<Text>().text = "Use";
        use.onClick.RemoveAllListeners();
        use.onClick.AddListener(() => PotionsSystem.Instance.UsePotion(item));
        use.gameObject.SetActive(true);

        // Button button = go.GetComponent<Button>();
        // button.onClick.RemoveAllListeners();
        // var onClick = new Button.ButtonClickedEvent();
        // onClick.AddListener(() => PotionsSystem.Instance.UsePotion(item));
        // button.onClick = onClick;
    }

    private void SetEffectVisibility(GameObject effect, int divineLevel)
    {
        if (divineLevel > 0)
        {
            if (effect.activeSelf)
            {
                effect.GetComponent<Animator>().Play(animations[divineLevel - 1].name);
            }
            else
            {
                effect.SetActive(true);
            }
        }
        else
        {
            effect.SetActive(false);
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

    public void SetInventory(Inventory inventoryData)
    {
        foreach (InventorySlot slot in inventoryData.inventory)
        {
            if (slot.name == "None" || slot.name == "")
                continue;
            if (ItemSystem.Instance.ItemsCollection.Find(x => x.Name == slot.name) == null)
                continue;
            InventorySlot newSlot = new InventorySlot()
            {
                name = slot.name,
                quantity = slot.quantity
            };
            inventory.inventory.Add(newSlot);
        }
    }

    private void Start() 
    {
        if (equipedWeaponButton != null)
        {
            equipedWeaponButton.transform.Find("Text").GetComponent<Text>().text = "Equipped";
        }
    }
}
