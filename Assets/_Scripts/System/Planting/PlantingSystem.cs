using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantingSystem : MonoBehaviour
{
    private static PlantingSystem instance;
    public static PlantingSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlantingSystem>();
            }
            return instance;
        }
    }

    [Header("Planting Level")]
    [SerializeField] private int plantingLevel = 1;
    [SerializeField] private float plantingExp;
    [SerializeField] private Text textPlantingLevel;
    [SerializeField] private Text textPlantingExp;
    public int PlantingLevel
    {
        get => plantingLevel;
        set
        {
            if (value < 1)
            {
                plantingLevel = 1;
            }
            else
            {
                plantingLevel = value;
            }
        }
    }
    public float PlantingExp { get => plantingExp; set => plantingExp = value; }

    [Header("Stats")]
    public int plantingLuck = 0;
    public float PlantingFortune = 1;
    public float HarvestFortune = 1;

    [Header("Water")]
    [SerializeField] private int water;
    [SerializeField] private int maxWater = 10;
    [SerializeField] private Text textWater;
    public int Water { get => water; }
    private float lastWaterTime; // Player get water every 24h or //TODO watch ads

    [Header("Fertilizer")]
    [SerializeField] private int fertilizer;
    [SerializeField] private int maxFertilizer = 10;
    [SerializeField] private Text textFertilizer;
    public int Fertilizer { get => fertilizer; }

    public void AddPlantingExp(Plant plant)
    {
        switch (plant.rarity)
        {
            case Items.Rarity.Common:
                plantingExp += plant.timeToGrow * 0.1f;
                break;
            case Items.Rarity.Uncommon:
                plantingExp += plant.timeToGrow * 0.2f;
                break;
            case Items.Rarity.Rare:
                plantingExp += plant.timeToGrow * 0.3f;
                break;
            case Items.Rarity.Epic:
                plantingExp += plant.timeToGrow * 0.4f;
                break;
            case Items.Rarity.Legendary:
                plantingExp += plant.timeToGrow * 0.5f;
                break;
            case Items.Rarity.Mythical:
                plantingExp += plant.timeToGrow * 0.6f;
                break;   
        }
        NeededExpToLevelUp();
    }

    private void NeededExpToLevelUp()
    {
        if (plantingExp >= plantingLevel * 100)
        {
            plantingExp -= plantingLevel * 100;
            plantingLevel++;
            if (plantingLevel % 5 == 0 && plantingLuck < 10)
                plantingLuck++;
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        textWater.text = $"Water: {water}/{maxWater}";
        textFertilizer.text = $"Fertilizer: {fertilizer}/{maxFertilizer}";
        textPlantingLevel.text = $"Planting Level: {plantingLevel}";
        textPlantingExp.text = $"Planting Exp: {plantingExp}/{plantingLevel * 100}";
    }

    private void WaterRefill()
    {
        lastWaterTime = PlayerPrefs.GetFloat("lastWaterTime");
        if (water < maxWater)
        {
            if (Time.time - lastWaterTime >= 86400)
            {
                water = maxWater;
                UpdateUI();
                lastWaterTime = Time.time;
            }
        }
        PlayerPrefs.SetFloat("lastWaterTime", Time.time);
        StartCoroutine(WaterRefillCoroutine());
    }

    IEnumerator WaterRefillCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.time - lastWaterTime >= 86400 ? 0 : 86400 - (Time.time - lastWaterTime));
            WaterRefill();
        }
    }

    private void Start() 
    {
        WaterRefill();
        UpdateUI();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("lastWaterTime", Time.time);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetFloat("lastWaterTime", Time.time);
        }
    }
}

