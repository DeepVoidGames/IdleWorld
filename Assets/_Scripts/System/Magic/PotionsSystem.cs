using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Potion Cost")]
    [SerializeField] private int potionCost = 1000000;
    [Header("Potion Cooldown")]
    [SerializeField] private float getPotionCooldown = 1800f;
    [SerializeField] private bool isPotionCooldown = false;
    
    [Header("Potion")]
    public Items currentPotion;

    [Header("Potion Timer")]
    [SerializeField] private float _timer;
    public float Timer { get => _timer; }

    [Header("Potion Cooldown Timer")]
    [SerializeField] private float _potionCooldownTimer;

    [Header("UI")]
    [SerializeField] private Button getPotionButton;

    public void GetRandomPotion()
    {
        if(!(GoldSystem.Instance.Gold >= potionCost))
            return;
        // Common - 70% | Uncommon - 20% | Rare - 7% | Epic - 2% | Legendary - 1%
        List<Items> potions = ItemSystem.Instance.ItemsCollection.FindAll(x => x.category == InventorySystem.Category.Potion);
        Items potion;        
        float rarity = UnityEngine.Random.Range(0f, 100f);
        if (rarity < 70f)
        {
            potion = potions.FindAll(x => x.rarity == Items.Rarity.Common)[UnityEngine.Random.Range(0, potions.FindAll(x => x.rarity == Items.Rarity.Common).Count)];
        }
        else if (rarity < 90f)
        {
            potion = potions.FindAll(x => x.rarity == Items.Rarity.Uncommon)[UnityEngine.Random.Range(0, potions.FindAll(x => x.rarity == Items.Rarity.Uncommon).Count)];
        }
        else if (rarity < 97f)
        {
            potion = potions.FindAll(x => x.rarity == Items.Rarity.Rare)[UnityEngine.Random.Range(0, potions.FindAll(x => x.rarity == Items.Rarity.Rare).Count)];
        }
        else if (rarity < 99f)
        {
            potion = potions.FindAll(x => x.rarity == Items.Rarity.Epic)[UnityEngine.Random.Range(0, potions.FindAll(x => x.rarity == Items.Rarity.Epic).Count)];
        }
        else if (rarity < 99.9f)
        {
            potion = potions.FindAll(x => x.rarity == Items.Rarity.Legendary)[UnityEngine.Random.Range(0, potions.FindAll(x => x.rarity == Items.Rarity.Legendary).Count)];
        }
        else
        {
            potion = potions.FindAll(x => x.rarity == Items.Rarity.Mythical)[UnityEngine.Random.Range(0, potions.FindAll(x => x.rarity == Items.Rarity.Mythical).Count)];
        }
        GoldSystem.Instance.SpendGold(potionCost);
        InventorySystem.Instance.AddItem(potion.id, 1);
        // StartCoroutine(GetPotionCooldown());
    }

    private IEnumerator GetPotionCooldown()
    {
        isPotionCooldown = true;
        if (_potionCooldownTimer <= 0)
            _potionCooldownTimer = getPotionCooldown;
        while (_potionCooldownTimer > 0)
        {
            _potionCooldownTimer -= Time.deltaTime;
            getPotionButton.interactable = false;
            getPotionButton.GetComponentInChildren<Text>().text = $"Cooldown {(int)_potionCooldownTimer / 60}m";
            yield return null;
        }
        getPotionButton.interactable = true;
        getPotionButton.GetComponentInChildren<Text>().text = "Get Potion";
        isPotionCooldown = false;
        yield return null;
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
        else if(currentPotion.potionType == Items.PotionType.Health)
        {
            HealthSystem.Instance.HealOverTime(currentPotion.potionValue, currentPotion.potionDuration);
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
        PlayerPrefs.DeleteKey("PotionID");
        PlayerPrefs.DeleteKey("PotionDuration");
        UISystem.Instance.UpdatePotionUI();
    }

    private void LoadPotion()
    {
        if (PlayerPrefs.HasKey("PotionID"))
        {
            currentPotion = ItemSystem.Instance.GetItem(PlayerPrefs.GetInt("PotionID"));
            _timer = PlayerPrefs.GetFloat("PotionDuration");
            AddPotionBonus();
        }

        // if (PlayerPrefs.HasKey("PotionCooldown"))
        // {
        //     _potionCooldownTimer = PlayerPrefs.GetFloat("PotionCooldown");
        //     if (_potionCooldownTimer > 0)
        //     {
        //         StartCoroutine(GetPotionCooldown());
        //     }
        // }
    }

    private void SavePotion()
    {
        if (isPotionCooldown)
            PlayerPrefs.SetFloat("PotionCooldown", _potionCooldownTimer);
        else 
            PlayerPrefs.DeleteKey("PotionCooldown");
        if (currentPotion == null)
            return;
        if (_timer <= 0)
        {
            RemovePotionBonus();
            return;
        }
        PlayerPrefs.SetInt("PotionID", currentPotion.id);
        PlayerPrefs.SetFloat("PotionDuration", _timer);
    }

    private IEnumerator PotionDuration()
    {
        if (_timer <= 0)
            _timer = currentPotion.potionDuration;
        while (_timer > 0)
        {
            _timer -= Time.deltaTime;
            UISystem.Instance.UpdatePotionUI();
            yield return null;
        }
        RemovePotionBonus();
        yield return null;
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
