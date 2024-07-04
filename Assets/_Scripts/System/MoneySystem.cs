using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneySystem : MonoBehaviour
{
    private static MoneySystem _instance;
    public static MoneySystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MoneySystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MoneySystem");
                    _instance = go.AddComponent<MoneySystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField]
    private float startingMoney = 0f; // Początkowa ilość pieniędzy
    public float Money { get; private set; } // Aktualna ilość pieniędzy

    [SerializeField]
    private Text moneyText;

    private void Awake()
    {
        Money = startingMoney;
    }

    public void AddMoney(float amount)
    {
        Money += amount;
    }

    public void RemoveMoney(float amount)
    {
        Money -= amount;
    }

    public void SetMoney    (float amount)
    {
        Money = amount;
    }

    private void Update()
    {
        moneyText.text = Money + "$";
    }
}
