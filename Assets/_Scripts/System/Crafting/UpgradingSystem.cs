using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Upgrade
{
    public string itemName;
    public int level;
    public int maxLevel;

    public Upgrade(string itemName, int level)
    {
        this.itemName = itemName;
        this.level = level;
        //! Max level depends on the item rarity
        Items item = ItemSystem.Instance.GetItemByName(itemName);
        if (item != null)
        {
            switch (item.rarity)
            {
                case Items.Rarity.Common:
                    this.maxLevel = 5;
                    break;
                case Items.Rarity.Uncommon:
                    this.maxLevel = 10;
                    break;
                case Items.Rarity.Rare:
                    this.maxLevel = 15;
                    break;
                case Items.Rarity.Epic:
                    this.maxLevel = 20;
                    break;
                case Items.Rarity.Legendary:
                    this.maxLevel = 25;
                    break;
                case Items.Rarity.Mythical:
                    this.maxLevel = 30;
                    break;
                default:
                    this.maxLevel = 5;
                    break;
            }
        }
        else
        {
            this.maxLevel = 5;
        }
    }
}

public class UpgradingSystem : MonoBehaviour
{
    private static UpgradingSystem instance;
    public static UpgradingSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UpgradingSystem>();
            }
            return instance;
        }
    }

    [Header("Upgrades")]
    public List<Upgrade> Upgrades = new List<Upgrade>();

    [Header("UI Elements")]
    [SerializeField] private Image upgradeSlotImage;
    [SerializeField] private Text itemNameText;

    [SerializeField] private Text statsText;
    [SerializeField] private Text resultText;
    [SerializeField] private Text chanceText;
    [SerializeField] private Text costText;

    [Header("Upgrading")]
    public Items UpgradeSlot;
    [SerializeField] private Upgrade upgrade;
    private bool isUpgrading = false;

    public void Upgrade()
    {
        if (UpgradeSlot == null)
            return;
        if (isUpgrading)
            return;
        if (upgrade == null)
            return;
        
        isUpgrading = true;

        if (upgrade == null)
        {
            upgrade = new Upgrade(UpgradeSlot.Name, 0);
            Upgrades.Add(upgrade);
        }

        if (upgrade.level < upgrade.maxLevel)
        {
            double cost = CalculateGoldCost();
            if (GoldSystem.Instance.Gold >= cost)
            {
                GoldSystem.Instance.SpendGold(cost);
            }
            else
            {
                resultText.text = "Not enough gold!";
                isUpgrading = false;
                return;
            }

            double chance = CalculateChance();
            double random = Random.Range(0, 100);
            if (random <= chance)
            {
                upgrade.level++;
                UpdateUI();
                statsText.text = "Level: " + upgrade.level + " / " + upgrade.maxLevel;
                resultText.text = "Success!";
            }
            else
            {
                resultText.text = "Failed!";
            }
        }
        else
        {
            resultText.text = "Max level reached!";
        }
        isUpgrading = false;
    }

    private void UpdateUI()
    {
        if (UpgradeSlot != null)
        {
            if (upgrade.level == upgrade.maxLevel)
            {
                statsText.text = $"Level: {upgrade.level} / {upgrade.maxLevel} (Max)\n" +
                                 $"Bonus: {GetBonus(UpgradeSlot.Name, UpgradeSlot.baseDamage)}";
                costText.gameObject.SetActive(false);
                chanceText.text = "Max level reached!";
            }
            else
            {
                statsText.text = $"Level: {upgrade.level} / {upgrade.maxLevel}\n" +
                                 $"Bonus: {GetBonus(UpgradeSlot.Name, UpgradeSlot.baseDamage)}";
                costText.text = "Cost: " + UISystem.Instance.NumberFormat(CalculateGoldCost());
                costText.gameObject.SetActive(true);
                chanceText.text = "Chance: " + CalculateChance() + "%";
            }
        }
        InventorySystem.Instance.UpdateUI();
    }

    private double CalculateChance()
    {
        if (upgrade == null)
            return 0;
        double chance = 0;
        if (upgrade.level < upgrade.maxLevel)
        {
            if (upgrade.level == 0)
                return 100;
            chance = 100 - (upgrade.level * 5);
            if (chance <= 0)
                chance = 1;
        }
        return chance;
    }

    private double CalculateGoldCost()
    {
        if (upgrade == null)
            return 0;
        double cost = 0;
        Items item = ItemSystem.Instance.GetItemByName(upgrade.itemName);
        if (upgrade.level < upgrade.maxLevel)
        {
            if (item == null)
                return 0;
            if (item.baseDamage > 0)
            {   if(upgrade.level == 0)
                    return item.baseDamage * 1000;
                return item.baseDamage * 1000 * upgrade.level;
            }
            //TODO Add support for tools
        }
        return cost;
    }

    private void ClearUpgradeSlot()
    {
        UpgradeSlot = null;
        upgradeSlotImage.sprite = null;
        itemNameText.text = "";
        statsText.text = "";
        resultText.text = "";
        chanceText.text = "";
    }

    public void ImageOnClick()
    {
        if (!InventorySystem.Instance.InventoryPanel.activeSelf)
        {
            InventorySystem.Instance.SetCategory(2);
            InventorySystem.Instance.InventoryPanel.SetActive(true);
        }
        else
            ClearUpgradeSlot();
    }

    public void SetUpgradeSlot(Items item)
    {
        UpgradeSlot = item;
        upgradeSlotImage.sprite = item.icon;
        itemNameText.text = item.Name;
        upgrade = Upgrades.Find(x => x.itemName == item.Name);
        if (upgrade == null)
        {
            upgrade = new Upgrade(item.Name, 0);
            Upgrades.Add(upgrade);
        }
        UpdateUI();
    }

    public void SetUpgrades(List<Upgrade> upgrades)
    {
        Upgrades = upgrades;
    }

    public float GetBonus(string name, float baseDamage)
    {
        Upgrade upgrade = Upgrades.Find(x => x.itemName == name);
        if (upgrade != null)
        {
            if (baseDamage > 0)
                return baseDamage * (0.1f * upgrade.level);
        }
        return 0;
    }

    public int GetLevel(string name)
    {
        Upgrade upgrade = Upgrades.Find(x => x.itemName == name);
        if (upgrade != null)
            return upgrade.level;
        return 0;
    }
}
