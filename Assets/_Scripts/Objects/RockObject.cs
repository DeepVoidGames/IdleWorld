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
    public void Damage(double damage)
    {
        _messageSpawner.SpawnMessage(String.Format("-{0}", UISystem.Instance.NumberFormat(damage)));
        health -= damage;
        if (health <= 0)
        {
            DestroyRock();
        }
        UpdateUI();
    }

    private void DestroyRock()
    {
        MiningSystem.Instance.DestroyRock(this);
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
        _messageSpawner = GetComponent<MessageSpawner>();
        UpdateUI();
    }
}
