using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIElement : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPanel;
    [SerializeField]
    private string itemName;
    [Header("UI Elements")]
    public Text itemNameText;
    public Text itemAmountText;

    private void Start()
    {
        if (InventorySystem.Instance.items.ContainsKey(itemName) && InventorySystem.Instance.items[itemName].amount > 0)
        {
            itemPanel.SetActive(true);
        }
        else
        {
            itemPanel.SetActive(false);
        }
        itemNameText.text = itemName;
        UpdateAmount();
    }

    private void Update() {
        UpdateAmount();
    }

    private void UpdateAmount()
    {
        if (InventorySystem.Instance.items.ContainsKey(itemName))
        {
            itemAmountText.text = InventorySystem.Instance.items[itemName].amount.ToString();
        }
        else
        {
            itemAmountText.text = "0";
        }
    }

    public void SellItem()
    {
        if (InventorySystem.Instance.items.ContainsKey(itemName))
        {
            InventoryItem item = InventorySystem.Instance.items[itemName];
            MoneySystem.Instance.AddMoney(item.sellPrice * item.amount);
            InventorySystem.Instance.RemoveItem(itemName, item.amount);
            SaveSystem.Instance.Save();
        }
    }
}
