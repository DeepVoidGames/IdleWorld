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

    [Header("Damage System")]
    [SerializeField] private double damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float criticalChance;
    [SerializeField] private double criticalDamage;

    public double Damage
    {
        get => DifficultySystem.Instance.GetDamage(damage);
    }

    public void AddDamageBoost(double value)
    {
        damage += value;
        UISystem.Instance.UpdateLevelText();
    }

    public void RemoveDamageBoost(double value)
    {
        damage -= value;
    }

    [Header("Weapon System")]
    [SerializeField] private bool isWeaponEquipped;

    public bool IsWeaponEquipped
    {
        get => isWeaponEquipped;
        set => isWeaponEquipped = value;
    }
    private Items weapon;

    public void EquipWeapon(Items weapon)
    {
        this.weapon = weapon;
        UpdateWeaponDamage();
        //TODO: Add damage boost percentage
        // if (weapon.damageBoostPercentage > 0)
        //     DifficultySystem.Instance.AddDamagePercentage(weapon.damageBoostPercentage / 100);
        isWeaponEquipped = true;
        UISystem.Instance.UpdateLevelText();
    }

    public void UpdateWeaponDamage()
    {
        if (weapon != null)
        {
            damage = weapon.Damage;
        }
    }

    public Items GetWeapon()
    {
        return weapon;
    }

    // Attack
    private void Attack()
    {
        double dps = DifficultySystem.Instance.GetDPS();
        if (dps > 0)
        {
            if (MonsterSystem.Instance.CurrentMonster != null)
                MonsterSystem.Instance.CurrentMonster.GetComponent<MonsterObject>().TakeDamage(dps, new Color(1, 0.2987421f, 0.9054203f, 1));
           else if (BossSystem.Instance.CurrentBoss != null)
                BossSystem.Instance.BossObject.TakeDamage(dps);
        }
    }

    private void Start() 
    {
        InvokeRepeating("Attack", 2f, 1f);
    }

    private void OnApplicationQuit() 
    {
        CancelInvoke();    
    }
}
