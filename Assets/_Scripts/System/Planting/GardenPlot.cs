using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenPlot : MonoBehaviour 
{
    [Header("Plant")]
    [SerializeField] private Plant _plant;
    [Header("UI Elements")]
    [SerializeField] private Image plantImage;
    [SerializeField] private Text timerText;

    private bool harvestable = false;

    public void Plant(Plant plant)
    {
        _plant = plant;
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
        //TODO start growing, set ui timer change sprite, start coroutine
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
        float _timer = _plant.timeToGrow;
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

    private void Start() 
    {
        if (_plant != null)
        {
            int random = UnityEngine.Random.Range(0, PlantsSystem.Instance.plants.Count);
            Plant(PlantsSystem.Instance.plants[random]);
        }   
    }
        
}