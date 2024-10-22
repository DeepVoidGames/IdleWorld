using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeSlayer
{
    public string name;
    public double value;
    public double basePrice;

    public float level;
    public float maxLevel;

    public float priceMultiplier;

    public double Price
    {
        get => basePrice + (basePrice * priceMultiplier) * level;
    }

    public enum UpgradeType
    {
        Damage,
        Health,
        AttackSpeed
    }
    public UpgradeType type;

    public GameObject panel;
}

public class UpgradesSlayerSystem : MonoBehaviour 
{
    private static UpgradesSlayerSystem _instance;
    public static UpgradesSlayerSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UpgradesSlayerSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("UpgradesSlayerSystem");
                    _instance = go.AddComponent<UpgradesSlayerSystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private List<UpgradeSlayer> upgrades;
    private Coroutine upgradeCoroutine;


    private void UIUpdate(UpgradeSlayer upgrade)
    {
        upgrade.panel.transform.Find("Title").GetComponent<UnityEngine.UI.Text>().text = upgrade.name;
        upgrade.panel.transform.Find("Level").GetComponent<UnityEngine.UI.Text>().text =  $"Level: {UISystem.Instance.NumberFormat(upgrade.level)}";

        string format;
        switch (upgrade.type)
        {
            case UpgradeSlayer.UpgradeType.Damage:
                format = "Damage Boost Per Level +{0}\nAll Damage Boost +{1}";
                break;
            case UpgradeSlayer.UpgradeType.Health:
                format = "Health Boost Per Level +{0}\nAll Health Boost +{1}";
                break;
            case UpgradeSlayer.UpgradeType.AttackSpeed:
                format = "Attack Speed Boost Per Level +{0}\nAll Attack Speed Boost +{1}";
                break;
            default:
                format = "";
                break;
        }

        upgrade.panel.transform.Find("Bonus").GetComponent<UnityEngine.UI.Text>().text = String.Format(format, upgrade.value, UISystem.Instance.NumberFormat(upgrade.value * upgrade.level));

        upgrade.panel.transform.Find("Upgrade").Find("Price").GetComponent<UnityEngine.UI.Text>().text = $"Buy\n{UISystem.Instance.NumberFormat(upgrade.Price)}";

        Button upgradeButton = upgrade.panel.transform.Find("Upgrade").GetComponent<UnityEngine.UI.Button>();
        EventTrigger trigger = upgradeButton.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = upgradeButton.gameObject.AddComponent<EventTrigger>();
        }
        trigger.triggers.Clear();

        if (upgrade.level < upgrade.maxLevel)
        {
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data, upgrade.name); });
            trigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
            trigger.triggers.Add(pointerUpEntry);
        }

        UIButtonUpdate(GoldSystem.Instance.Gold);
    }

    private void UIButtonUpdate(double gold)
    {
        foreach (UpgradeSlayer upgrade in upgrades)
        {
            // Change color of button if player can afford upgrade
            if (upgrade.level >= upgrade.maxLevel)
            {
                upgrade.panel.transform.Find("Upgrade").GetComponent<UnityEngine.UI.Image>().color = UISystem.Instance.buyButtonMaxedColor;
                upgrade.panel.transform.Find("Upgrade").Find("Price").GetComponent<UnityEngine.UI.Text>().text = "Maxed";
                upgrade.panel.transform.Find("Upgrade").GetComponent<UnityEngine.UI.Button>().interactable = false;
                continue;
            }
            
            if (upgrade.Price >= gold)
            {
                upgrade.panel.transform.Find("Upgrade").GetComponent<UnityEngine.UI.Image>().color = UISystem.Instance.buyButtonDisabledColor;
                upgrade.panel.transform.Find("Upgrade").GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
            else
            {
                upgrade.panel.transform.Find("Upgrade").GetComponent<UnityEngine.UI.Image>().color = UISystem.Instance.buyButtonColor;
                upgrade.panel.transform.Find("Upgrade").GetComponent<UnityEngine.UI.Button>().interactable = true;
                
            }
        }
    }

    private UpgradeSlayer GetUpgradeByName(string name)
    {
        return upgrades.Find(x => x.name == name);
    }

    private void OnPointerDown(PointerEventData data, string name)
    {
        upgradeCoroutine = StartCoroutine(UpgradeCoroutine(name));
    }

    private void OnPointerUp(PointerEventData data)
    {
        if (upgradeCoroutine != null)
        {
            StopCoroutine(upgradeCoroutine);
            upgradeCoroutine = null;
        }
    }
    
    IEnumerator UpgradeCoroutine(string name)
    {
        float waitTime = 0.2f; // Initial wait time
        float minWaitTime = 0.1f; // Minimum wait time
        float speedUpFactor = 0.01f; // Factor to speed up the upgrade

        while (true)
        {
            UpgradeSlayer upgrade = GetUpgradeByName(name);
            if (upgrade.Price > GoldSystem.Instance.Gold)
                break;

            if (upgrade.level <= upgrade.maxLevel)
                upgrade.level++;
            else 
                break;

            GoldSystem.Instance.SpendGold(upgrade.Price);
            AddBonus(upgrade);
            yield return new WaitForSeconds(waitTime);
            waitTime = Mathf.Max(minWaitTime, waitTime * speedUpFactor); // Decrease wait time but not below minWaitTime
        }
    }

    private void AddBonus(UpgradeSlayer upgrade, bool isLoad = false)
    {
        switch (upgrade.type)
        {
            case UpgradeSlayer.UpgradeType.Damage:
                if (isLoad)
                {
                    DamageSystem.Instance.AddDamageBoost(upgrade.value * upgrade.level);
                }
                else
                {
                    DamageSystem.Instance.AddDamageBoost(upgrade.value);
                }
                break;
            case UpgradeSlayer.UpgradeType.Health:
                if (isLoad)
                {
                    HealthSystem.Instance.AddHealthBoost(upgrade.value * upgrade.level);
                }
                else
                {
                    HealthSystem.Instance.AddHealthBoost(upgrade.value);
                }
                break;
            case UpgradeSlayer.UpgradeType.AttackSpeed:
                if (isLoad)
                {
                    DamageSystem.Instance.AttackSpeed -= (float)(upgrade.value * upgrade.level);
                }
                else
                {
                    DamageSystem.Instance.AttackSpeed -= (float)upgrade.value;
                }
                break;
            default:
                break;
        }
        UIUpdate(upgrade);
    }

    private void LoadBonuses()
    {
        foreach (UpgradeSlayer upgrade in upgrades)
        {
            upgrade.level = PlayerPrefs.GetFloat(upgrade.name, 0);
            if (upgrade.level > upgrade.maxLevel)
            {
                upgrade.level = upgrade.maxLevel;
            }
            AddBonus(upgrade, true);
        }
    }

    private void SaveBonuses()
    {
        foreach (UpgradeSlayer upgrade in upgrades)
        {
            PlayerPrefs.SetFloat(upgrade.name, upgrade.level);
        }
    }

    private void Start()
    {
        LoadBonuses();
        foreach (UpgradeSlayer upgrade in upgrades)
        {
            UIUpdate(upgrade);
        }
        GoldSystem.Instance.OnGoldChanged += UIButtonUpdate;
        UIButtonUpdate(GoldSystem.Instance.Gold);
    }

    private void OnApplicationQuit()
    {
        SaveBonuses();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveBonuses();
    }

    private void OnDestroy()
    {
        GoldSystem.Instance.OnGoldChanged -= UIButtonUpdate;
    }
}