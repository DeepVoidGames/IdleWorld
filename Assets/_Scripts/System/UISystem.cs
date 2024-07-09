using System;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI")]
    [SerializeField] private Text levelText;
    [SerializeField] private Text goldText;

    public void UpdateLevelText()
    {
        levelText.text = String.Format("Level: {0} Stage: {1}", NumberFormatInt(LevelSystem.Instance.Level), NumberFormatInt(LevelSystem.Instance.Stage));
    }

    public void UpdateGoldText()
    {
        goldText.text = NumberFormat(GoldSystem.Instance.Gold);
    }

    public void LoadUI()
    {
        UpdateLevelText();
        UpdateGoldText();
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
}