using System;
using UnityEngine;
using UnityEngine.UI;

public class CaveUpgrades : MonoBehaviour
{
    [Header("Upgrade Info")]
    [SerializeField] private string upgradeName;
    [SerializeField] private int level;
    [SerializeField] private int maxLevel;
    [SerializeField] private double baseCost;
    [SerializeField] private double costRate;
    [SerializeField] private string resourceName;
    [SerializeField] private bool useGold;
    private double cost;


    [Header("Damage Upgrade")]
    [SerializeField] private bool isDamageUpgrade;
    [SerializeField] private double damageBoostPercentage;

    [Header("Resource Upgrade")]
    [SerializeField] private bool isMiningEfficiencyUpgrade;
    [SerializeField] private double miningEfficiencyBoostPercentage;

    [Header("Mining Drop Rate Upgrade")]
    [SerializeField] private bool isMiningDropRateUpgrade;
    [SerializeField] private double miningDropRateBoostMultiplier;

    [Header("Mining Efficiency Upgrade Base")]
    [SerializeField] private bool isMiningEfficiencyUpgradeBase;
    [SerializeField] private double miningEfficiencyBoostBase;

    [Header("UI")]
    [SerializeField] private Text Title;
    [SerializeField] private Text Cost;
    [SerializeField] private Text Bonus;
    [SerializeField] private Text ProgressText;
    [SerializeField] private Image ProgressImage;
    [SerializeField] private Button BuyButton;


    private void Start()
    {
        // PlayerPrefs.DeleteKey("CaveUpgradeLevel-" + resourceName);
        Load();
        UIUpdate();
    }

    private void BonusText()
    {
        if (isDamageUpgrade)
        {
            if (level > 0)
            {
                Bonus.text = "Damage: " + (damageBoostPercentage * level) * 100 + "%";
            }
            else
            {
                Bonus.text = "Damage: 0%";
            }
        }
        else if (isMiningEfficiencyUpgrade)
        {
            if (level > 0)
            {
                Bonus.text = "Mining Efficiency: " + (miningEfficiencyBoostPercentage * level) * 100 + "%";
            }
            else
            {
                Bonus.text = "Mining Efficiency: 0%";
            }
        }
        else if (isMiningDropRateUpgrade)
        {
            if (level > 0)
            {
                Bonus.text = "Mining Drop Rate Multiplier: " + miningDropRateBoostMultiplier * level + "";
            }
            else
            {
                Bonus.text = "Mining Drop Rate Multiplier: 0";
            }
        }
        else if (isMiningEfficiencyUpgradeBase)
        {
            if (level > 0)
            {
                Bonus.text = "Mining Efficiency: " + miningEfficiencyBoostBase * level;
            }
            else
            {
                Bonus.text = "Mining Efficiency: 0";
            }
        }
    }

    private void UIUpdate()
    {
        Title.text = String.Format("{0} Level {1}", upgradeName, level);
        Cost.text = String.Format("Cost: {0}", UISystem.Instance.NumberFormat(cost));
        BonusText();
        ProgressText.text = level + "/" + maxLevel;
        ProgressBar();

        if (level >= maxLevel)
        {
            BuyButton.interactable = false;
            Text text = BuyButton.GetComponentInChildren<Text>();
            text.text = "Max";
            Cost.text = "Max Level";
        }
    }

    private void ProgressBar()
    {
        // Image min size 0, max 1055.4
        // Image min X -672.7001, max -145
        float progress = (float)level / maxLevel;
        ProgressImage.rectTransform.sizeDelta = new Vector2(1055.4f * progress, ProgressImage.rectTransform.sizeDelta.y);
        ProgressImage.rectTransform.anchoredPosition = new Vector2(-672.7001f + (progress * 527.7f), ProgressImage.rectTransform.anchoredPosition.y);
    }

    private void Load()
    {
        level = PlayerPrefs.GetInt("CaveUpgradeLevel-" + resourceName, 0);
        if (level == 0)
        {
            cost = (baseCost * costRate);
            return;
        }
        else
        {
            cost = CalculateCost(); 
        }
        
        if(isDamageUpgrade)
        {
            DifficultySystem.Instance.AddDamagePercentage(damageBoostPercentage * level);
        }
        if (isMiningEfficiencyUpgrade)
        {
            DifficultySystem.Instance.AddMiningEfficiencyPercentage(miningEfficiencyBoostPercentage * level);
        }
        if (isMiningDropRateUpgrade)
        {
            DifficultySystem.Instance.AddMiningDropRateMultiplier(miningDropRateBoostMultiplier * level);
        }
        if (isMiningEfficiencyUpgradeBase)
        {
            DifficultySystem.Instance.MiningBonusMiningEfficiency += miningEfficiencyBoostBase * level;
        }
        UIUpdate();
    }

    private double CalculateCost()
    {
        return (baseCost * costRate) * level;
    }

    public void BuyUpgrade()
    {
        if (level >= maxLevel)
        {
            BuyButton.interactable = false;
            return;
        }

        if (InventorySystem.Instance.GetResourceByName(resourceName) >= cost && !useGold)
        {
            InventorySystem.Instance.RemoveItemByName(resourceName, cost);
        }
        else if (useGold && GoldSystem.Instance.Gold >= cost)
        {
            GoldSystem.Instance.SpendGold(cost);
        }
        else
        {
            return;
        }

        level++;
        cost = CalculateCost();
        if (isDamageUpgrade)
        {
            DifficultySystem.Instance.AddDamagePercentage(damageBoostPercentage * level);
            BonusText();    
        }
        else if (isMiningEfficiencyUpgrade)
        {
            DifficultySystem.Instance.AddMiningEfficiencyPercentage(miningEfficiencyBoostPercentage * level);
            BonusText();
        }
        else if (isMiningDropRateUpgrade)
        {
            DifficultySystem.Instance.AddMiningDropRateMultiplier(miningDropRateBoostMultiplier * level);
            BonusText();
        }
        else if (isMiningEfficiencyUpgradeBase)
        {
            DifficultySystem.Instance.MiningBonusMiningEfficiency += miningEfficiencyBoostBase;
            BonusText();
        }
        
        UIUpdate();
        PlayerPrefs.SetInt("CaveUpgradeLevel-" + resourceName, level);
    }
}
