using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Upgrade
{
    public string itemName;
    public int level;
    public int maxLevel;

    public int divineLevel;
    public int maxDivineLevel;

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
                    this.maxDivineLevel = 1;
                    break;
                case Items.Rarity.Uncommon:
                    this.maxLevel = 10;
                    this.maxDivineLevel = 2;
                    break;
                case Items.Rarity.Rare:
                    this.maxLevel = 15;
                    this.maxDivineLevel = 3;
                    break;
                case Items.Rarity.Epic:
                    this.maxLevel = 20;
                    this.maxDivineLevel = 4;
                    break;
                case Items.Rarity.Legendary:
                    this.maxLevel = 25;
                    this.maxDivineLevel = 5;
                    break;
                case Items.Rarity.Mythical:
                    this.maxLevel = 30;
                    this.maxDivineLevel = 6;
                    break;
                default:
                    this.maxLevel = 5;
                    this.maxDivineLevel = 1;
                    break;
            }
        }
        else
        {
            this.maxLevel = 5;
            this.maxDivineLevel = 1;
        }
    }

    public bool IsMaxLevel()
    {
        return level >= maxLevel;
    }

    public void IncreaseDivineLevel()
    {
        if (divineLevel < maxDivineLevel)
        {
            divineLevel++;
            level = 0; // Reset normal level
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
    [SerializeField] private Button upgradeButton;

    [Header("Upgrading")]
    public Items UpgradeSlot;
    [SerializeField] private Upgrade upgrade;
    private bool isUpgrading = false;

    [SerializeField] private float divineLevelMultiplier = 5f;
    private int divineLevelRequirement = 50;

    public void Upgrade()
    {
        if (UpgradeSlot == null || isUpgrading || upgrade == null)
            return;

        isUpgrading = true;

        if (upgrade.IsMaxLevel())
        {
            // Handle divine upgrade
            PerformDivineUpgrade();
        }
        else
        {
            // Handle normal upgrade
            PerformUpgrade();
        }

        isUpgrading = false;
    }   

    private void PerformUpgrade()
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
            resultText.text = "Success!";
        }
        else
        {
            resultText.text = "Failed!";
        }
    }

    private void PerformDivineUpgrade()
    {
        // Divine upgrade requires a different level to upgrade
        if (upgrade.divineLevel >= upgrade.maxDivineLevel)
        {
            resultText.text = "Max divine level reached!";
            isUpgrading = false;
            return;
        }

        if (upgrade.level < upgrade.maxLevel)
        {
            resultText.text = "You need to reach max level to perform divine upgrade!";
            isUpgrading = false;
            return;
        }

        int _d = upgrade.divineLevel;

        if (upgrade.divineLevel == 0)
        {
            _d = 1;
        }

        if (!(LevelSystem.Instance.Level >= _d * divineLevelRequirement))  
        {
            resultText.text = $"You need to be level {_d * divineLevelRequirement} to perform divine upgrade!";
            isUpgrading = false;
            return;
        }
        upgrade.IncreaseDivineLevel();
        UpdateUI();
        resultText.text = "Success!";
    }

    private void UpdateUI()
    {
        if (UpgradeSlot != null)
        {
            statsText.text = $"Level: {upgrade.level} / {upgrade.maxLevel}\nDivine Level: {upgrade.divineLevel} / {upgrade.maxDivineLevel}\nBonus: {UISystem.Instance.NumberFormat(GetBonus(UpgradeSlot.Name, UpgradeSlot.baseDamage))}";
            if (upgrade.IsMaxLevel() && upgrade.divineLevel < upgrade.maxDivineLevel)
            {
                costText.gameObject.SetActive(false);
                chanceText.text = "Ready for divine upgrade!";
                upgradeButton.GetComponentInChildren<Text>().text = "Divine Upgrade";
            }
            else if (upgrade.IsMaxLevel())
            {
                costText.gameObject.SetActive(false);
                chanceText.text = "Max level reached!";
                upgradeButton.GetComponentInChildren<Text>().text = "Max Level";
                upgradeButton.interactable = false;
            }
            else
            {
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
            chance = 100 - (upgrade.level * 4f);
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
                else if (upgrade.divineLevel > 0)
                    return item.baseDamage * 1000 * (upgrade.divineLevel * divineLevelMultiplier) * upgrade.level;
                else
                    return item.baseDamage * 1000 * upgrade.level;
            }
            else if(item.baseMiningEfficiency > 0)
            {
                if (upgrade.level == 0)
                    return item.baseMiningEfficiency * 1000;
                else if (upgrade.divineLevel > 0)
                    return item.baseMiningEfficiency * 1000 * (upgrade.divineLevel * divineLevelMultiplier) * upgrade.level;
                else
                    return item.baseMiningEfficiency * 1000 * upgrade.level;
            }
        }
        return cost;
    }

    private int CalculateMaxLevel(string itemName, bool divineUpgrade)
    {

        Items item = ItemSystem.Instance.GetItemByName(itemName);
        if (item != null)
        {
            switch (item.rarity)
            {
                case Items.Rarity.Common:
                    if (divineUpgrade)
                        return 1;
                    return 5;
                case Items.Rarity.Uncommon:
                    if (divineUpgrade)
                        return 2;
                    return 10;
                case Items.Rarity.Rare:
                    if (divineUpgrade)
                        return 3;
                    return 15;
                case Items.Rarity.Epic:
                    if (divineUpgrade)
                        return 4;
                    return 20;
                case Items.Rarity.Legendary:
                    if (divineUpgrade)
                        return 5;
                    return 25;
                case Items.Rarity.Mythical:
                    if (divineUpgrade)
                        return 6;
                    return 30;
                default:
                    if (divineUpgrade)
                        return 1;
                    return 5;
            }
        }
        else
        {
            if (divineUpgrade)
                return 1;
            return 5;
        }
    }

    private void ClearUpgradeSlot()
    {
        UpgradeSlot = null;
        upgradeSlotImage.sprite = null;
        itemNameText.text = "";
        statsText.text = "";
        resultText.text = "";
        chanceText.text = "";
        upgradeButton.interactable = true;
        upgradeButton.GetComponentInChildren<Text>().text = "Upgrade";
    }

    public void ImageOnClick()
    {
        if (!InventorySystem.Instance.InventoryPanel.activeSelf)
            InventorySystem.Instance.InventoryPanel.SetActive(true);
        else
            ClearUpgradeSlot();
    }

    public float GetBonus(string name, float baseDamage)
    {
        //! 
        float bonusMultiplier = 0.1f;

        Upgrade upgrade = Upgrades.Find(x => x.itemName == name);
        Items item = ItemSystem.Instance.GetItemByName(name);

        if (upgrade == null)
            return 0;
        if(item == null)
            return 0;

        if (item.baseDamage != 0)
        {
            if (baseDamage > 0 && upgrade.divineLevel <= 0)
                return baseDamage * (bonusMultiplier * upgrade.level);
            else if (baseDamage > 0 && (upgrade.divineLevel <= upgrade.maxDivineLevel))
                return baseDamage * (bonusMultiplier * (upgrade.maxLevel * upgrade.divineLevel + upgrade.level));
        }

        if (item.baseMiningEfficiency != 0)
        {
            if(baseDamage > 0 && upgrade.divineLevel <= 0)
                return baseDamage * (bonusMultiplier * upgrade.level);
            else if (baseDamage > 0 && (upgrade.divineLevel <= upgrade.maxDivineLevel))
                return baseDamage * (bonusMultiplier * (upgrade.maxLevel * upgrade.divineLevel + upgrade.level));
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

    public int GetDivineLevel(string name)
    {
        Upgrade upgrade = Upgrades.Find(x => x.itemName == name);
        if (upgrade != null)
            return upgrade.divineLevel;
        return 0;
    }

    public void SetUpgradeSlot(Items item)
    {
        ClearUpgradeSlot();
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
        foreach (Upgrade upgrade in upgrades)
        {
            Upgrade u = Upgrades.Find(x => x.itemName == upgrade.itemName);
            if (u == null)
            {
                if(upgrade.maxLevel == 0)
                    upgrade.maxLevel = CalculateMaxLevel(upgrade.itemName, false);
                else if (upgrade.maxDivineLevel == 0)
                    upgrade.maxDivineLevel = CalculateMaxLevel(upgrade.itemName, true);
                Upgrades.Add(upgrade);
            }
            else
            {
                u.level = upgrade.level;
                u.divineLevel = upgrade.divineLevel;
            }
        }

        if(DamageSystem.Instance.IsWeaponEquipped)
            DamageSystem.Instance.UpdateWeaponDamage();
    }
}
