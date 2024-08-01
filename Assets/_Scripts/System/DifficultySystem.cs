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
    //Mining
    private double miningEfficiencyPercentage;
    private double miningDropRateMultiplier;
    private double miningBonusMiningEfficiency;

    public double MiningBonusMiningEfficiency
    {
        get => miningBonusMiningEfficiency;
        set
        {
            miningBonusMiningEfficiency = value;
            UISystem.Instance.UpdateMiningUI();
        }
    }

    // Difficulty for Cave Upgrades
    public void AddDamagePercentage(double value)
    {
        DamagePercentage += value;
        UISystem.Instance.UpdateLevelText();
    }

    public void AddMiningEfficiencyPercentage(double value)
    {
        miningEfficiencyPercentage += value;
        UISystem.Instance.UpdateMiningUI();
    }

    public void AddMiningDropRateMultiplier(double value)
    {
        miningDropRateMultiplier += value;
    }

    public double GetMiningEfficiencyPercentage()
    {
        return miningEfficiencyPercentage;
    }

    // Damage

    public double GetDamage(double baseDamage)
    {
        if (baseDamage == 0)
            baseDamage = 5;
        return baseDamage + baseDamage * DamagePercentage;
    }

    public double GetDPS()
    {
        double dps = 0;
        foreach (var hero in TavernSystem.Instance.heroes)
        {
            if (hero.isUnlocked)
                dps += hero.dps;
        }
        return dps;
    }

    public double GetIdleReward()
    {
        double health = BiomeSystem.Instance.Bioms.Find(x => x.Name == BiomeSystem.Instance.CurrentBiome).Monsters[0].Health;
        return health + (health * _difficultyMultiplier) * IdleSystem.Instance.IdleTime / 60 / 60;
    }

    // Boss
    public double GetBossHealth(double baseHealth)
    {
        return baseHealth + (baseHealth * LevelSystem.Instance.Level) + (LevelSystem.Instance.Stage * _difficultyMultiplier) + (DamageSystem.Instance.Damage * _difficultyMultiplier);
    }

    public double GetBossDrop(double maxHealth)
    {
        return maxHealth + (maxHealth * _difficultyMultiplier);
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

    public double GetRockDrop(double maxHealth)
    {
        if (miningDropRateMultiplier == 0)
            return maxHealth/2;
        return (maxHealth/2) * miningDropRateMultiplier;
    }

    public double GetMiningExperience(double maxHealth)
    {
        return maxHealth/2 + (MiningSystem.Instance.MiningLevel * _difficultyMultiplier);
    }

    public double GetMiningExperienceNeeded()
    {
        return (100f + Math.Pow(MiningSystem.Instance.MiningLevel, 2) + (MiningSystem.Instance.MiningLevel * _difficultyMultiplier)) * 5;
    }

    public double GetMiningEfficiency(double baseEfficiency)
    {
        if (baseEfficiency == 0 && miningBonusMiningEfficiency == 0)
            baseEfficiency = 1;
        double dmg = baseEfficiency + miningBonusMiningEfficiency;
        return dmg + dmg * miningEfficiencyPercentage;
    }
}