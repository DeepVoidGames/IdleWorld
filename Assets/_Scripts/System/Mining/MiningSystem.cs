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

    private double miningLevel = 1;
    private double miningExperience = 0;
    private double miningEfficiency = 1;

    private bool isToolEquiped = false;
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
        if (miningExperience >= DifficultySystem.Instance.GetMiningExperienceNeeded())
        {
            miningExperience -= DifficultySystem.Instance.GetMiningExperienceNeeded();
            miningLevel++;
            UISystem.Instance.UpdateMiningUI();
        }
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
}
