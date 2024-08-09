using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockObject : MonoBehaviour
{
    public List<Drop> drops;
    private double health;
    private double maxHealth;

    [SerializeField] private Text healthText;
    private MessageSpawner _messageSpawner;

    public double Health { get => health; set => health = value; }
    public double MaxHealth { get => maxHealth; set => maxHealth = value; }

    private Animator _animator;

    public void Damage(double damage)
    {
        _messageSpawner.SpawnMessage(String.Format("-{0}", UISystem.Instance.NumberFormat(damage)));
        _animator.Play("RockHit");
        health -= damage;
        if (health <= 0)
        {
            DestroyRock();
        }
        UpdateUI();
    }

    private void DestroyRock()
    {
        CaveSystem.Instance.DestroyRock(this);
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        healthText.text = UISystem.Instance.NumberFormat(health) + "/" + UISystem.Instance.NumberFormat(maxHealth);
    }

    private void OnMouseDown() 
    {
        Damage(MiningSystem.Instance.MiningEfficiency);
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _messageSpawner = GetComponent<MessageSpawner>();
        UpdateUI();
    }
}
