using System;
using System.Collections;
using UnityEngine;

public class MiningSystem : MonoBehaviour
{
    private static MiningSystem _instance;
    public static MiningSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MiningSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MiningSystem");
                    _instance = go.AddComponent<MiningSystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private double miningLevel = 1;
    private double miningExperience = 0;
    private double miningEfficiency = 1;

    private bool isToolEquiped = false;
    private bool isAutoMining = false;

    private Items tool;

    public double MiningLevel { get { return miningLevel; } }
    public double MiningExperience { get { return miningExperience; } }
    public double MiningEfficiency { get { return DifficultySystem.Instance.GetMiningEfficiency(miningEfficiency); } }
    public bool IsToolEquipped { get { return isToolEquiped; } set { isToolEquiped = value; } }

    // Event
    public delegate void ExperienceAddedHandler(double experienceAdded);
    public event ExperienceAddedHandler OnExperienceAdded;

    public void AddMiningExperience(double value)
    {
        miningExperience += value;
        OnExperienceAdded?.Invoke(value); // Invoke the event

        if (miningExperience >= DifficultySystem.Instance.GetMiningExperienceNeeded())
        {
            miningExperience -= DifficultySystem.Instance.GetMiningExperienceNeeded();
            miningLevel++;
            OverflowMiningExperience();
            UISystem.Instance.UpdateMiningUI();
        }
    }

    private void OverflowMiningExperience()
    {
        while (miningExperience >= DifficultySystem.Instance.GetMiningExperienceNeeded())
        {
            miningExperience -= DifficultySystem.Instance.GetMiningExperienceNeeded();
            miningLevel++;
        }
        UISystem.Instance.UpdateMiningUI();
    }

    public void AddMiningEfficiency(double value)
    {
        miningEfficiency += value;
    }


    public void SetMiningEfficiency(double value)
    {
        miningEfficiency = value;
    }

    public void SetMiningLevel(double value)
    {
        miningLevel = value;
    }

    public void RemoveMiningLevel(double value)
    {
        miningLevel -= value;
        UISystem.Instance.UpdateMiningUI();
    }

    public void SetMiningExperience(double value)
    {
        miningExperience = value;
    }


    public Items GetTool()
    {
        return tool;
    }

    public void EquipTool(Items item)
    {
        this.tool = item;
        UpdateToolMiningEfficiency();
        isToolEquiped = true;
        UISystem.Instance.UpdateMiningUI();
    }

    private void UpdateToolMiningEfficiency()
    {
        if (tool != null)
        {
            miningEfficiency = tool.miningEfficiency;
        }
    }

    // TODO : Replace AutoMining with workers system. Player can have workers to mine for him
    public void SetAutoMining(bool value)
    {
        isAutoMining = value;
        if (isAutoMining)
        {
            StartCoroutine(AutoMining());
        }
    }

    // Auto Mining
    IEnumerator AutoMining()
    {
        // Every one second attcck rock if exist
        float _timer = 0;
        while (isAutoMining)
        {
            _timer += Time.deltaTime;
            if (_timer >= .5f)
            {
                _timer = 0;
                if (CaveSystem.Instance.CurrentRock != null)
                {
                    CaveSystem.Instance.CurrentRock.GetComponent<RockObject>().Damage(MiningEfficiency);
                }
            }
            yield return null;
        }
    }
}
