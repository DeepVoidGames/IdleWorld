using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private Coroutine upgradeCoroutine;

    private int currentHero = -1;

    private void UpdateUI(int id)
    {
        heroImage.sprite = heroes[id].sprite;
        heroName.text = heroes[id].name;
        heroData.text = $"Level: {UISystem.Instance.NumberFormatInt(heroes[id].level)}/{UISystem.Instance.NumberFormatInt(heroes[id].maxLevel)}\nHero Damage: {UISystem.Instance.NumberFormat(Instance.heroes[id].dps)}";
        if (heroes[id].isUnlocked)
            heroCost.text = $"Cost: {UISystem.Instance.NumberFormat(heroes[id].upgradeCost)}";
        else
            heroCost.text = $"Cost: {UISystem.Instance.NumberFormat(heroes[id].cost)}";
        upgradeButton.onClick.RemoveAllListeners(); 

        if (heroes[id].isUnlocked)
        {
            upgradeButton.GetComponentInChildren<Text>().text = "Upgrade";
            upgradeButton.interactable = true;
            // Remove all listeners to prevent multiple listeners
            // upgradeButton.onClick.AddListener(() => UpgradeHero(id));

            EventTrigger trigger = upgradeButton.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = upgradeButton.gameObject.AddComponent<EventTrigger>();
            }
            trigger.triggers.Clear();
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data, id); });
            trigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
            trigger.triggers.Add(pointerUpEntry);
        }
        else
        {
            upgradeButton.GetComponentInChildren<Text>().text = "Hire";
            upgradeButton.interactable = true;
            upgradeButton.onClick.AddListener(() => UnlockHero(id));
        }  
    }

    private void UpdateBuyButtonUI(double gold)
    {
        if (currentHero < 0)
            return;
        int id = currentHero;
        if (heroes[id].level >= heroes[id].maxLevel)
        {
            heroCost.text = "Max Level";
            upgradeButton.interactable = false;
            upgradeButton.GetComponentInChildren<Text>().text = "Max Level";
            upgradeButton.GetComponent<Image>().color = UISystem.Instance.buyButtonMaxedColor;
            return;
        }

        if (heroes[id].upgradeCost > GoldSystem.Instance.Gold)
        {
            upgradeButton.GetComponentInChildren<Text>().text = "Not enough gold";
            upgradeButton.GetComponent<Image>().color = UISystem.Instance.buyButtonDisabledColor;
            return;
        }
        upgradeButton.GetComponentInChildren<Text>().text = "Upgrade";
        upgradeButton.GetComponent<Image>().color = UISystem.Instance.buyButtonColor;
    }

    public void OpenPanel(int id)
    {
        upgradeButton.onClick.RemoveAllListeners();   
        heroPanel.SetActive(true);
        currentHero = id;
        UpdateUI(id);
        UpdateBuyButtonUI(GoldSystem.Instance.Gold);
    }
    
    public void ClosePanel()
    {
        heroPanel.SetActive(false);
        currentHero = -1;
    }

    private void UpgradeHero(int id)
    {
        if (heroes[id].level >= heroes[id].maxLevel)
            return;
        if (heroes[id].upgradeCost > GoldSystem.Instance.Gold)
            return;
        heroes[id].level++;
        GoldSystem.Instance.SpendGold(heroes[id].upgradeCost);
        currentHero = id;
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
        currentHero = id;
        SpawnHero(id);
        UpdateUI(id);
        UISystem.Instance.UpdateLevelText();
    }   

    public void SpawnHero(int id)
    {
        GameObject hero = Instantiate(heroes[id].prefab, heroSpawnPoints[id].transform.position, Quaternion.identity);
        hero.transform.SetParent(heroSpawnPoints[id].transform);
    }

    private void OnPointerDown(PointerEventData data, int id)
    {
        upgradeCoroutine = StartCoroutine(ContinuousUpgrade(id));
    }

    private void OnPointerUp(PointerEventData data)
    {
        if (upgradeCoroutine != null)
        {
            StopCoroutine(upgradeCoroutine);
            upgradeCoroutine = null;
        }
    }

    private IEnumerator ContinuousUpgrade(int id)
    {
        float waitTime = 0.2f; // Initial wait time
        float minWaitTime = 0.1f; // Minimum wait time
        float speedUpFactor = 0.01f; // Factor to speed up the upgrade

        while (true)
        {
            UpgradeHero(id);
            yield return new WaitForSeconds(waitTime);
            waitTime = Mathf.Max(minWaitTime, waitTime * speedUpFactor); // Decrease wait time but not below minWaitTime
        }
    }

    private void Start()
    {
        GoldSystem.Instance.OnGoldChanged += UpdateBuyButtonUI;
    }

    private void OnDestroy()
    {
        GoldSystem.Instance.OnGoldChanged -= UpdateBuyButtonUI;
    }
}
