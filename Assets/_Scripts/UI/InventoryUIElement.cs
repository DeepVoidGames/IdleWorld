using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIElement : MonoBehaviour
{
    [SerializeField]
    private string itemName;
    [Header("UI Elements")]
    public Text itemNameText;
    public Text itemAmountText;

    private void Start()
    {
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
}
