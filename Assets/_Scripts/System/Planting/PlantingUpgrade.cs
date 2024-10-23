using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantingUpgrade : MonoBehaviour
{
    [Header("Upgrade Info")]
    [SerializeField] private string upgradeName;
    [SerializeField] private string description;
    [SerializeField] private int level;
    [SerializeField] private int maxLevel;
    private double cost;
    [SerializeField] private double baseCost;
    [SerializeField] private double costRate;
    [SerializeField] private string resourceName;
    [SerializeField] private double boostValue;
    [SerializeField] private UpgradeType upgradeType;
    private enum UpgradeType
    {
        PlantingFortune,
    }

    [Header("UI")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text costText;
    [SerializeField] private Text desctiptionText;
    [SerializeField] private Button buyButton;



    private void UIUpdate()
    {
        titleText.text = upgradeName + " " + level + "/" + maxLevel;
        costText.text = "Cost: " + cost + " " + resourceName;
        desctiptionText.text = String.Format(description, boostValue * level);

        if(level >= maxLevel)
        {
            buyButton.interactable = false;
            Text text = buyButton.GetComponentInChildren<Text>();
            text.text = "Max";
            buyButton.GetComponent<Image>().color = UISystem.Instance.buyButtonMaxedColor;
        }
        else if (InventorySystem.Instance.GetResourceByName(resourceName) < cost)
        {
            buyButton.interactable = false;
            buyButton.GetComponentInChildren<Text>().text = "Not Enough";
            buyButton.GetComponent<Image>().color = UISystem.Instance.buyButtonDisabledColor;
        }
        else
        {
            buyButton.GetComponentInChildren<Text>().text = "Buy";
            buyButton.interactable = true;
            buyButton.GetComponent<Image>().color = UISystem.Instance.buyButtonColor;
        } 
       

    }

    private void OnResourceChanged(string resourceName)
    {
        if (resourceName == this.resourceName)
        {
            UIUpdate();
        }
    }

    private void Load()
    {
        cost = baseCost * costRate * level;
        UIUpdate();
    }

    private void Start() 
    {
        Load();
        InventorySystem.Instance.OnItemChanged += OnResourceChanged;
    }
}
