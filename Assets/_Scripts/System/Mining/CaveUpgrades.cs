using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaveUpgrades : MonoBehaviour
{
    [Header("Upgrade Info")]
    [SerializeField] private int level;
    [SerializeField] private int maxLevel;
    [SerializeField] private double cost;
    [SerializeField] private double costRate;
    [SerializeField] private string resourceName;

    [Header("Damage Upgrade")]
    [SerializeField] private bool isDamageUpgrade;
    [SerializeField] private double damageBoostPercentage;
    [Header("UI")]
    [SerializeField] private Text Title;
    [SerializeField] private Text Cost;
    [SerializeField] private Text Bonus;
    [SerializeField] private Text ProgressText;
    [SerializeField] private Image ProgressImage;


    private void Start()
    {
        if(!PlayerPrefs.HasKey("CaveUpgradeLevel-" + resourceName))
        {
            UIUpdate(); 
        }
        Load();
        UIUpdate();
    }

    private void BonusText()
    {
        if (isDamageUpgrade)
        {
            if (level > 0)
            {
                Bonus.text = "Damage: " + damageBoostPercentage * level + "%";
            }
            else
            {
                Bonus.text = "Damage: 0%";
            }
        }
    }

    private void UIUpdate()
    {
        Title.text = "Level " + level;
        Cost.text = String.Format("Cost: {0}", UISystem.Instance.NumberFormat(cost));
        BonusText();
        ProgressText.text = level + "/" + maxLevel;
    }

    private void Load()
    {
        level = PlayerPrefs.GetInt("CaveUpgradeLevel-" + resourceName, 0);
        cost = cost * Math.Pow(costRate, level);
        if(isDamageUpgrade)
        {
            DifficultySystem.Instance.AddDamagePercentage((damageBoostPercentage / 100) * level);
        }
        UIUpdate();
    }

    public void BuyUpgrade()
    {
        if (InventorySystem.Instance.GetResourceByName(resourceName) >= cost)
        {
            InventorySystem.Instance.RemoveItemByName(resourceName, cost);
            level++;
            cost *= costRate;
            if (isDamageUpgrade)
            {
                DifficultySystem.Instance.AddDamagePercentage((damageBoostPercentage / 100));
                BonusText();    
            }
            UIUpdate();
            PlayerPrefs.SetInt("CaveUpgradeLevel-" + resourceName, level);
        }
    }

}
