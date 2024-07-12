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

    public string NumberFormat(double number)
    {
        if (number >= 1e308)
        {
            return (number / 1e308).ToString("F2") + "G";
        }
        if (number >= 1e303)
        {
            return (number / 1e303).ToString("F2") + "V";
        }
        if (number >= 1e300)
        {
            return (number / 1e300).ToString("F2") + "W";
        }
        if (number >= 1e297)
        {
            return (number / 1e297).ToString("F2") + "X";
        }
        if (number >= 1e294)
        {
            return (number / 1e294).ToString("F2") + "Y";
        }
        if (number >= 1e291)
        {
            return (number / 1e291).ToString("F2") + "Z";
        }
        if (number >= 1e288)
        {
            return (number / 1e288).ToString("F2") + "U";
        }
        if (number >= 1e285)
        {
            return (number / 1e285).ToString("F2") + "S";
        }
        if (number >= 1e282)
        {
            return (number / 1e282).ToString("F2") + "R";
        }
        if (number >= 1e279)
        {
            return (number / 1e279).ToString("F2") + "Q";
        }
        if (number >= 1e276)
        {
            return (number / 1e276).ToString("F2") + "P";
        }
        if (number >= 1e273)
        {
            return (number / 1e273).ToString("F2") + "O";
        }
        if (number >= 1e270)
        {
            return (number / 1e270).ToString("F2") + "N";
        }
        if (number >= 1e267)
        {
            return (number / 1e267).ToString("F2") + "M";
        }
        if (number >= 1e264)
        {
            return (number / 1e264).ToString("F2") + "L";
        }
        if (number >= 1e261)
        {
            return (number / 1e261).ToString("F2") + "K";
        }
        if (number >= 1e258)
        {
            return (number / 1e258).ToString("F2") + "J";
        }
        if (number >= 1e255)
        {
            return (number / 1e255).ToString("F2") + "I";
        }
        if (number >= 1e252)
        {
            return (number / 1e252).ToString("F2") + "H";
        }
        if (number >= 1e249)
        {
            return (number / 1e249).ToString("F2") + "G";
        }
        if (number >= 1e246)
        {
            return (number / 1e246).ToString("F2") + "F";
        }
        if (number >= 1e243)
        {
            return (number / 1e243).ToString("F2") + "E";
        }
        if (number >= 1e240)
        {
            return (number / 1e240).ToString("F2") + "D";
        }
        if (number >= 1e237)
        {
            return (number / 1e237).ToString("F2") + "C";
        }
        if (number >= 1e234)
        {
            return (number / 1e234).ToString("F2") + "B";
        }
        if (number >= 1e231)
        {
            return (number / 1e231).ToString("F2") + "A";
        }
        if (number >= 1e228)
        {
            return (number / 1e228).ToString("F2") + "Z";
        }
        if (number >= 1e225)
        {
            return (number / 1e225).ToString("F2") + "Y";
        }
        if (number >= 1e222)
        {
            return (number / 1e222).ToString("F2") + "X";
        }
        if (number >= 1e219)
        {
            return (number / 1e219).ToString("F2") + "W";
        }
        if (number >= 1e216)
        {
            return (number / 1e216).ToString("F2") + "V";
        }
        if (number >= 1e213)
        {
            return (number / 1e213).ToString("F2") + "U";
        }
        if (number >= 1e210)
        {
            return (number / 1e210).ToString("F2") + "T";
        }
        if (number >= 1e207)
        {
            return (number / 1e207).ToString("F2") + "S";
        }
        if (number >= 1e204)
        {
            return (number / 1e204).ToString("F2") + "R";
        }
        if (number >= 1e201)
        {
            return (number / 1e201).ToString("F2") + "Q";
        }
        if (number >= 1e198)
        {
            return (number / 1e198).ToString("F2") + "P";
        }
        if (number >= 1e195)
        {
            return (number / 1e195).ToString("F2") + "O";
        }
        if (number >= 1e192)
        {
            return (number / 1e192).ToString("F2") + "N";
        }
        if (number >= 1e189)
        {
            return (number / 1e189).ToString("F2") + "M";
        }
        if (number >= 1e186)
        {
            return (number / 1e186).ToString("F2") + "L";
        }
        if (number >= 1e183)
        {
            return (number / 1e183).ToString("F2") + "K";
        }
        if (number >= 1e180)
        {
            return (number / 1e180).ToString("F2") + "J";
        }
        if (number >= 1e177)
        {
            return (number / 1e177).ToString("F2") + "I";
        }
        if (number >= 1e174)
        {
            return (number / 1e174).ToString("F2") + "H";
        }
        if (number >= 1e171)
        {
            return (number / 1e171).ToString("F2") + "G";
        }
        if (number >= 1e168)
        {
            return (number / 1e168).ToString("F2") + "F";
        }
        if (number >= 1e165)
        {
            return (number / 1e165).ToString("F2") + "E";
        }
        if (number >= 1e162)
        {
            return (number / 1e162).ToString("F2") + "D";
        }
        if (number >= 1e159)
        {
            return (number / 1e159).ToString("F2") + "C";
        }
        if (number >= 1e156)
        {
            return (number / 1e156).ToString("F2") + "B";
        }
        if (number >= 1e153)
        {
            return (number / 1e153).ToString("F2") + "A";
        }
        if (number >= 1e150)
        {
            return (number / 1e150).ToString("F2") + "Z";
        }
        if (number >= 1e147)
        {
            return (number / 1e147).ToString("F2") + "Y";
        }
        if (number >= 1e144)
        {
            return (number / 1e144).ToString("F2") + "X";
        }
        if (number >= 1e141)
        {
            return (number / 1e141).ToString("F2") + "W";
        }
        if (number >= 1e138)
        {
            return (number / 1e138).ToString("F2") + "V";
        }
        if (number >= 1e135)
        {
            return (number / 1e135).ToString("F2") + "U";
        }
        if (number >= 1e132)
        {
            return (number / 1e132).ToString("F2") + "T";
        }
        if (number >= 1e129)
        {
            return (number / 1e129).ToString("F2") + "S";
        }
        if (number >= 1e126)
        {
            return (number / 1e126).ToString("F2") + "R";
        }
        if (number >= 1e123)
        {
            return (number / 1e123).ToString("F2") + "Q";
        }
        if (number >= 1e120)
        {
            return (number / 1e120).ToString("F2") + "P";
        }
        if (number >= 1e117)
        {
            return (number / 1e117).ToString("F2") + "O";
        }
        if (number >= 1e114)
        {
            return (number / 1e114).ToString("F2") + "N";
        }
        if (number >= 1e111)
        {
            return (number / 1e111).ToString("F2") + "M";
        }
        if (number >= 1e108)
        {
            return (number / 1e108).ToString("F2") + "L";
        }
        if (number >= 1e105)
        {
            return (number / 1e105).ToString("F2") + "K";
        }
        if (number >= 1e102)
        {
            return (number / 1e102).ToString("F2") + "J";
        }
        if (number >= 1e99)
        {
            return (number / 1e99).ToString("F2") + "I";
        }
        if (number >= 1e96)
        {
            return (number / 1e96).ToString("F2") + "H";
        }
        if (number >= 1e93)
        {
            return (number / 1e93).ToString("F2") + "G";
        }
        if (number >= 1e90)
        {
            return (number / 1e90).ToString("F2") + "F";
        }
        if (number >= 1e87)
        {
            return (number / 1e87).ToString("F2") + "E";
        }
        if (number >= 1e84)
        {
            return (number / 1e84).ToString("F2") + "D";
        }
        if (number >= 1e81)
        {
            return (number / 1e81).ToString("F2") + "C";
        }
        if (number >= 1e78)
        {
            return (number / 1e78).ToString("F2") + "B";
        }
        if (number >= 1e75)
        {
            return (number / 1e75).ToString("F2") + "A";
        }
        if (number >= 1e72)
        {
            return (number / 1e72).ToString("F2") + "Z";
        }
        if (number >= 1e69)
        {
            return (number / 1e69).ToString("F2") + "Y";
        }
        if (number >= 1e66)
        {
            return (number / 1e66).ToString("F2") + "X";
        }
        if (number >= 1e63)
        {
            return (number / 1e63).ToString("F2") + "W";
        }
        if (number >= 1e60)
        {
            return (number / 1e60).ToString("F2") + "V";
        }
        if (number >= 1e57)
        {
            return (number / 1e57).ToString("F2") + "U";
        }
        if (number >= 1e54)
        {
            return (number / 1e54).ToString("F2") + "T";
        }
        if (number >= 1e51)
        {
            return (number / 1e51).ToString("F2") + "S";
        }
        if (number >= 1e48)
        {
            return (number / 1e48).ToString("F2") + "R";
        }
        if (number >= 1e45)
        {
            return (number / 1e45).ToString("F2") + "Q";
        }
        if (number >= 1e42)
        {
            return (number / 1e42).ToString("F2") + "P";
        }
        if (number >= 1e39)
        {
            return (number / 1e39).ToString("F2") + "O";
        }
        if (number >= 1e36)
        {
            return (number / 1e36).ToString("F2") + "N";
        }
        if (number >= 1e33)
        {
            return (number / 1e33).ToString("F2") + "M";
        }
        if (number >= 1e30)
        {
            return (number / 1e30).ToString("F2") + "L";
        }
        if (number >= 1e27)
        {
            return (number / 1e27).ToString("F2") + "K";
        }
        if (number >= 1e24)
        {
            return (number / 1e24).ToString("F2") + "Y";
        }
        if (number >= 1e21)
        {
            return (number / 1e21).ToString("F2") + "Z";
        }
        if (number >= 1e18)
        {
            return (number / 1e18).ToString("F2") + "E";
        }
        if (number >= 1e15)
        {
            return (number / 1e15).ToString("F2") + "P";
        }
        if (number >= 1e12)
        {
            return (number / 1e12).ToString("F2") + "T";
        }
        if (number >= 1e9)
        {
            return (number / 1e9).ToString("F2") + "B";
        }
        if (number >= 1e6)
        {
            return (number / 1e6).ToString("F2") + "M";
        }
        if (number >= 1e3)
        {
            return (number / 1e3).ToString("F2") + "K";
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