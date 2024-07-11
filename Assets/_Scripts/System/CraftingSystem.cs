using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemsToCraft
{
    public int itemID;
    public float chanceToCraft;

}

[System.Serializable]
public class CraftingRecipe
{
    public int id;
    public int material1ID;
    public int material2ID;
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
    private bool isCrafting;

    [Header("UI")]
    [SerializeField] private GameObject craftingOptionUI;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private Image slot1;
    [SerializeField] private Image slot2;
    [SerializeField] private Image CraftedPrev;
    [SerializeField] private Button craftButton;

    private void UpdateUI()
    {
        if (currentCraftingType == CraftingType.None)
        {
            craftingOptionUI.SetActive(true);
            craftingUI.SetActive(false);
            return;
        }
        else
        {
            craftingOptionUI.SetActive(false);
            craftingUI.SetActive(true);
        }

        if (CraftSlot.Count == 1)
        {
            slot1.sprite = ItemSystem.Instance.GetItemIcon(CraftSlot[CraftSlot.Count - 1].id);
        }
        else if (CraftSlot.Count == 2)
        {
            slot2.sprite = ItemSystem.Instance.GetItemIcon(CraftSlot[CraftSlot.Count - 1].id);
        }
        else
        {
            slot1.sprite = null;
            slot2.sprite = null;
        }
    }

    public void SetCraftingType(int type)
    {
        currentCraftingType = (CraftingType)type;
        if (currentCraftingType == CraftingType.None)
        {
            CraftSlot.Clear();
        }
        UpdateUI();
    }

    public void SetCraftSlot(int id)
    {
        if (currentCraftingType == CraftingType.None) return;
        if(CraftSlot.Count == 0)
        {
            CraftSlot.Add(ItemSystem.Instance.ItemsCollection.Find(x => x.id == id));
            
        }
        else if (CraftSlot.Count == 1)
        {
            CraftSlot.Add(ItemSystem.Instance.ItemsCollection.Find(x => x.id == id));
        }
        else{
            CraftSlot.Clear();
            CraftSlot.Add(ItemSystem.Instance.ItemsCollection.Find(x => x.id == id));
        }
        UpdateUI();
        return;
    }

    public void RemoveFromSlot(int id)
    {
        if(CraftSlot.Count == 0) return;
        if (id == 0)
        {
            slot1.sprite = null;
            CraftSlot.RemoveAt(0);
        }
        else if (id == 1)
        {
            if (CraftSlot.Count == 1)
            {
                slot2.sprite = null;
                CraftSlot.RemoveAt(0);
                return;
            }
            slot2.sprite = null;
            CraftSlot.RemoveAt(1);
        }
    }

    public void Craft()
    {
        if (CraftSlot.Count < 2) return;
        if (currentCraftingType == CraftingType.None) return;
        if (CraftingRecipes.Count == 0) return;
        craftButton.interactable = false;
        isCrafting = true;
        for (int i = 0; i < CraftingRecipes.Count; i++)
        {
            if (CraftingRecipes[i].craftingType != currentCraftingType) continue;
            if ((CraftingRecipes[i].material1ID == CraftSlot[0].id && CraftingRecipes[i].material2ID == CraftSlot[1].id) ||
                (CraftingRecipes[i].material1ID == CraftSlot[1].id && CraftingRecipes[i].material2ID == CraftSlot[0].id))
            {
                for (int j = 0; j < CraftingRecipes[i].itemsToCraft.Count; j++)
                {
                    float random = UnityEngine.Random.Range(0f, 1f);
                    Debug.Log(String.Format("Random: {0}, Chance: {1}, IF {2}", random, CraftingRecipes[i].itemsToCraft[j].chanceToCraft, random <= CraftingRecipes[i].itemsToCraft[j].chanceToCraft));
                    if (random <= CraftingRecipes[i].itemsToCraft[j].chanceToCraft)
                    {
                        InventorySystem.Instance.AddItem(CraftingRecipes[i].itemsToCraft[j].itemID, 1);
                        // CraftSlot.Clear();
                        UpdateUI();
                        CraftedPrev.sprite = ItemSystem.Instance.GetItemIcon(CraftingRecipes[i].itemsToCraft[j].itemID);
                        CraftedPrev.gameObject.SetActive(true);
                        isCrafting = false;
                        return;
                    }
                }
                InventorySystem.Instance.AddItem(CraftingRecipes[i].itemsToCraft[0].itemID, 1);
                UpdateUI();
                CraftedPrev.sprite = ItemSystem.Instance.GetItemIcon(CraftingRecipes[i].itemsToCraft[0].itemID);
                CraftedPrev.gameObject.SetActive(true);
                isCrafting = false;
                return;
                
            }
        }
        isCrafting = false;
    }
    
    public void SetCraftingRecipes(List<CraftingRecipe> craftingRecipes)
    {
        CraftingRecipes = craftingRecipes;
    }

    public CraftingRecipe GetCraftingRecipe(int id)
    {
        return CraftingRecipes.Find(x => x.id == id);
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if(CraftedPrev.gameObject.activeSelf || isCrafting)
        {
           craftButton.interactable = false;
        }
        else
        {
            craftButton.interactable = true;
        }
    }
}
