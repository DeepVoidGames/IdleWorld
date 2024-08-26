using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Bonus 
{
    public string name;
    public double value;
    public string description;

    public bool isBonusDamage;
    public bool isBonusGold;

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
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private GameObject bonusTextPanel;
    [SerializeField] private Text bonusText;
    [Header("Bonuses Image Cards")]
    [SerializeField] private List<Sprite> bonusImagesCards;
    [SerializeField] private Sprite backImageCard;

    private double _bonusDamage;
    private double _bonusGold;

    [Header("Bonuses Cards")]
    [SerializeField] private GameObject firstCard;
    [SerializeField] private GameObject secondCard;
    
    [Header("Bonuses")]
    public List<Bonus> bonuses = new List<Bonus>();


    private int RandomValue(int min, int max)
    {
        int value = UnityEngine.Random.Range(min, max);
        return value;
    }

    public void ShowBonus()
    {
        MonsterSystem.Instance.PauseSpawning = true;
        MonsterSystem.Instance.DestroyMonster();
        bonusPanel.SetActive(true);

        if(bonuses.Count == 0)
        {
            Debug.LogError("Bonuses is empty");
            return;
        }

        if(bonuses.Count < 2)
        {
            Debug.LogError("Bonuses is less than 2");
            return;
        }

        Bonus firstBonus = GetRandomBonus();
        Bonus secondBonus = GetRandomBonus();

        SetCard(firstCard, firstBonus.name, String.Format(firstBonus.description, firstBonus.value * 100), (int)firstBonus.rarity);
        SetCard(secondCard, secondBonus.name, String.Format(secondBonus.description, secondBonus.value * 100), (int)secondBonus.rarity);
        
        if (SettingsSystem.Instance.AutomaticCardSelection)
        {
            //TODO If rarity was implemented, choose the card with the highest rarity
            if (firstBonus.value > secondBonus.value)
                ChooseBonus(firstBonus.name);
            else
                ChooseBonus(secondBonus.name);
        }
    }

    private Bonus GetRandomBonus()
    {
        int rarity = RandomValue(0, 100);
        if (rarity < 70)
        {
            List<Bonus> commonBonuses = bonuses.FindAll(x => x.rarity == Bonus.Rarity.Common);
            return commonBonuses[RandomValue(0, commonBonuses.Count)];
        }
        else if (rarity < 90)
        {
            List<Bonus> uncommonBonuses = bonuses.FindAll(x => x.rarity == Bonus.Rarity.Uncommon);
            return uncommonBonuses[RandomValue(0, uncommonBonuses.Count)];
        }
        else if (rarity < 97)
        {
            List<Bonus> rareBonuses = bonuses.FindAll(x => x.rarity == Bonus.Rarity.Rare);
            return rareBonuses[RandomValue(0, rareBonuses.Count)];
        }
        else if (rarity < 99)
        {
            List<Bonus> epicBonuses = bonuses.FindAll(x => x.rarity == Bonus.Rarity.Epic);
            return epicBonuses[RandomValue(0, epicBonuses.Count)];
        }
        else if (rarity < 99.9)
        {
            List<Bonus> legendaryBonuses = bonuses.FindAll(x => x.rarity == Bonus.Rarity.Legendary);
            return legendaryBonuses[RandomValue(0, legendaryBonuses.Count)];
        }
        else
        {
            List<Bonus> mythicalBonuses = bonuses.FindAll(x => x.rarity == Bonus.Rarity.Mythical);
            return mythicalBonuses[RandomValue(0, mythicalBonuses.Count)];
        }
    }

    private void ChooseBonus(string name)
    {
        Bonus bonus = bonuses.Find(x => x.name == name);
        if(bonus == null)
        {
            Debug.LogError("Bonus is null");
            return;
        }
        bonuses.Find(x => x.name == name).amount++;

        if(PlayerPrefs.HasKey(name))
            PlayerPrefs.SetInt(name, PlayerPrefs.GetInt(name) + 1);
        else
            PlayerPrefs.SetInt(name, 1);

        if(bonus.isBonusDamage)
        {
            DifficultySystem.Instance.AddDamagePercentage(bonus.value);
            _bonusDamage += bonus.value;
        }

        if(bonus.isBonusGold)
        {
            DifficultySystem.Instance.GoldBonus += bonus.value;
            _bonusGold += bonus.value;
        }

        UpdateBonusText();
        CloseCard();
    }

    public void RestartBonus()
    {
        _bonusDamage = 0;

        foreach(Bonus bonus in bonuses)
        {
            PlayerPrefs.DeleteKey(bonus.name);
            if(bonus.isBonusDamage)
                DifficultySystem.Instance.RemoveDamagePercentage(bonus.amount * bonus.value);
            bonus.amount = 0;
        }

        UpdateBonusText();
    }

    private void LoadBonus()
    {
        foreach(Bonus bonus in bonuses)
        {
            if(PlayerPrefs.HasKey(bonus.name))
            {
                bonus.amount = PlayerPrefs.GetInt(bonus.name);

                if (bonus.isBonusDamage)
                {
                    DifficultySystem.Instance.AddDamagePercentage(bonus.amount * bonus.value);
                    _bonusDamage += bonus.amount * bonus.value;
                }

                if (bonus.isBonusGold)
                {
                    DifficultySystem.Instance.GoldBonus += bonus.amount * bonus.value;
                    _bonusGold += bonus.amount * bonus.value;

                }
            }
        }
        UpdateBonusText();
    }

    private void SetCard(GameObject card, string title, string description, int rarityIndex)
    {
        Text titleText = card.transform.Find("CardData").Find("Title").GetComponent<Text>();
        Text descriptionText = card.transform.Find("CardData").Find("Description").GetComponent<Text>();

        if(titleText == null || descriptionText == null)
        {
            Debug.LogError("Title or Description is null");
            return;
        }

        titleText.text = title;
        descriptionText.text = description;

        card.GetComponent<Button>().onClick.RemoveAllListeners();
        card.GetComponent<Button>().onClick.AddListener(() => ChooseBonus(title));

        card.GetComponent<Image>().sprite = bonusImagesCards[rarityIndex];
        card.transform.gameObject.SetActive(true);
        StartCoroutine(ChangeCardBackgroud(card));


    }

    private IEnumerator ChangeCardBackgroud(GameObject card)
    {
        yield return new WaitForSeconds(1);
        card.GetComponent<Image>().sprite = backImageCard;
    }

    private void CloseCard()
    {
        bonusPanel.SetActive(false);
        firstCard.transform.Find("CardData").Find("Title").GetComponent<Text>().text = "";
        firstCard.transform.Find("CardData").Find("Description").GetComponent<Text>().text = "";
        firstCard.transform.Find("CardData").gameObject.SetActive(false);

        secondCard.transform.Find("CardData").Find("Title").GetComponent<Text>().text = "";
        secondCard.transform.Find("CardData").Find("Description").GetComponent<Text>().text = "";
        secondCard.transform.Find("CardData").gameObject.SetActive(false);

        MonsterSystem.Instance.PauseSpawning = false;
    }

    private void UpdateBonusText()
    {
        if (_bonusDamage == 0 && _bonusGold == 0)
        {
            bonusTextPanel.SetActive(false);
            return;
        }
        string _bonusText = "";
    
        if (_bonusDamage != 0)
            _bonusText += $"Bonus Gold: {_bonusGold * 100}%\n";
        if (_bonusGold != 0)
            _bonusText += $"Bonus Damage: {_bonusDamage * 100}%\n";
    
        bonusText.text = _bonusText;
        bonusTextPanel.SetActive(true);
    }

    private void Start()
    {
        LoadBonus();
    }

}