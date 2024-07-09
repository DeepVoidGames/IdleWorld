using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    private string saveFilePath;
    
    private void Awake()
    {
         if (Instance == null)
        {
            Instance = this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    private void Start()
    {
        Load();
    }

    public void Save()
    {
        GameData gameData = new GameData
        {
            goldData = GoldSystem.Instance.Gold,
            levelData = LevelSystem.Instance.Level,
            stageData = LevelSystem.Instance.Stage
        };

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved to " + saveFilePath);
    }

    public void Load()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            // Load gold data
            if (gameData.goldData != 0)
            {
                GoldSystem.Instance.SetGold(gameData.goldData);
            }

            // Load level
            if (gameData.levelData != 0)
            {
                LevelSystem.Instance.SetLevel(gameData.levelData);
            }

            // Load stage
            if (gameData.stageData != 0)
            {
                LevelSystem.Instance.SetStage(gameData.stageData);
            }
            Debug.Log("Game loaded from " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + saveFilePath);
        }
    
    }
}

[System.Serializable]
public class GameData
{
    public float goldData;
    public int levelData;
    public int stageData;
}
