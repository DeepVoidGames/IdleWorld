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

    [Header("Level System UI")]
    [SerializeField] private Text levelText;
    [SerializeField] private Text stageText;
    [Header("Gold System UI")]
    [SerializeField] private Text goldText;
    [Header("Mining System UI")]
    [SerializeField] private Text miningLevelText;
    [SerializeField] private Text miningExperienceText;
    [SerializeField] private Text miningEfficiencyText;

    public void UpdateLevelText()
    {
        levelText.text = String.Format("Slayer Level: {0}", NumberFormatInt(LevelSystem.Instance.Level));
        stageText.text = String.Format("Stage: {0}", NumberFormatInt(LevelSystem.Instance.Stage));
        damageText.text = String.Format("Damage: {0}", NumberFormat(DamageSystem.Instance.Damage));
    }

    public void UpdateGoldText()
    {
        goldText.text = NumberFormat(GoldSystem.Instance.Gold);
    }

    public void UpdateMiningUI()
    {
        miningLevelText.text = String.Format("Mining Level: {0}", MiningSystem.Instance.MiningLevel);
        miningExperienceText.text = String.Format("Experience: {0} / {1}", NumberFormat(MiningSystem.Instance.MiningExperience), NumberFormat(DifficultySystem.Instance.GetMiningExperienceNeeded()));
        miningEfficiencyText.text = String.Format("Efficiency: {0}", NumberFormat(MiningSystem.Instance.MiningEfficiency));
    }

    public void LoadUI()
    {
        UpdateLevelText();
        UpdateGoldText();
        UpdateMiningUI();
    }

    public string NumberFormat(float number)
    {
            if (number >= 1e38f)
            {
                return (number / 1e38f).ToString("F2") + "Z";
            }
            if (number >= 1e33f)
            {
                return (number / 1e33f).ToString("F2") + "E";
            }
            if (number >= 1e30f)
            {
                return (number / 1e30f).ToString("F2") + "N";
            }
            if (number >= 1e27f)
            {
                return (number / 1e27f).ToString("F2") + "O";
            }
            if (number >= 1e24f)
            {
                return (number / 1e24f).ToString("F2") + "Y";
            }
            if (number >= 1e21f)
            {
                return (number / 1e21f).ToString("F2") + "Z";
            }
            if (number >= 1e18f)
            {
                return (number / 1e18f).ToString("F2") + "E";
            }
            if (number >= 1e15f)
            {
                return (number / 1e15f).ToString("F2") + "P";
            }
            if (number >= 1e12f)
            {
                return (number / 1e12f).ToString("F2") + "T";
            }
            if (number >= 1e9f)
            {
                return (number / 1e9f).ToString("F2") + "B";
            }
            if (number >= 1e6f)
            {
                return (number / 1e6f).ToString("F2") + "M";
            }
            if (number >= 1e3f)
            {
                return (number / 1e3f).ToString("F2") + "K";
            }
            return number.ToString("F2");
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