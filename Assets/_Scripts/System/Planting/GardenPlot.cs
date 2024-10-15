using System;
using System.Collections;
using Unity.VisualScripting;
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
        int random = UnityEngine.Random.Range(0, PlantsSystem.Instance.plants.Count);
        Plant(PlantsSystem.Instance.plants[random]);
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
            //TODO harvest plant, add to inventory
            float harvestRate = UnityEngine.Random.Range(1f, 5f);
            float amount = harvestRate * 1;
            PlantsSystem.Instance.AddPlant(_plant, amount);
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