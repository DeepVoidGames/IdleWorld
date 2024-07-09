using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockObject : MonoBehaviour
{
    internal IEnumerable<Drop> drops;
    private float health;
    private float maxHealth;

    [SerializeField] private Text healthText;

    public float Health { get => health; set => health = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DestroyRock();
        }
        UpdateUI();
    }

    private void DestroyRock()
    {
        MiningSystem.Instance.DestroyRock();
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        healthText.text = health + "/" + maxHealth;
    }

    private void OnMouseDown() 
    {
        Damage(MiningSystem.Instance.miningEfficiency);
    }

    void Start()
    {
        UpdateUI();
    }
}
