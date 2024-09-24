using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Rune
{
    public string name;
    public double value;
    public string description;
    public int level;
    public int maxLevel; 

    public double baseCost;
    public double costMultiplier;

    public double cost => GetCost();

    public double GetCost()
    {
        if (level == 0)
            return baseCost;
        return baseCost * level * costMultiplier;
    }
    
    public Items.Rarity rarity;

    public Type type;
    public enum Type
    {
        Fire,
        Water,
        Earth,
        Air,
        Light,
        Dark,
    }

    public BonusType bonusType;
    public enum BonusType
    {
        DamageBase,
        DamagePercentage,
        GoldPercentage,
        HealtBase,
        ManaBase,
    }
}

public class RuneData
{
    public Rune rune;
    public int amount;
}

public class RunesSystem : MonoBehaviour
{
    private static RunesSystem _instance;
    public static RunesSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RunesSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("RunesSystem");
                    _instance = go.AddComponent<RunesSystem>();
                }
            }
            return _instance;
        }
    }

    [Header("Runes")]
    public List<Rune> runes = new List<Rune>();

    [Header("UI")]
    [SerializeField] private GameObject runePanel;

    private void LoadRunes()
    {
        foreach (Rune rune in runes)
        {
            rune.level = PlayerPrefs.GetInt(rune.name, 0);
            AddBonus(rune, true);
        }
    }

    private void AddBonus(Rune rune, bool isLoad = false)
    {
        double value = rune.value;
        if (isLoad)
            value = value * rune.level;

        switch (rune.bonusType)
        {
            case Rune.BonusType.DamageBase:
                DamageSystem.Instance.AddDamageBoost(value);
                break;
            case Rune.BonusType.DamagePercentage:
                DifficultySystem.Instance.AddDamagePercentage(value / 100);
                break;
            case Rune.BonusType.GoldPercentage:
                DifficultySystem.Instance.GoldBonus += value / 100;
                break;
            case Rune.BonusType.HealtBase:
                HealthSystem.Instance.AddHealthBoost(value);
                break;
            case Rune.BonusType.ManaBase:
                ManaSystem.Instance.AddManaPerHour(value);
                break;
        }
    }

    private void UpdateUI(int index)
    {
        runePanel.transform.Find("TitleText").GetComponent<Text>().text = runes[index].name;
        runePanel.transform.Find("TitleText").GetComponent<Text>().color = UISystem.Instance.GetRarityColor(runes[index].rarity);
        runePanel.transform.Find("LevelText").GetComponent<Text>().text = $"Level: {runes[index].level}/{runes[index].maxLevel}";

        if (runes[index].level <= 0)
            runePanel.transform.Find("BonusText").GetComponent<Text>().text = String.Format(runes[index].description, runes[index].value);
        else
            runePanel.transform.Find("BonusText").GetComponent<Text>().text = String.Format(runes[index].description, runes[index].value * runes[index].level);
        
        runePanel.transform.Find("CostText").GetComponent<Text>().text = UISystem.Instance.NumberFormat(runes[index].cost);

        runePanel.transform.Find("UpgradeButton").GetComponent<Button>().onClick.RemoveAllListeners();
        runePanel.transform.Find("UpgradeButton").GetComponent<Button>().onClick.AddListener(() => UpgradeRune(index));
    }

    private void UpgradeRune(int index)
    {
        Rune rune = runes[index];
        if(rune.level >= rune.maxLevel)
            return;
        if(ManaSystem.Instance.GetMana() < rune.cost)
            return;
        ManaSystem.Instance.RemoveMana(rune.cost);
        rune.level++;    
        AddBonus(rune);
        PlayerPrefs.SetInt(rune.name, rune.level);
        UpdateUI(index);
        
    }

    public void OpenPanel(int index)
    {
        runePanel.SetActive(true);
        UpdateUI(index);
    }

    private void Start()
    {
        runePanel.SetActive(false);
        LoadRunes();
    }

}
