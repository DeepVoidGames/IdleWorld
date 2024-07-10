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

    // Boss
    public float GetBossHealth(float baseHealth)
    {
        return (float)(baseHealth * Math.Pow(LevelSystem.Instance.Level, 2) + (LevelSystem.Instance.Stage * _difficultyMultiplier) + (DamageSystem.Instance.Damage * _difficultyMultiplier));
    }

    public float GetBossDrop(float health)
    {
        return health + (health * _difficultyMultiplier);
    }

    // Monster
    public float GetMonsterHealth(float baseHealth)
    {
        return (float)(baseHealth * Math.Pow(LevelSystem.Instance.Level, 2));
    }

    public float GetMonsterDrop(float maxHealth)
    {
        return maxHealth + (maxHealth * _difficultyMultiplier);
    }

    //  Mining
    public float GetRockHealth(float baseHealth)
    {
        return (float)(baseHealth * Math.Pow(MiningSystem.Instance.MiningEfficiency, 2)) + (MiningSystem.Instance.MiningLevel * _difficultyMultiplier);
    }

    public float GetRockDrop(float maxHealth, float minDrop)
    {
        return UnityEngine.Random.Range(minDrop, maxHealth);
    }

    public float GetMiningExperience(float maxHealth)
    {
        return (float)(maxHealth  + (MiningSystem.Instance.MiningLevel * _difficultyMultiplier));
    }

    public float GetMiningExperienceNeeded()
    {
        return (float)(MiningSystem.Instance.MiningLevel * Math.Pow(MiningSystem.Instance.MiningLevel, 2)) + (MiningSystem.Instance.MiningLevel * _difficultyMultiplier);
    }

    public float GetMiningEfficiency()
    {
        if (MiningSystem.Instance.MiningLevel < 10)
        {
            return MiningSystem.Instance.MiningEfficiency * .1f;
        }
        else
        {
            return MiningSystem.Instance.MiningEfficiency * .05f;
        }
    }


}