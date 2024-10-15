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

[System.Serializable]
public class PlantIndex
{
    public int index;
    public Plant plant;
    public float amount;
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

    [Header("Plant Index")]
    private List<PlantIndex> plantIndex;

    public void AddPlant(Plant plant, float amount)
    {
        PlantIndex plantIndex = this.plantIndex.Find(x => x.plant == plant);
        if (plantIndex != null)
        {
            plantIndex.amount += amount;
        }
         
    }

    private void Awake()
    {
        plantIndex = new List<PlantIndex>();
        for (int i = 0; i < plants.Count; i++)
        {
            PlantIndex plantIndex = new PlantIndex();
            plantIndex.index = i;
            plantIndex.plant = plants[i];
            plantIndex.amount = 0;
            this.plantIndex.Add(plantIndex);
        }
    }

    public List<PlantData> SavePlantData()
    {
        List<PlantData> plantData = new List<PlantData>();
        foreach (PlantIndex plant in plantIndex)
        {
            PlantData data = new PlantData
            {
                index = plant.index,
                amount = plant.amount
            };
            plantData.Add(data);
        }
        return plantData;
    }

    public void LoadPlantData(List<PlantData> plantData)
    {
        foreach (PlantData data in plantData)
        {
            PlantIndex plantIndex = this.plantIndex.Find(x => x.index == data.index);
            if (plantIndex != null)
            {
                plantIndex.amount = data.amount;
            }
        }
    }
}

[System.Serializable]
public class PlantData
{
    public int index;
    public float amount;
}
