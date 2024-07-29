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

    private float _timerDPS;

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
        //TODO: Add damage boost percentage
        // if (weapon.damageBoostPercentage > 0)
        //     DifficultySystem.Instance.AddDamagePercentage(weapon.damageBoostPercentage / 100);
        isWeaponEquipped = true;
        UISystem.Instance.UpdateLevelText();
    }

    public Items GetWeapon()
    {
        return weapon;
    }

    private void Attack()
    {
        double dps = DifficultySystem.Instance.GetDPS();
        if (dps > 0)
        {
            if (MonsterSystem.Instance.CurrentMonster != null)
                MonsterSystem.Instance.CurrentMonster.GetComponent<MonsterObject>().TakeDamage(dps);
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
