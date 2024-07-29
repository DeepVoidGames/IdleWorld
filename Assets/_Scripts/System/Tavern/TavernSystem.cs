using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Hero
{
    public string name;
    public int level;
    public int maxLevel;
    public double baseDPS;
    public double baseCost;
    public double baseUpgradeCost;
    public double baseUpgradeDps;
    public double upgradeCostMultiplier;
    public double upgradeDpsMultiplier;
    public bool isUnlocked;
    public Sprite sprite;
    public GameObject prefab;

    public double cost {
        get {
            return baseCost * upgradeCostMultiplier * level;
        }
    }

    public double upgradeCost {
        get {
            return baseUpgradeCost * upgradeCostMultiplier * level;
        }
    }

    public double dps {
        get {
            return baseDPS * upgradeDpsMultiplier * level;
        }
    }
}

public class TavernSystem : MonoBehaviour
{
    private static TavernSystem _instance;
    public static TavernSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TavernSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("TavernSystem");
                    _instance = go.AddComponent<TavernSystem>();
                }
            }
            return _instance;
        }
    }

    public List<Hero> heroes;

    public List<GameObject> heroSpawnPoints;

    [Header("UI")]
    [SerializeField] private GameObject heroPanel;
    [SerializeField] private Image heroImage;    
    [SerializeField] private Text heroName;
    [SerializeField] private Text heroData;
    [SerializeField] private Text heroCost;
    [SerializeField] private Button upgradeButton;

    private void UpdateUI(int id)
    {
        heroImage.sprite = heroes[id].sprite;
        heroName.text = heroes[id].name;
        heroData.text = $"Level: {UISystem.Instance.NumberFormatInt(heroes[id].level)}\nDPS: {UISystem.Instance.NumberFormat(Instance.heroes[id].dps)}";
        heroCost.text = $"Cost: {UISystem.Instance.NumberFormat(heroes[id].cost)}";
        upgradeButton.onClick.RemoveAllListeners(); 
        if (heroes[id].level >= heroes[id].maxLevel)
        {
            heroCost.text = "Max Level";
            upgradeButton.interactable = false;
            upgradeButton.GetComponentInChildren<Text>().text = "Max Level";
            return;
        }
        if (heroes[id].isUnlocked)
        {
            upgradeButton.GetComponentInChildren<Text>().text = "Upgrade";
            upgradeButton.onClick.AddListener(() => UpgradeHero(id));
        }
        else
        {
            upgradeButton.GetComponentInChildren<Text>().text = "Hire";
            upgradeButton.onClick.AddListener(() => UnlockHero(id));
        }  
    }

    public void OpenPanel(int id)
    {
        upgradeButton.onClick.RemoveAllListeners();   
        heroPanel.SetActive(true);
        UpdateUI(id);
    }

    private double CalculateCost(int id)
    {
        return heroes[id].cost + (heroes[id].upgradeCost * heroes[id].upgradeCostMultiplier);
    }

    private double CalculateDps(int id)
    {
        return heroes[id].dps;
    }

    private void UpgradeHero(int id)
    {
        if (heroes[id].level >= heroes[id].maxLevel)
            return;
        if (heroes[id].cost > GoldSystem.Instance.Gold)
            return;
        heroes[id].level++;
        GoldSystem.Instance.SpendGold(heroes[id].cost);
        UpdateUI(id);
        UISystem.Instance.UpdateLevelText();
    }

    private void UnlockHero(int id)
    {
        if (heroes[id].cost > GoldSystem.Instance.Gold)
            return;
        heroes[id].level = 1;
        heroes[id].isUnlocked = true;
        GoldSystem.Instance.SpendGold(heroes[id].cost);
        SpawnHero(id);
        UpdateUI(id);
        UISystem.Instance.UpdateLevelText();
    }   

    public void SpawnHero(int id)
    {
        GameObject hero = Instantiate(heroes[id].prefab, heroSpawnPoints[id].transform.position, Quaternion.identity);
        hero.transform.SetParent(heroSpawnPoints[id].transform);
    }

}
