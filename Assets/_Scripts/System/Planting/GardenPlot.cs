using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenPlot : MonoBehaviour 
{
    [Header("Garden Plant ID")]
    [SerializeField] private int gardenPlantID;
    [Header("Plant")]
    [SerializeField] private Plant _plant;
    [Header("UI Elements")]
    [SerializeField] private Image plantImage;
    [SerializeField] private Text timerText;

    private float _timer;

    private bool harvestable = false;

    public void Plant(Plant plant)
    {
        _plant = plant;
        _timer = _plant.timeToGrow;
        StartGrowing();
    }

    private void StartRandomPlant()
    {
        // PlantingSystem.Instance.plantingLuck Should have impact on the rarity of the plant

        float plantingLuck = PlantingSystem.Instance.plantingLuck;
        float randomRarity = UnityEngine.Random.Range(0f, 100f) + plantingLuck;
        Items.Rarity rarity = Items.Rarity.Common;

        if (randomRarity < 40f)
        {
            rarity = Items.Rarity.Common;
        }
        else if (randomRarity < 65f)
        {
            rarity = Items.Rarity.Uncommon;
        }
        else if (randomRarity < 85f)
        {
            rarity = Items.Rarity.Rare;
        }
        else if (randomRarity < 95f)
        {
            rarity = Items.Rarity.Epic;
        }
        else if (randomRarity < 99f)
        {
            rarity = Items.Rarity.Legendary;
        }
        else
        {
            rarity = Items.Rarity.Mythical;
        }

        List<Plant> plantsByRarity = PlantsSystem.Instance.plants.FindAll(p => p.rarity == rarity);
        
        if (plantsByRarity.Count == 0)
        {
            Plant(PlantsSystem.Instance.plants[0]);
            return;
        }

        int random = UnityEngine.Random.Range(0, plantsByRarity.Count);
        Plant(plantsByRarity[random]);
    }

    private void StartGrowing()
    {
        UIUpdate();
        StartCoroutine(Grow());
    }

    private void UIUpdate()
    {
        plantImage.sprite = _plant.stages[0];
    }

    private void SetStageOfPlant(float _timer)
    {
        if (_timer <= _plant.timeToGrow / 3)
        {
            plantImage.sprite = _plant.stages[2];
        }
        else if (_timer <= _plant.timeToGrow / 3 * 2)
        {
            plantImage.sprite = _plant.stages[1];
        }
        else
        {
            plantImage.sprite = _plant.stages[0];
        }
    }

    private void SetTimerUI(float _timer)
    {
        if (_timer <= 0)
        {
            timerText.text = "Ready to harvest!";
        }
        else if(_timer <= 60)
        {
            timerText.text = String.Format("{0}s", _timer.ToString("F0"));
        }
        else
        {
            timerText.text = String.Format("{0}m {1}s", Mathf.Floor(_timer / 60).ToString("F0"), (_timer % 60).ToString("F0"));
        }
    }

    IEnumerator Grow()
    {
        if (_timer <= 0)
            yield break;

        while (true)
        {
            yield return new WaitForSeconds(1);
            _timer -= 1;
            SetTimerUI(_timer);
            SetStageOfPlant(_timer);
            if (_timer <= 0)
            {
                harvestable = true;
                break;
            }
        }
        yield return null;
    }

    public void Harvest()
    {
        if (harvestable)
        {
            float harvestRate = UnityEngine.Random.Range(1f, 5f);
            float amount = harvestRate * 1;
            InventorySystem.Instance.AddItemByName(_plant.Name, amount);
            PlantingSystem.Instance.AddPlantingExp(_plant);
            StartRandomPlant();
        }
    }

    private void SaveCurrentPlant()
    {
        PlayerPrefs.SetString("GardenPlot_" + gardenPlantID, _plant.Name);
        PlayerPrefs.SetFloat("GardenPlot_" + gardenPlantID + "_Timer", _timer);
        PlayerPrefs.SetFloat("GardenPlot_" + gardenPlantID + "_CurrentTime", Time.time);
    }

    private bool LoadCurrentPlant()
    {
        if (PlayerPrefs.HasKey("GardenPlot_" + gardenPlantID))
        {
            string plantName = PlayerPrefs.GetString("GardenPlot_" + gardenPlantID);
            float timer = PlayerPrefs.GetFloat("GardenPlot_" + gardenPlantID + "_Timer");
            float currentTime = PlayerPrefs.GetFloat("GardenPlot_" + gardenPlantID + "_CurrentTime");
            float elapsedTime = Time.time - currentTime;
            _plant = PlantsSystem.Instance.plants.Find(p => p.Name == plantName);
            _timer = timer - elapsedTime;
            if (_timer <= 0)
                harvestable = true;
            else
                StartGrowing();
            UIUpdate();
            return true;
        }
        return false;
    }

    private void Start() 
    {
        if (_plant != null && !LoadCurrentPlant())
        {
            int random = UnityEngine.Random.Range(0, PlantsSystem.Instance.plants.Count);
            Plant(PlantsSystem.Instance.plants[random]);
        }   
    }

    private void OnApplicationQuit() 
    {
        SaveCurrentPlant();
    }

    private void OnApplicationPause(bool pauseStatus) 
    {
        if (pauseStatus)
        {
            SaveCurrentPlant();
        }
    }
    
    private void OnDisable() 
    {
        SaveCurrentPlant();
    }

    private void OnEnable() 
    {
        LoadCurrentPlant();
    }
}