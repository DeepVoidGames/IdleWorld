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
        if (payWithMoney)
        {
            buttonText.text = "Price: " + MinerSystem.Instance.minersDict[minerName].price + "$";
        }
        else
        {
            buttonText.text = "Price: " + MinerSystem.Instance.minersDict[minerName].price + " " + resourceName;
        }
        minerDescription.text = String.Format("{0}:\nAmount: {1}/{2} \nProduction Rate: {3}", minerName, MinerSystem.Instance.minersDict[minerName].amount, MinerSystem.Instance.minersDict[minerName].maxAmount, MinerSystem.Instance.minersDict[minerName].productionRate);

    }

    void Update()
    {
        if (miner.amount > 0)
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
            }else
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
            timer += Time.deltaTime;
            if (timer >= miner.productionTime)
            {
                rockSystem.DamageCurrentRock(miner.productionRate);
                miner = MinerSystem.Instance.minersDict[minerName];
                minerDescription.text = String.Format("{0}:\nAmount: {1}/{2} \nProduction Rate: {3} / {4}s", minerName, MinerSystem.Instance.minersDict[minerName].amount, MinerSystem.Instance.minersDict[minerName].maxAmount, MinerSystem.Instance.minersDict[minerName].productionRate, MinerSystem.Instance.minersDict[minerName].productionTime);
                timer = 0;
            }
        }
        
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

