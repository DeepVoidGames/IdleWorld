using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Bonus 
{
    public string name;
    public double value;

    public bool isBonusDamage;

    public double amount;
}

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
    
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private Text bonusText;

    private double _bonusDamage;

    [SerializeField] private GameObject firstCard;
    [SerializeField] private GameObject secondCard;
    
    public List<Bonus> bonuses = new List<Bonus>();


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

        Bonus firstBonus = bonuses[UnityEngine.Random.Range(0, bonuses.Count)];
        Bonus secondBonus = bonuses[UnityEngine.Random.Range(0, bonuses.Count)];

        SetCard(firstCard, firstBonus.name, firstBonus.value.ToString());
        SetCard(secondCard, secondBonus.name, secondBonus.value.ToString());
        
    }

    private void SetCard(GameObject card, string title, string description)
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

    private void UpdateBonusText()
    {
        bonusText.text = $"Bonus Damage: {_bonusDamage * 100}%";
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
            }
        }
        UpdateBonusText();
    }

    private void Start()
    {
        LoadBonus();
    }

}