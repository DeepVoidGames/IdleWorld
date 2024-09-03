using UnityEngine;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour 
{
    private static LevelSystem _instance;
    public static LevelSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("LevelSystem");
                    _instance = go.AddComponent<LevelSystem>();
                }
            }
            return _instance;
        }
    }

    [Header("Level System")]
    [SerializeField] private int level = 1;
    [SerializeField] private int levelToNextBiome = 10;
    [Header("Stage System")]
    [SerializeField] private int stage = 1;
    [SerializeField] private int maxStage = 100;

    [Header("Prestige System")]
    [SerializeField] private int prestigeLevel = 0;
    [SerializeField] private int baseLevelToPrestige = 100;
    [SerializeField] private int levelToPrestige = 100;
    [SerializeField] private int maxPrestigeLevel = 100;

    [Header("UI Elements")]
    [SerializeField] private Button prestigeButton;

    public int PrestigeLevel {get => prestigeLevel; private set => prestigeLevel = value;}
    public int MaxPrestigeLevel {get => maxPrestigeLevel; private set => maxPrestigeLevel = value;}
    public int LevelToPrestige {get => levelToPrestige; private set => levelToPrestige = value;}

    public float HighestStage => PlayerPrefs.GetInt("HighestStage", 0);
    public float HighestLevel => PlayerPrefs.GetInt("HighestLevel", 0);

    public int Level {get => level; private set => level = value;}
    public int Stage {get => stage; private set => stage = value;}

    public void NextStage()
    {
        if (stage == maxStage)
        {
            BossSystem.Instance.SpawnBoss();
        }
        else
        {
            stage++;
        }
        UISystem.Instance.UpdateLevelText();
    }

    public void ResetStage()
    {
        level++;
        ContentLocker.Instance.CheckContent(level);
        stage = 1;
        if (level % levelToNextBiome == 0)
        {
            BiomeSystem.Instance.NextBiome();
        }
        UISystem.Instance.UpdateLevelText();
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void SetStage(int stage)
    {
        this.stage = stage;
    }

    public void Prestige()
    {
        if (prestigeLevel == maxPrestigeLevel)
            return;

        if (level == levelToPrestige)
        {
            prestigeLevel++;
            level -= levelToPrestige;
            stage = 1;
            BiomeSystem.Instance.SetCurrentBiome(BiomeSystem.Instance.Bioms[0].Name);
        }
        else if (level > levelToPrestige)
        {
            for (int i = 0; i < levelToPrestige; i++)
            {
                if (prestigeLevel >= maxPrestigeLevel)
                    break;
                level -= levelToPrestige;
                prestigeLevel++;
                levelToPrestige = baseLevelToPrestige + baseLevelToPrestige * prestigeLevel;
                UISystem.Instance.UpdateLevelText();
                if (level < levelToPrestige)
                    break;
            }
            stage = 1;
            BiomeSystem.Instance.SetCurrentBiome(BiomeSystem.Instance.Bioms[0].Name);
            BonusSystem.Instance.RestartBonuses();
            return;
        }
        else 
        {
            return;
        }

        BonusSystem.Instance.RestartBonuses();
        levelToPrestige = baseLevelToPrestige + baseLevelToPrestige * prestigeLevel;
        UISystem.Instance.UpdateLevelText();
    }

    public void RestetSlayer()
    {
        level = 1;
        stage = 1;
        BiomeSystem.Instance.SetCurrentBiome(BiomeSystem.Instance.Bioms[0].Name);
        UISystem.Instance.UpdateLevelText();
    }

    public void SetPrestigeLevel(int prestigeLevel)
    {
        this.prestigeLevel = prestigeLevel;

        if (prestigeLevel == maxPrestigeLevel)
            prestigeButton.interactable = false;

        if (prestigeLevel != 0)
            levelToPrestige = baseLevelToPrestige + baseLevelToPrestige * prestigeLevel;
    }
}