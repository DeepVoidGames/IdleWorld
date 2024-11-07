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
    [SerializeField] private Button actionButton;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Text unlockButtonText;

    [Header("Unlock Cost")]
    [SerializeField] private double unlockCost = 1000;
    [SerializeField] private bool isLocked = true;

    private float gardenTimer;
    private bool harvestable = false;

    public void Plant(Plant plant)
    {
        if (isLocked) return;

        _plant = plant;
        gardenTimer = _plant.timeToGrow;
        StartGrowing();
    }

   private void StartRandomPlant()
    {
        if (isLocked) return;

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

        int playerLevel = PlantingSystem.Instance.PlantingLevel;
        List<Plant> plantsByRarity = PlantsSystem.Instance.plants.FindAll(p => p.rarity == rarity && p.levelRequired <= playerLevel);
        
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
        if (isLocked) return;

        StartCoroutine(Grow());
        SetStageOfPlant();
        SetTimerUI();
    }

    private void SetStageOfPlant()
    {
        if (isLocked || _plant == null || _plant.stages == null || _plant.stages.Count < 3) return;

        if (gardenTimer <= 0)
        {
            plantImage.sprite = _plant.stages[2];
        }
        else if (gardenTimer <= _plant.timeToGrow / 3)
        {
            plantImage.sprite = _plant.stages[2];
        }
        else if (gardenTimer <= _plant.timeToGrow / 3 * 2)
        {
            plantImage.sprite = _plant.stages[1];
        }
        else
        {
            plantImage.sprite = _plant.stages[0];
        }
    }

    private void SetTimerUI()
    {
        if (isLocked)
        {
            timerText.text = "Locked";
            return;
        }

        if (gardenTimer <= 0)
        {
            timerText.text = "Ready to harvest!";
        }
        else if(gardenTimer <= 60)
        {
            timerText.text = String.Format("{0}s", gardenTimer.ToString("F0"));
        }
        else
        {
            timerText.text = String.Format("{0}m {1}s", Mathf.Floor(gardenTimer / 60).ToString("F0"), (gardenTimer % 60).ToString("F0"));
        }
    }

    IEnumerator Grow()
    {
        if (gardenTimer <= 0 || isLocked)
            yield break;

        while (true)
        {
            yield return new WaitForSeconds(1);
            gardenTimer -= 1;
            SetTimerUI();
            SetStageOfPlant();
            if (gardenTimer <= 0)
            {
                harvestable = true;
                break;
            }
        }
        yield return null;
    }

    public void Harvest()
    {
        if (harvestable && !isLocked && _plant != null)
        {
            float harvestRate = UnityEngine.Random.Range(1f, 5f);
            float amount = harvestRate * 1;
            InventorySystem.Instance.AddItemByName(_plant.Name, amount);
            PlantingSystem.Instance.AddPlantingExp(_plant);
            harvestable = false;
            StartRandomPlant();
        }
    }

    public void UnlockGardenPlot()
    {
        if (GoldSystem.Instance.Gold >= unlockCost)
        {
            GoldSystem.Instance.SpendGold(unlockCost);
            isLocked = false;
            unlockButton.gameObject.SetActive(false);
            actionButton.gameObject.SetActive(true);
            SetTimerUI();
            SaveCurrentPlant(); // Save the state immediately after unlocking
        }
    }

    private void UpdateUnlockButton(double gold)
    {
        if (GoldSystem.Instance.Gold >= unlockCost)
        {
            unlockButtonText.text = "Unlock (" + UISystem.Instance.NumberFormat(unlockCost) + " Gold)";
            unlockButton.GetComponent<Image>().color = UISystem.Instance.buyButtonColor;
        }
        else
        {
            unlockButtonText.text = "Unlock (" + UISystem.Instance.NumberFormat(unlockCost) + " Gold)";
            unlockButton.GetComponent<Image>().color = UISystem.Instance.buyButtonDisabledColor;
        }
    }

    private void SaveCurrentPlant()
    {
        if (_plant != null)
        {
            PlayerPrefs.SetString("GardenPlot_" + gardenPlantID, _plant.Name);
            PlayerPrefs.SetFloat("GardenPlot_" + gardenPlantID + "_Timer", gardenTimer);
            PlayerPrefs.SetString("GardenPlot_" + gardenPlantID + "_CurrentTime", DateTime.Now.ToString());
        }
        PlayerPrefs.SetInt("GardenPlot_" + gardenPlantID + "_Locked", isLocked ? 1 : 0);
    }

    private void LoadCurrentPlant()
    {
        isLocked = PlayerPrefs.GetInt("GardenPlot_" + gardenPlantID + "_Locked", 1) == 1;
        unlockButton.gameObject.SetActive(isLocked);
        actionButton.gameObject.SetActive(!isLocked);
        UpdateUnlockButton(GoldSystem.Instance.Gold);
        SetTimerUI(); // Call SetTimerUI after updating isLocked

        if (PlayerPrefs.HasKey("GardenPlot_" + gardenPlantID) && !isLocked)
        {
            string plantName = PlayerPrefs.GetString("GardenPlot_" + gardenPlantID);
            float timer = PlayerPrefs.GetFloat("GardenPlot_" + gardenPlantID + "_Timer");
            string currentTimeString = PlayerPrefs.GetString("GardenPlot_" + gardenPlantID + "_CurrentTime");

            DateTime currentTime;
            if (DateTime.TryParse(currentTimeString, out currentTime))
            {
                TimeSpan elapsedTime = DateTime.Now - currentTime;

                _plant = PlantsSystem.Instance.plants.Find(p => p.Name == plantName);

                if (_plant == null)
                {
                    Debug.LogWarning("Plant not found in PlantsSystem. Resetting garden timer.");
                    StartRandomPlant();
                }

                gardenTimer = timer - (float)elapsedTime.TotalSeconds;

                if (gardenTimer <= 0)
                {
                    gardenTimer = 0;
                    harvestable = true;
                }
                else
                {
                    StartGrowing();
                }
                SetStageOfPlant();
            }
            else
            {
                Debug.LogWarning("Invalid date format in PlayerPrefs. Resetting garden timer.");
                gardenTimer = 0;
                harvestable = true;
                SetStageOfPlant();
            }
        }
        else if (!isLocked)
        {
            int random = UnityEngine.Random.Range(0, PlantsSystem.Instance.plants.Count);
            Plant(PlantsSystem.Instance.plants[random]);
        }
    }

    private void Start() 
    {
        LoadCurrentPlant();
        unlockButton.onClick.AddListener(UnlockGardenPlot);
        GoldSystem.Instance.OnGoldChanged += (gold) => UpdateUnlockButton(gold);
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
}