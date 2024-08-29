using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Load/Save Current Potion

public class PotionsSystem : MonoBehaviour
{
    private static PotionsSystem _instance;
    public static PotionsSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PotionsSystem>();
            }
            return _instance;
        }
    }

    [SerializeField] private float getPotionCooldown = 1800f;
    [SerializeField] private bool isPotionCooldown = false;
    private float _potionCooldownTimer;

    public Items currentPotion;
    private float _timer;
    public float Timer { get => _timer; }

    public void GetRandomPotion()
    {
        if (isPotionCooldown)
            return;
        List<Items> potions = ItemSystem.Instance.ItemsCollection.FindAll(x => x.category == InventorySystem.Category.Potion);
        InventorySystem.Instance.AddItem(potions[Random.Range(0, potions.Count)].id, 1);
        StartCoroutine(GetPotionCooldown());
    }

    private IEnumerator GetPotionCooldown()
    {
        isPotionCooldown = true;
        _potionCooldownTimer = getPotionCooldown;
        while (_potionCooldownTimer > 0)
        {
            _potionCooldownTimer -= Time.deltaTime;
            yield return null;
        }
        isPotionCooldown = false;
    }

    public void UsePotion(Items item)
    {
        if (currentPotion != null)
        {
            RemovePotionBonus();
        }
        InventorySystem.Instance.RemoveItem(item.id, 1);
        currentPotion = item;
        UISystem.Instance.UpdatePotionUI();
        AddPotionBonus();
        InventorySystem.Instance.UpdateUI();
        return;
    }

    private void AddPotionBonus()
    {
        if(currentPotion.potionType == Items.PotionType.Damage)
        {
            DifficultySystem.Instance.AddDamagePercentage(currentPotion.potionValue);
        }
        else if(currentPotion.potionType == Items.PotionType.Gold)
        {
            DifficultySystem.Instance.GoldBonus += currentPotion.potionValue;
        }
        else if(currentPotion.potionType == Items.PotionType.MiningEfficiency)
        {
             DifficultySystem.Instance.AddMiningEfficiencyPercentage(currentPotion.potionValue);
        }
        StartCoroutine(PotionDuration());
    }

    private void RemovePotionBonus()
    {
        if (currentPotion.potionType == Items.PotionType.Damage)
        {
            DifficultySystem.Instance.RemoveDamagePercentage(currentPotion.potionValue);
        }
        else if (currentPotion.potionType == Items.PotionType.Gold)
        {
            DifficultySystem.Instance.GoldBonus -= currentPotion.potionValue;
        }
        else if (currentPotion.potionType == Items.PotionType.MiningEfficiency)
        {
            DifficultySystem.Instance.RemoveMiningEfficiencyPercentage(currentPotion.potionValue);
        }
        currentPotion = null;
    }

    private void LoadPotion()
    {
        if (PlayerPrefs.HasKey("PotionID"))
        {
            currentPotion = ItemSystem.Instance.GetItem(PlayerPrefs.GetInt("PotionID"));
            currentPotion.potionDuration = PlayerPrefs.GetFloat("PotionDuration");
            AddPotionBonus();
        }
    }

    private void SavePotion()
    {
        if (currentPotion == null)
            return;
        if (currentPotion.potionDuration <= 0)
        {
            RemovePotionBonus();
            return;
        }
        PlayerPrefs.SetInt("PotionID", currentPotion.id);
        PlayerPrefs.SetFloat("PotionDuration", _timer);
    }

    private IEnumerator PotionDuration()
    {
        _timer = currentPotion.potionDuration;
        while (_timer > 0)
        {
            _timer -= Time.deltaTime;
            UISystem.Instance.UpdatePotionUI();
            yield return null;
        }
        RemovePotionBonus();
    }

    private void Start() 
    {
        LoadPotion();    
    }

    private void OnApplicationQuit() 
    {
        SavePotion();
    }

    private void OnApplicationPause(bool pauseStatus) 
    {
        if (pauseStatus)
        {
            SavePotion();
        }
    }
}
