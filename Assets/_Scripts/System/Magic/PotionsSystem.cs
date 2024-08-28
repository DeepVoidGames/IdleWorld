using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion
{
    public string name;
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
    public string description;
    public float bonus;
    public float duration;

    // Potion Type
    public PotionType type;
    public enum PotionType
    {
        Damage,
        Gold
    }
}

public class PotionsSystem : MonoBehaviour
{

}
