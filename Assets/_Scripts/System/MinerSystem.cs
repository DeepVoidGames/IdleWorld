using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Miner
{
    public string minerName;
    public float amount;
    public float maxAmount = 100f;
    public float _productionRate;
    public float _productionTime;
    public float _price;

    public float productionRate
    {
        get 
        {
            if (amount == 1)
            {
                return _productionRate;
            } 
            return _productionRate + productionRateIncreaseFactor * amount; 
        }
        set { _productionRate = value; }
    }

    public float productionTime
    {
        get 
        {
            if (amount == 1)
            {
                return _productionTime;
            } 
            return Mathf.Max(minimumProductionTime, _productionTime - (productionRateIncreaseFactor * amount)); 
        }
        set { _productionTime = value; }
    }
    
    public float price
    {
        get 
        {
            if (amount == 1)
            {
                return basePrice;
            } 
            return basePrice * Mathf.Pow(priceIncreaseFactor, amount); 
        }
        set { _price = value; }
    }
    
    public float basePrice = 10f;

    [Header("Increase factors")]
    public float priceIncreaseFactor = .5f;
    public float productionRateIncreaseFactor = 0.01f;
    public float minimumProductionTime = 0.1f;
}

public class MinerSystem : MonoBehaviour
{
    private static MinerSystem _instance;
    public static MinerSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MinerSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MinerSystem");
                    _instance = go.AddComponent<MinerSystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField]
    private List<Miner> miners = new List<Miner>();

    public Dictionary<string, Miner> minersDict = new Dictionary<string, Miner>();

    private void Awake()
    {
        foreach (Miner miner in miners)
        {
            minersDict[miner.minerName] = miner;
        }
    }

    public void AddMiner(string minerName, float amount = 1f)
    {
        if (minersDict.ContainsKey(minerName))
        {
            minersDict[minerName].amount += amount;
            SaveSystem.Instance.SaveMiners();
        }
        else
        {
            Debug.Log("Miner " + minerName + " not found in miners.");
        }
    }

    public Miner GetMiner(string minerName)
    {
        if (minersDict.ContainsKey(minerName))
        {
            return minersDict[minerName];
        }
        else
        {
            Debug.Log("Miner " + minerName + " not found in miners.");
            return null;
        }
    }
}
