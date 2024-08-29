using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManaSystem : MonoBehaviour 
{
    private static ManaSystem _instance;
    public static ManaSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ManaSystem>();
            }
            return _instance;
        }
    }

    [Header("Mana")]
    [SerializeField] private double mana;
    [SerializeField] private double manaPerHour = 10;

    [Header("UI")]
    [SerializeField] private Text textMana;

    private void UIUpdate()
    {
        textMana.text = $"Mana: {UISystem.Instance.NumberFormat(mana)}";
    }

    public void AddMana(double value)
    {
        mana += value;
        UIUpdate();
    }

    public void RemoveMana(double value)
    {
        mana -= value;
        UIUpdate();
    }

    public void SetManaPerHour(double value)
    {
        manaPerHour = value;
    }

    public double GetMana()
    {
        return mana;
    }

    public double GetManaPerHour()
    {
        return manaPerHour;
    }

    IEnumerator ManaRegen()
    {
        while (true)
        {
            yield return new WaitForSeconds(3600);
            AddMana(manaPerHour);
        }
    }

    public void IdleReward(float idleTime)
    {
        AddMana(idleTime / 3600 * manaPerHour);
    }

    private void Start()
    {
        StartCoroutine(ManaRegen());
    }
}