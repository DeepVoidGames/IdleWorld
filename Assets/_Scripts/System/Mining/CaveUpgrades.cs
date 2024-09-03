using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CaveUpgrades : MonoBehaviour
{
    [Header("Upgrade Info")]
    [SerializeField] private string upgradeName;
    [SerializeField] private int level;
    [SerializeField] private int maxLevel;
    [SerializeField] private double baseCost;
    private double cost;
    [SerializeField] private double costRate;
    [SerializeField] private string resourceName;
    [SerializeField] private bool useGold;
    [SerializeField] private double boostValue;

    [SerializeField] private UpgradeType upgradeType;
    private enum UpgradeType
    {
        DamagePercentage,
        DamageBase,
        MiningEfficiencyPercentage,
        MiningEfficiencyBase,
        MiningDropRateMultiplier,
        HealthBoost
    }


    [Header("UI")]
    [SerializeField] private GameObject UpgradePanel;
    private Text Title;
    private Text Cost;
    private Text Bonus;
    private Text ProgressText;
    private Image ProgressImage;
    private Button BuyButton;


    private Coroutine buyCoroutine;

    private void Start()
    {
        // PlayerPrefs.DeleteKey("CaveUpgradeLevel-" + resourceName);
        Title = UpgradePanel.transform.Find("Title").GetComponent<Text>();
        Cost = UpgradePanel.transform.Find("CostPanel").Find("CostText").GetComponent<Text>();
        Bonus = UpgradePanel.transform.Find("Bonus").GetComponent<Text>();
        ProgressText = UpgradePanel.transform.Find("ProgressText").GetComponent<Text>();
        ProgressImage = UpgradePanel.transform.Find("ProgressImage").GetComponent<Image>();
        BuyButton = UpgradePanel.transform.Find("BuyButton").GetComponent<Button>();

        if(Title == null || Cost == null || Bonus == null || ProgressText == null || ProgressImage == null || BuyButton == null)
        {
            Debug.LogError("UI Elements not found");
            return;
        }

        if (BuyButton != null)
        {
            BuyButton.onClick.RemoveAllListeners();
            EventTrigger trigger = BuyButton.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = BuyButton.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
            trigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
            trigger.triggers.Add(pointerUpEntry);

        }

        Load();
        UIUpdate();
    }

    private void BonusText()
    {
        if (upgradeType == UpgradeType.DamagePercentage)
        {
            if (level > 0)
            {
                Bonus.text = "Damage: " + (boostValue * level) * 100 + "%";
            }
            else
            {
                Bonus.text = "Damage: 0%";
            }
        }
        else if (upgradeType == UpgradeType.MiningEfficiencyPercentage)
        {
            if (level > 0)
            {
                Bonus.text = "Mining Efficiency: " + (boostValue * level) * 100 + "%";
            }
            else
            {
                Bonus.text = "Mining Efficiency: 0%";
            }
        }
        else if (upgradeType == UpgradeType.MiningDropRateMultiplier)
        {
            if (level > 0)
            {
                Bonus.text = "Ore Drop Rate Multiplier: " + boostValue * level + "";
            }
            else
            {
                Bonus.text = "Ore Drop Rate Multiplier: 0";
            }
        }
        else if (upgradeType == UpgradeType.MiningEfficiencyBase)
        {
            if (level > 0)
            {
                Bonus.text = "Mining Efficiency: " + boostValue * level;
            }
            else
            {
                Bonus.text = "Mining Efficiency: 0";
            }
        }
        else if (upgradeType == UpgradeType.HealthBoost)
        {
            if (level > 0)
            {
                Bonus.text = "Health Boost: " + boostValue * level;
            }
            else
            {
                Bonus.text = "Health Boost: 0";
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
        // Image min size 0, max 538.88
        // Image min X -381.63, max -112.19
        float progress = (float)level / maxLevel;
        ProgressImage.rectTransform.sizeDelta = new Vector2(538.88f * progress, ProgressImage.rectTransform.sizeDelta.y);
        ProgressImage.rectTransform.localPosition = new Vector3(-381.63f + (progress * 269.44f), ProgressImage.rectTransform.localPosition.y, ProgressImage.rectTransform.localPosition.z);
    }

    private void Load()
    {
        level = PlayerPrefs.GetInt("CaveUpgradeLevel-" + resourceName, 0);

        if (level > maxLevel)
        {
            level = maxLevel;
        }

        if (level == 0)
        {
            cost = (baseCost * costRate);
            return;
        }
        else
        {
            cost = CalculateCost(); 
        }
        
        if(upgradeType == UpgradeType.DamagePercentage)
        {
            DifficultySystem.Instance.AddDamagePercentage(boostValue * level);
        }
        if (upgradeType == UpgradeType.MiningEfficiencyPercentage)
        {
            DifficultySystem.Instance.AddMiningEfficiencyPercentage(boostValue * level);
        }
        if (upgradeType == UpgradeType.MiningDropRateMultiplier)
        {
            DifficultySystem.Instance.AddMiningDropRateMultiplier(boostValue * level);
        }
        if (upgradeType == UpgradeType.MiningEfficiencyBase)
        {
            DifficultySystem.Instance.MiningBonusMiningEfficiency += boostValue * level;
        }
        if (upgradeType == UpgradeType.HealthBoost)
        {
            HealthSystem.Instance.AddHealthBoost(boostValue * level);
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
        if (upgradeType == UpgradeType.DamagePercentage)
        {
            DifficultySystem.Instance.AddDamagePercentage(boostValue);  
        }
        else if (upgradeType == UpgradeType.MiningEfficiencyPercentage)
        {
            DifficultySystem.Instance.AddMiningEfficiencyPercentage(boostValue);
        }
        else if (upgradeType == UpgradeType.MiningDropRateMultiplier)
        {
            DifficultySystem.Instance.AddMiningDropRateMultiplier(boostValue);
        }
        else if (upgradeType == UpgradeType.MiningEfficiencyBase)
        {
            DifficultySystem.Instance.MiningBonusMiningEfficiency += boostValue;
        }
        else if (upgradeType == UpgradeType.HealthBoost)
        {
            HealthSystem.Instance.AddHealthBoost(boostValue);
        }
        BonusText();
        UIUpdate();
        PlayerPrefs.SetInt("CaveUpgradeLevel-" + resourceName, level);
    }

    private void OnPointerDown(PointerEventData data)
    {
        buyCoroutine = StartCoroutine(ContinuousBuy());
    }

    private void OnPointerUp(PointerEventData data)
    {
        if (buyCoroutine != null)
        {
            StopCoroutine(buyCoroutine);
            buyCoroutine = null;
        }
    }

    private IEnumerator ContinuousBuy()
    {
        float waitTime = 0.2f; // Initial wait time
        float minWaitTime = 0.1f; // Minimum wait time
        float speedUpFactor = 0.01f; // Factor to speed up the upgrade

        while (true)
        {
            if (InventorySystem.Instance.GetResourceByName(resourceName) < cost && !useGold)
            {
                yield break; // Stop the coroutine if out of resources
            }

            BuyUpgrade();
            yield return new WaitForSeconds(waitTime);
            waitTime = Mathf.Max(minWaitTime, waitTime * speedUpFactor); // Decrease wait time but not below minWaitTime
        }
    }
}
