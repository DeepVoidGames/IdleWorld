using System;
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
    public string material1Name;
    public string material2Name;
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

    public List<CraftingRecipe> CraftingRecipes = new List<CraftingRecipe>();

    [Header("Crafting")]
    public List<Items> CraftSlot = new List<Items>();
    [SerializeField] private CraftingType currentCraftingType;
    private bool isCrafting = false;

    public void SlotOnClick()
    {
        if (!InventorySystem.Instance.InventoryPanel.activeSelf)
        {
            InventorySystem.Instance.SetCategory(1);
            InventorySystem.Instance.InventoryPanel.SetActive(true);
        }
    }
    
    public void SetCraftSlot(Items item)
    {
        CraftSlot.Add(item);
    }
    public void SetCraftingRecipes(List<CraftingRecipe> craftingRecipes)
    {
        CraftingRecipes = craftingRecipes;
    } 

    private void Start()
    {
        LoadGameData.Instance.CraftingRecipes();
    }
}
