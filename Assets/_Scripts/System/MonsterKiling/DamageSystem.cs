using System;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    private static DamageSystem _instance;
    public static DamageSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DamageSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("DamageSystem");
                    _instance = go.AddComponent<DamageSystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private double damage;

    [SerializeField] private bool isWeaponEquipped;

    private Items weapon;

    public double Damage
    {
        get => DifficultySystem.Instance.GetDamage(damage);
        set => damage = value;
    }

    public bool IsWeaponEquipped
    {
        get => isWeaponEquipped;
        set => isWeaponEquipped = value;
    }

    public void EquipWeapon(Items weapon)
    {
        this.weapon = weapon;
        damage = weapon.damage;
        if (weapon.damageBoostPercentage > 0)
            damage += damage * weapon.damageBoostPercentage / 100;
        isWeaponEquipped = true;
        UISystem.Instance.UpdateLevelText();
    }

    public Items GetWeapon()
    {
        return weapon;
    }
}
