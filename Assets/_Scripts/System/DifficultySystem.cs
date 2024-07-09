using System;
using UnityEngine;

public class DifficultySystem : MonoBehaviour 
{
    private static DifficultySystem _instance;
    public static DifficultySystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DifficultySystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("DifficultySystem");
                    _instance = go.AddComponent<DifficultySystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private float _difficultyMultiplier = 1.2f;

    public float GetBossHealth(float baseHealth)
    {
        return baseHealth * (LevelSystem.Instance.Level * LevelSystem.Instance.Stage);
    }

    public float GetBossDrop(float health)
    {
        return health + (health * _difficultyMultiplier);
    }
}