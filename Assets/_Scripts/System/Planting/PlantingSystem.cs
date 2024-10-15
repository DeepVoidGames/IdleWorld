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
    [SerializeField] private int plantingLevel;
    [SerializeField] private Text textPlantingLevel;
    public int PlantingLevel { get => plantingLevel; set => plantingLevel = value; }

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

    private void UpdateUI()
    {
        textWater.text = $"Water: {water}/{maxWater}";
        textFertilizer.text = $"Fertilizer: {fertilizer}/{maxFertilizer}";
        textPlantingLevel.text = $"Planting Level: {plantingLevel}";
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

