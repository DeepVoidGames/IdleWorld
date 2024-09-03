using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RarityColor
{
    public string rarity;
    public Color color;
}

[System.Serializable]
public class RarityColors
{
    public List<RarityColor> rarityColors = new List<RarityColor>();
}

public class UISystem : MonoBehaviour {
    private static UISystem _instance;
    public static UISystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UISystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("UISystem");
                    _instance = go.AddComponent<UISystem>();
                }
            }
            return _instance;
        }
    }

    [Header("UI System")]
    [SerializeField] private RarityColors rarityColors;

    [Header("Damage System UI")]
    [SerializeField] private Text damageText;
    [SerializeField] private Text dpsText;

    [Header("Level System UI")]
    [SerializeField] private Text levelText;
    [SerializeField] private Text stageText;
    [SerializeField] private Text prestigeLevelText;
    [SerializeField] private Text prestigeRequirementText;
    [Header("Gold System UI")]
    [SerializeField] private Text goldText;
    [SerializeField] private GameObject goldPanel;
    [Header("Mining System UI")]
    [SerializeField] private Text miningLevelText;
    [SerializeField] private Text miningExperienceText;
    [SerializeField] private Text miningEfficiencyText;

    [Header("Stats UI")]
    [SerializeField] private Text statsText;

    [Header("Potions System UI")]
    [SerializeField] private Text potionText;
    [SerializeField] private Image potionIcon;

    public void UpdateLevelText()
    {
        levelText.text = String.Format("Slayer Level: {0}", NumberFormatInt(LevelSystem.Instance.Level));
        stageText.text = String.Format("Stage: {0}", NumberFormatInt(LevelSystem.Instance.Stage));
        damageText.text = String.Format("Damage: {0}", NumberFormat(DamageSystem.Instance.Damage));
        dpsText.text = String.Format("Hero Damage: {0}/s", NumberFormat(DifficultySystem.Instance.GetDPS()));

        prestigeLevelText.text = $"Prestige Level: {NumberFormatInt(LevelSystem.Instance.PrestigeLevel)}/{NumberFormatInt(LevelSystem.Instance.MaxPrestigeLevel)}\nMonster HP x{LevelSystem.Instance.PrestigeLevel + 1}";
        prestigeRequirementText.text = $"Required Level: {NumberFormatInt(LevelSystem.Instance.LevelToPrestige)}";
    }

    public void MoneyIndicator(double money, string prefix = "+")
    {
        goldPanel.GetComponent<MessageSpawner>().SpawnMessage(prefix + NumberFormat(money));
    }

    public void UpdateGoldText()
    {
        goldText.text = NumberFormat(GoldSystem.Instance.Gold);
    }

    public void UpdateMiningUI()
    {
        miningLevelText.text = String.Format("Mining Level: {0}", NumberFormat(MiningSystem.Instance.MiningLevel));
        miningExperienceText.text = String.Format("Experience: {0} / {1}", NumberFormat(MiningSystem.Instance.MiningExperience), NumberFormat(DifficultySystem.Instance.GetMiningExperienceNeeded()));
        miningEfficiencyText.text = String.Format("Efficiency: {0}", NumberFormat(MiningSystem.Instance.MiningEfficiency));
    }

    public void UpdateStatsText()
    {
        statsText.text = 
                        $"Damage bonus: {NumberFormat(DifficultySystem.Instance.GetDamagePercentage() * 100)}%\n"+
                        $"Gold bonus: {NumberFormat(DifficultySystem.Instance.GoldBonus * 100)}%\n"+
                        $"Mining Efficiency bonus: + {NumberFormat(DifficultySystem.Instance.MiningBonusMiningEfficiency)}\n"+
                        $"Mining Efficiency bonus: {NumberFormat(DifficultySystem.Instance.MiningEfficiencyPercentage*100)}%\n"+
                        $"Mining Drop Rate: {DifficultySystem.Instance.MiningDropRateMultiplier}\n"+
                        $"Highest Level: {LevelSystem.Instance.HighestLevel}\n"+
                        $"Highest Stage: {LevelSystem.Instance.HighestStage}\n";
    }

    public void UpdatePotionUI()
    {
        // Check if there is a potion active
        if (PotionsSystem.Instance.currentPotion != null)
        {
            potionText.text = $"{(PotionsSystem.Instance.Timer / 60).ToString("F1")}m";
            potionIcon.sprite = ItemSystem.Instance.GetItemIcon(PotionsSystem.Instance.currentPotion.id);
            potionIcon.gameObject.SetActive(true);
        }
        else
        {
            potionText.text = "";
            potionIcon.gameObject.SetActive(false);
            potionIcon.sprite = null;
        }
    }

    public void LoadUI()
    {
        UpdateLevelText();
        UpdateGoldText();
        UpdateMiningUI();
    }

    public string NumberFormat(double number)
    {
        if (number < 1e3)
        {
            return number.ToString("F2");
        }

        string[] commonSuffixes = { "K", "M", "B" };
        var suffixes = GenerateSuffixes();

        double divisor = 1;
        int commonSuffixCount = commonSuffixes.Length;
        int suffixIndex = 0;

        for (int exp = 3; exp <= 300; exp += 3)
        {
            divisor = Math.Pow(10, exp);

            if (number < divisor * 1000)
            {
                if (suffixIndex < commonSuffixCount)
                {
                    return (number / divisor).ToString("F2") + commonSuffixes[suffixIndex];
                }
                else
                {
                    return (number / divisor).ToString("F2") + suffixes[suffixIndex - commonSuffixCount];
                }
            }
            suffixIndex++;
        }
        
        // In case the number is larger than all defined ranges
        return (number / divisor).ToString("F2") + suffixes[suffixIndex - commonSuffixCount];
    }

    private List<string> GenerateSuffixes()
    {
        var suffixes = new List<string>();
        // Double letter suffixes
        for (char first = 'A'; first <= 'Z'; first++)
        {
            for (char second = 'A'; second <= 'Z'; second++)
            {
                suffixes.Add(first.ToString() + second.ToString());
            }
        }
        return suffixes;
    }
    
    public string NumberFormatInt(int number)
    {
        if (number >= 1000000000)
        {
            return (number / 1000000000.0).ToString("F2") + "B";
        }
        if (number >= 1000000)
        {
            return (number / 1000000.0).ToString("F2") + "M";
        }
        if (number >= 1000)
        {
            return (number / 1000.0).ToString("F2") + "K";
        }
        return number.ToString("N0");
    }

    public Color GetRarityColor(Items.Rarity rarity)
    {
        return rarityColors.rarityColors.Find(x => x.rarity == rarity.ToString()).color;
    }
}