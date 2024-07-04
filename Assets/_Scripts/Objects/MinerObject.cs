using UnityEngine.UI;
using UnityEngine;
using System;
public class MinerObject: MonoBehaviour
{
    [SerializeField]
    private string minerName;

    [Header("Resource")]
    [SerializeField]
    private string resourceName;
    [SerializeField]
    private bool payWithMoney;
    [SerializeField]
    private RockSystem rockSystem;

    [Header("UI Elements")]
    [SerializeField]
    private Text minerDescription;
    [SerializeField]
    private Text buttonText;
    [SerializeField]
    private Image buttonImage;

    private Miner miner;
    private float timer;


    void Start()
    {
        if (!MinerSystem.Instance.minersDict.ContainsKey(minerName))
        {
            Debug.Log("Miner " + minerName + " not found in miners.");
            return;
        }
        miner = MinerSystem.Instance.minersDict[minerName];
        Debug.Log("Miner: " + miner.minerName + " Amount: " + miner.amount + " Production Rate: " + miner.productionRate);
        ButtonTextChange();
        MinerDescriptionChange();

    }

    void Update()
    {
        if (miner.amount > 0)
        {
            ButtonChangeColor();
            timer += Time.deltaTime;
            if (timer >= miner.productionTime)
            {
                rockSystem.DamageCurrentRock(miner.productionRate);
                miner = MinerSystem.Instance.minersDict[minerName];
                timer = 0;
            }
            MinerDescriptionChange();
            ButtonTextChange();
        }
        
    }

    private void ButtonTextChange()
    {
        if (MinerSystem.Instance.minersDict[minerName].amount >= MinerSystem.Instance.minersDict[minerName].maxAmount)
        {
            buttonText.text = "Maxed";
            buttonImage.color = new Color32(115, 23, 36, 100);
            return;
        }

        if (payWithMoney)
        {
            buttonText.text = "Price: " + MinerSystem.Instance.minersDict[minerName].price + "$";
        }
        else
        {
            buttonText.text = "Price: " + MinerSystem.Instance.minersDict[minerName].price + " " + resourceName;
        }
    }

    private void ButtonChangeColor()
    {
        if (payWithMoney)
        {
            if (MoneySystem.Instance.Money < MinerSystem.Instance.minersDict[minerName].price)
            {
                buttonImage.color = new Color32(115, 23, 36, 100);
            }
            else
            {
                buttonImage.color = new Color32(32, 115, 23, 100);
            }
        }
        else
        {
            if (InventorySystem.Instance.items[resourceName].amount < MinerSystem.Instance.minersDict[minerName].price)
            {
                buttonImage.color = new Color32(115, 23, 36, 100);
            }
            else
            {
                buttonImage.color = new Color32(32, 115, 23, 100);
            }
        }
    }

    private void MinerDescriptionChange()
    {
        minerDescription.text = String.Format("{0}:\nAmount: {1}/{2} \nProduction Rate: {3}", minerName, MinerSystem.Instance.minersDict[minerName].amount, MinerSystem.Instance.minersDict[minerName].maxAmount, MinerSystem.Instance.minersDict[minerName].productionRate);
    }

    public void BuyMiner()
    {
        if (payWithMoney)
        {
            if (MoneySystem.Instance.Money >= MinerSystem.Instance.minersDict[minerName].price)
            {
                MoneySystem.Instance.RemoveMoney(MinerSystem.Instance.minersDict[minerName].price);
                MinerSystem.Instance.AddMiner(minerName);
                buttonText.text = "Price: " + MinerSystem.Instance.minersDict[minerName].price + "$";
                return;
            }
            return;
        }
        
        if (InventorySystem.Instance.items[resourceName].amount >= MinerSystem.Instance.minersDict[minerName].price)
        {
            InventorySystem.Instance.RemoveItem(resourceName, MinerSystem.Instance.minersDict[minerName].price);
            MinerSystem.Instance.AddMiner(minerName);
            buttonText.text = "Price: " + MinerSystem.Instance.minersDict[minerName].price + " " + resourceName;
            return;
        }
    } 
}

