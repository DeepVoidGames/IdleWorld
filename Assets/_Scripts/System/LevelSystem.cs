using UnityEngine;

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

    [SerializeField] private int level = 1;
    [SerializeField] private int stage = 1;
    [SerializeField] private int maxStage = 10;
    
    public int Level {get => level; private set => level = value;}
    public int Stage {get => stage; private set => stage = value;}

    public void NextStage()
    {
        if (stage == maxStage)
        {
            level++;
            stage = 1;
        }
        else
        {
            stage++;
        }
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void SetStage(int stage)
    {
        this.stage = stage;
    }
}