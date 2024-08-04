using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemsToCraft
{
    public string itemName;
    public float chance;

}

[System.Serializable]
public class CraftingRecipe
{
    public int id;
    public string materialName;
    public CraftingType craftingType;
    public List<ItemsToCraft> itemsToCraft = new List<ItemsToCraft>();

}

public enum CraftingType
    {
        None,
        Weapon,
        Tools,
    }

public class CraftingRecipesWrapper
{
    public List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();
}

public class CraftingSystem : MonoBehaviour
{
    private static CraftingSystem _instance;
    public static CraftingSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CraftingSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CraftingSystem");
                    _instance = go.AddComponent<CraftingSystem>();
                }
            }
            return _instance;
        }
    }

    [Header("Crafting Recipes")]
    public List<CraftingRecipe> CraftingRecipes = new List<CraftingRecipe>();
    [Header("UI Elements")]
    [SerializeField] private Image craftSlotImage;
    [SerializeField] private GameObject categoryPanel;
    [SerializeField] private Button craftButton;
    [SerializeField] private List<GameObject> chancePanels = new List<GameObject>();

    [Header("Crafting")]
    public Items CraftSlot;
    [SerializeField] private CraftingType currentCraftingType;
    private bool isCrafting = false;

    public void Craft()
    {
        if (CraftSlot == null || currentCraftingType == CraftingType.None)
            return;
        if (isCrafting)
            return;
    
        CraftingRecipe recipe = CraftingRecipes.Find(x => x.materialName == CraftSlot.Name && x.craftingType == currentCraftingType);
        if (InventorySystem.Instance.GetResourceByName(recipe.materialName) <= 1000f)
            return;
        isCrafting = true;
        craftButton.interactable = false;
        StartCoroutine(StartCrafting());
        if (recipe != null)
        {
            foreach (var item in recipe.itemsToCraft)
            {
                if (UnityEngine.Random.Range(0f, 1f) <= item.chance)
                {
                    
                    InventorySystem.Instance.RemoveItemByName(recipe.materialName, 1000);
                    InventorySystem.Instance.AddItemByName(item.itemName, 1);
                    UpdateChancePanel(recipe);
                    return;
                }
            }
        }
        InventorySystem.Instance.RemoveItemByName(recipe.materialName, 1000);
        InventorySystem.Instance.AddItemByName(recipe.itemsToCraft[0].itemName, 1);
        UpdateChancePanel(recipe);
    }

    private void ClearChancePanels()
    {
        foreach (var panel in chancePanels)
        {
            panel.SetActive(false);
        }
    }

    public void SlotOnClick()
    {
        if (!InventorySystem.Instance.InventoryPanel.activeSelf)
        {
            InventorySystem.Instance.SetCategory(1);
            InventorySystem.Instance.InventoryPanel.SetActive(true);
        }

        if (CraftSlot != null)
        {
            CraftSlot = null;
            craftSlotImage.sprite = null;
            
            ClearChancePanels();
        }
    }
    
    public void OpenCategoryPanel()
    {
        categoryPanel.SetActive(true);
    }
    public void SetCraftingType(int type)
    {
        currentCraftingType = (CraftingType)type;
        CraftSlot = null;
        craftSlotImage.sprite = null;
        categoryPanel.SetActive(false);
        ClearChancePanels();
    }
    
    private void UpdateChancePanel(CraftingRecipe recipe)
    {       
            foreach (var panel in chancePanels)
            {
                panel.SetActive(false);
            }

            for (int i = 0; i < recipe.itemsToCraft.Count; i++)
            {
                chancePanels[i].SetActive(true);
                chancePanels[i].transform.GetChild(0).GetComponent<Text>().text = $"{recipe.itemsToCraft[i].chance * 100}%";
                chancePanels[i].GetComponent<Image>().sprite = ItemSystem.Instance.GetItemIconByName(recipe.itemsToCraft[i].itemName);
                
                if(!InventorySystem.Instance.IsItemInInventory(recipe.itemsToCraft[i].itemName))
                    chancePanels[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 1);
                else
                    chancePanels[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);
            }
    }
    public void SetCraftSlot(Items item)
    {
        CraftSlot = item;
        craftSlotImage.sprite = item.icon;

        CraftingRecipe recipe = CraftingRecipes.Find(x => x.materialName == item.Name && x.craftingType == currentCraftingType);
        if (recipe != null)  
            UpdateChancePanel(recipe);
    }
    public void SetCraftingRecipes(List<CraftingRecipe> craftingRecipes)
    {
        CraftingRecipes = craftingRecipes;
    } 

    private void Start()
    {
        LoadGameData.Instance.CraftingRecipes();
    }

    private IEnumerator StartCrafting()
    {
        yield return new WaitForSeconds(.25f);
        isCrafting = false;
        craftButton.interactable = true;
    }
}
