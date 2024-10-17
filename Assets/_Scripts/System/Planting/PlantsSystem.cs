using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class Plant
{
    public string Name;
    public int levelRequired;
    public Items.Rarity rarity;
    [Description("Time to grow in seconds")]
    public float timeToGrow;
    [Description("Stages of the plant")]
    public List<Sprite> stages;
}

public class PlantsSystem : MonoBehaviour 
{
    private static PlantsSystem instance;
    public static PlantsSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlantsSystem>();
            }
            return instance;
        }
    }      

    [Header("Plants")]
    public List<Plant> plants;
}
