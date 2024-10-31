using System;
using System.Collections;
using UnityEngine;

public class SlayerSystem : MonoBehaviour 
{
    private static SlayerSystem _instance;
    public static SlayerSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SlayerSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SlayerSystem");
                    _instance = go.AddComponent<SlayerSystem>();
                }
            }
            return _instance;
        }
    }

    private bool isAttacking = false;

    //TODO: Critical Chance
    // Player Attack Monster
    IEnumerator AttackMonsterCoroutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(DamageSystem.Instance.AttackSpeed / 1000);
        if (MonsterSystem.Instance.CurrentMonster != null)
            MonsterSystem.Instance.CurrentMonster.GetComponent<MonsterObject>().TakeDamage(DamageSystem.Instance.Damage, new Color(1f, 0f, 0f, 0.8f));
        isAttacking = false;
    }

    // Player Attack Boss
    IEnumerator AttackBossCoroutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(DamageSystem.Instance.AttackSpeed / 1000);
        if (BossSystem.Instance.CurrentBoss != null)
            BossSystem.Instance.CurrentBoss.GetComponent<BossObject>().TakeDamage(DamageSystem.Instance.Damage);
        isAttacking = false;
    }

    private void AttackMonster()
    {
        StartCoroutine(AttackMonsterCoroutine());
    }

    private void AttackBoss()
    {
        StartCoroutine(AttackBossCoroutine());
    }

    // Monster Attack Player
    IEnumerator MonsterAttackCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.5f);

            if (MonsterSystem.Instance.CurrentMonster != null)
            {
                HealthSystem.Instance.TakeDamage(MonsterSystem.Instance.CurrentMonster.GetComponent<MonsterObject>().Damage);
            }
            else if (BossSystem.Instance.CurrentBoss != null)
            {
                HealthSystem.Instance.TakeDamage(BossSystem.Instance.CurrentBoss.GetComponent<BossObject>().Damage);
            }
            else
            {
                // Debug.Log("No monster or boss found to attack.");
            }
        }
    }

    private void Update()
    {
        if (isAttacking)
        {
            return;
        }

        if (MonsterSystem.Instance.CurrentMonster != null)
        {
            AttackMonster();
        }
        else if (BossSystem.Instance.CurrentBoss != null)
        {
            AttackBoss();
        }
        else
        {
            // Debug.Log("No monster or boss found to attack.");
        }
    }

    private void Start()
    {
        StartCoroutine(MonsterAttackCoroutine());
    }
}