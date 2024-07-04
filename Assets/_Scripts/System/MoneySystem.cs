using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
