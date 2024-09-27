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
    [SerializeField] private double manaPerHour = 10f;

    [Header("UI")]
    [SerializeField] private Text textMana;

    private bool isLoaded = false;

    public event Action<double> OnManaChanged;

    private void UIUpdate()
    {
        textMana.text = $"Mana: {UISystem.Instance.NumberFormat(mana)} + {UISystem.Instance.NumberFormat(manaPerHour)}/h";
    }

    public void AddMana(double value)
    {
        mana += value;
        OnManaChanged?.Invoke(mana);
        UIUpdate();
    }

    public void RemoveMana(double value)
    {
        mana -= value;
        OnManaChanged?.Invoke(mana);
        UIUpdate();
    }

    public void SetManaPerHour(double value)
    {
        manaPerHour = value;
        UIUpdate();
        StartCoroutine(ManaRegen());
        isLoaded = true;
    }

    public void AddManaPerHour(double value)
    {
        manaPerHour += value;
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

    IEnumerator ManaRewards(float idleTime)
    {
        while (!isLoaded)
        {
            yield return new WaitForSeconds(1);
        }
        AddMana(idleTime / 3600 * manaPerHour);
    }

    public void IdleReward(float idleTime)
    {
        StartCoroutine(ManaRewards(idleTime));
    }
}