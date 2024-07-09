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
        levelText.text = String.Format("Level: {0} Stage: {1}", LevelSystem.Instance.Level, LevelSystem.Instance.Stage);
    }

    public void UpdateGoldText()
    {
        goldText.text = GoldSystem.Instance.Gold.ToString();
    }

    public void LoadUI()
    {
        UpdateLevelText();
        UpdateGoldText();
    }
}