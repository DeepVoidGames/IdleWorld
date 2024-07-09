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
        return (float)(baseHealth * Math.Pow(_difficultyMultiplier, LevelSystem.Instance.Level));
    }

    public float GetBossDrop(float health)
    {
        return health + (health * _difficultyMultiplier);
    }

    public float GetMonsterHealth(float baseHealth)
    {
        return (float)(baseHealth * Math.Pow(LevelSystem.Instance.Level, 2)) + (LevelSystem.Instance.Stage * _difficultyMultiplier);
    }

    public float GetMonsterDrop(float maxHealth)
    {
        return maxHealth + (maxHealth * _difficultyMultiplier);
    }

    public float GetRockHealth(float baseHealth)
    {
        return (float)(baseHealth * Math.Pow(MiningSystem.Instance.miningLevel, 2)) + (MiningSystem.Instance.miningLevel * _difficultyMultiplier);
    }

    public float GetRockDrop(float maxHealth, float minDrop, float maxDrop)
    {
        return UnityEngine.Random.Range(minDrop, maxDrop) + (maxHealth * _difficultyMultiplier);
    }

    public float GetMiningExperience(float maxHealth)
    {
        return (float)(maxHealth * Math.Pow(MiningSystem.Instance.miningLevel, 2)) + (MiningSystem.Instance.miningLevel * _difficultyMultiplier);
    }

    public float GetMiningNextLevel()
    {
        return (float)(MiningSystem.Instance.miningLevel * Math.Pow(MiningSystem.Instance.miningLevel, 2)) + (MiningSystem.Instance.miningLevel * _difficultyMultiplier);
    }



}