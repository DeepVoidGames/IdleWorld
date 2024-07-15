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

    // Damage
    private double DamagePercentage;

    public void AddDamagePercentage(double value)
    {
        DamagePercentage += value;
        UISystem.Instance.UpdateLevelText();
    }

    public double GetDamage(double baseDamage)
    {
        return baseDamage  + baseDamage * DamagePercentage;
    }

    // Boss
    public double GetBossHealth(double baseHealth)
    {
        return baseHealth + (baseHealth * LevelSystem.Instance.Level) + (LevelSystem.Instance.Stage * _difficultyMultiplier) + (DamageSystem.Instance.Damage * _difficultyMultiplier);
    }

    public double GetBossDrop(double health)
    {
        return health + (health * _difficultyMultiplier);
    }

    // Monster
    public double GetMonsterHealth(double baseHealth)
    {
        return baseHealth + baseHealth * LevelSystem.Instance.Level/2 + (LevelSystem.Instance.Stage * _difficultyMultiplier) + (DamageSystem.Instance.Damage * _difficultyMultiplier);
    }

    public double GetMonsterDrop(double maxHealth)
    {
        return maxHealth + (maxHealth * _difficultyMultiplier);
    }

    //  Mining
    public double GetRockHealth(double baseHealth)
    {
        return baseHealth + baseHealth * MiningSystem.Instance.MiningLevel;
    }

    public double GetRockDrop(double maxHealth, double minDrop)
    {
        return UnityEngine.Random.Range((float)minDrop, (float)maxHealth);
    }

    public double GetMiningExperience(double maxHealth)
    {
        return maxHealth/2 + (MiningSystem.Instance.MiningLevel * _difficultyMultiplier);
    }

    public double GetMiningExperienceNeeded()
    {
        return (100f + Math.Pow(MiningSystem.Instance.MiningLevel, 2) + (MiningSystem.Instance.MiningLevel * _difficultyMultiplier)) * 5;
    }
}