using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameDataManager : MonoBehaviour 
{
    private static GameDataManager _instance;
    public static GameDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameDataManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameDataManager");
                    _instance = go.AddComponent<GameDataManager>();
                }
            }
            return _instance;
        }
    }
    [SerializeField] private Items itemToAdd;

    [SerializeField] private CraftingRecipe craftingRecipeToAdd;

    public void Add()
    {

        if (itemToAdd.id != -1)
        {
            // Check if the item is already in the list update it
            Items item = ItemSystem.Instance.ItemsCollection.Find(x => x.id == itemToAdd.id);
            if (item != null)
            {
                item.Name = itemToAdd.Name;
                item.id = itemToAdd.id;
                item.icon = itemToAdd.icon;
                item.category = itemToAdd.category;
                item.rarity = itemToAdd.rarity;
                item.damage = itemToAdd.damage;
                item.damageBoostPercentage = itemToAdd.damageBoostPercentage;
            }
            else
            {
                ItemSystem.Instance.ItemsCollection.Add(itemToAdd);
            }
            File.WriteAllText(Path.Combine(Application.dataPath, "Resources/GameData/items.json"), JsonUtility.ToJson(item));            
        }        
    }

}