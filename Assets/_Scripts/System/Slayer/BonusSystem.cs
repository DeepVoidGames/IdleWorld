using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BonusType
{
    DamagePercentage,
    GoldPercentage,
    HealthBoost,
}

[System.Serializable]
public class Bonus 
{
    public string name;
    public double value;
    public string description;

    public BonusType type;
    public double amount;

    public Rarity rarity;
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythical,
    }
}

[System.Serializable]
public class UIBonus
{
    public BonusType bonusType;
    public GameObject panel;
    public Text title;
    public Text data;
}


// Common 70% Uncommon 20% Rare 7% Epic 2% Legendary 0.9% Mythical 0.1%
public class BonusSystem : MonoBehaviour 
{
    private static BonusSystem _instance;
    public static BonusSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BonusSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("BonusSystem");
                    _instance = go.AddComponent<BonusSystem>();
                }
            }
            return _instance;
        }
    }
    
    [Header("UI")]
    [SerializeField] private List<UIBonus> uiBonuses;
    [SerializeField] private List<Color> bonusColors;

    [Header("Bonuses Image Cards")]
    [SerializeField] private List<Sprite> bonusImagesCards;
    [SerializeField] private Sprite backImageCard;

    [Header("Bonuses Cards")]
    [SerializeField] private GameObject firstCard;
    [SerializeField] private GameObject secondCard;
    
    [Header("Bonuses")]
    public List<Bonus> bonuses = new List<Bonus>();

    private void Start()
    {
        LoadBonuses();
    }

    private int GetRandomValue(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public void ShowBonus()
    {
        MonsterSystem.Instance.PauseSpawning = true;
        MonsterSystem.Instance.DestroyMonster();
        // bonusPanel.SetActive(true);

        if (bonuses.Count < 2)
        {
            Debug.LogError("Not enough bonuses to display.");
            return;
        }

        Bonus firstBonus = GetRandomBonus();
        Bonus secondBonus = GetRandomBonus();

        SetCard(firstCard, firstBonus);
        SetCard(secondCard, secondBonus);
        
        if (SettingsSystem.Instance.AutomaticCardSelection)
        {
            ChooseBonus(firstBonus.value > secondBonus.value ? firstBonus.name : secondBonus.name);
        }
    }

    private Bonus GetRandomBonus()
    {
        float rarity = UnityEngine.Random.Range(0f, 100f);
        List<Bonus> selectedBonuses = rarity switch
        {
            < 70 => bonuses.FindAll(x => x.rarity == Bonus.Rarity.Common),
            < 90 => bonuses.FindAll(x => x.rarity == Bonus.Rarity.Uncommon),
            < 97 => bonuses.FindAll(x => x.rarity == Bonus.Rarity.Rare),
            < 99 => bonuses.FindAll(x => x.rarity == Bonus.Rarity.Epic),
            < 99.9f => bonuses.FindAll(x => x.rarity == Bonus.Rarity.Legendary),
            _ => bonuses.FindAll(x => x.rarity == Bonus.Rarity.Mythical),
        };

        return selectedBonuses[GetRandomValue(0, selectedBonuses.Count)];
    }

    private void ChooseBonus(string name)
    {
        Bonus bonus = bonuses.Find(x => x.name == name);
        if (bonus == null)
        {
            Debug.LogError("Bonus not found.");
            return;
        }

        bonus.amount++;
        PlayerPrefs.SetInt(name, PlayerPrefs.GetInt(name, 0) + 1);

        ApplyBonusEffect(bonus, bonus.value);
        UpdateBonusText();
        CloseCard();
    }

    private void ApplyBonusEffect(Bonus bonus, double amount = 1)
    {
        switch (bonus.type)
        {
            case BonusType.DamagePercentage:
                DifficultySystem.Instance.AddDamagePercentage(amount);
                break;
            case BonusType.GoldPercentage:
                DifficultySystem.Instance.GoldBonus += amount;
                break;
            case BonusType.HealthBoost:
                HealthSystem.Instance.AddHealthBoost(amount);
                break;
        }
    }

    public void RestartBonuses()
    {
        foreach (Bonus bonus in bonuses)
        {
            PlayerPrefs.DeleteKey(bonus.name);
            RemoveBonusEffect(bonus);
            bonus.amount = 0;
        }

        UpdateBonusText();
    }

    private void RemoveBonusEffect(Bonus bonus)
    {
        switch (bonus.type)
        {
            case BonusType.DamagePercentage:
                DifficultySystem.Instance.RemoveDamagePercentage((bonus.amount / 100) * bonus.value);
                break;
            case BonusType.GoldPercentage:
                DifficultySystem.Instance.GoldBonus -= (bonus.amount / 100) * bonus.value;
                break;
            case BonusType.HealthBoost:
                HealthSystem.Instance.RemoveHealthBoost(bonus.amount * bonus.value);
                break;
        }
    }

    private void LoadBonuses()
    {
        foreach (Bonus bonus in bonuses)
        {
            if (PlayerPrefs.HasKey(bonus.name))
            {
                bonus.amount = PlayerPrefs.GetInt(bonus.name);
                ApplyBonusEffect(bonus, bonus.value * bonus.amount);
            }
        }
        UpdateBonusText();
    }

    private void SetCard(GameObject card, Bonus bonus)
    {
        Text titleText = card.transform.Find("CardData/Title").GetComponent<Text>();
        Text descriptionText = card.transform.Find("CardData/Description").GetComponent<Text>();

        if (titleText == null || descriptionText == null)
        {
            Debug.LogError("Card title or description is missing.");
            return;
        }

        titleText.color = UISystem.Instance.GetRarityColor((Items.Rarity)bonus.rarity);
        titleText.text = bonus.name;

        switch (bonus.type)
        {
            case BonusType.DamagePercentage:
                descriptionText.text = string.Format(bonus.description, bonus.value * 100);
                break;
            case BonusType.GoldPercentage:
                descriptionText.text = string.Format(bonus.description, bonus.value * 100);
                break;
            default:
                descriptionText.text = string.Format(bonus.description, bonus.value);
                break;
        }

        Button cardButton = card.GetComponent<Button>();
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(() => ChooseBonus(bonus.name));

        card.GetComponent<Image>().sprite = bonusImagesCards[(int)bonus.rarity];
        card.SetActive(true);
        StartCoroutine(ChangeCardBackground(card));
    }

    private IEnumerator ChangeCardBackground(GameObject card)
    {
        yield return new WaitForSeconds(1);
        card.GetComponent<Image>().sprite = backImageCard;
    }

    private void CloseCard()
    {
        // bonusPanel.SetActive(false);
        ResetCard(firstCard);
        ResetCard(secondCard);
        MonsterSystem.Instance.PauseSpawning = false;
    }

    private void ResetCard(GameObject card)
    {
        card.transform.Find("CardData/Title").GetComponent<Text>().text = "";
        card.transform.Find("CardData/Description").GetComponent<Text>().text = "";
        card.transform.Find("CardData").gameObject.SetActive(false);
    }

    private Color ColorCalculation(double totalValue, int startValue)
    {
        int a = startValue;
        int index = 0;
        for (int i = 0; i < bonusColors.Count; i++)
        {
            if (totalValue >= a)
            {
                index = i;
            }
            else
                break;
            a = a * 10;
        }
        return bonusColors[index];
    }

    private void UpdateBonusText()
    {
        // bool hasActiveBonuses = false;
        // bonusText.text = "";

        foreach (UIBonus uiBonus in uiBonuses)
        {
            double totalValue = 0;
            foreach (Bonus bonus in bonuses)
            {
                if (bonus.type == uiBonus.bonusType && bonus.amount > 0)
                {
                    // hasActiveBonuses = true;
                    totalValue += bonus.amount * bonus.value;
                }
            }

            if (totalValue > 0)
            {
                uiBonus.panel.SetActive(true);
                uiBonus.title.text = uiBonus.bonusType switch
                {
                    BonusType.DamagePercentage => "Damage",
                    BonusType.GoldPercentage => "Gold",
                    BonusType.HealthBoost => "Health",
                    _ => uiBonus.title.text
                };

                uiBonus.data.text = uiBonus.bonusType switch
                {
                    BonusType.DamagePercentage => string.Format("{0}%", totalValue * 100),
                    BonusType.GoldPercentage => string.Format("{0}%", totalValue * 100),
                    BonusType.HealthBoost => totalValue.ToString(),
                    _ => uiBonus.data.text
                };

                uiBonus.data.color = uiBonus.bonusType switch
                {
                    BonusType.HealthBoost => ColorCalculation(totalValue, 100),
                    _ => ColorCalculation(totalValue, 10)
                };
            }
            else
            {
                uiBonus.panel.SetActive(false);
            }
        }
    }
}