using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rune
{
    public string name;
    public double value;
    public string description;
    public double amount;

    public Rarity rarity;
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythical,
    }

    public Type type;
    public enum Type
    {
        Fire,
        Water,
        Earth,
        Air,
        Light,
        Dark,
    }

    public BonusType bonusType;
    public enum BonusType
    {
        Damage,
        Gold,
    }

}

public class RuneData
{
    public Rune rune;
    public int amount;
}

public class RunesSystem : MonoBehaviour
{
    private static RunesSystem _instance;
    public static RunesSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RunesSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("RunesSystem");
                    _instance = go.AddComponent<RunesSystem>();
                }
            }
            return _instance;
        }
    }

    [Header("Runes")]
    public List<Rune> runes = new List<Rune>();
    
}
