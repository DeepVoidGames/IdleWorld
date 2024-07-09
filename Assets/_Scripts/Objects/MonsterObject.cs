using UnityEngine;
using UnityEngine.UI;

public class MonsterObject : MonoBehaviour
{
    [SerializeField] private string monsterName;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;

    public string MonsterName { get => monsterName; set => monsterName = value;}
    public float Health { get => health; set => health = value;}
    public float MaxHealth { get => maxHealth; set => maxHealth = value;}

    public void UpdateHealthUI()
    {
        healthSlider.value = health;
        healthSlider.maxValue = maxHealth;
        healthText.text = health + " / " + healthSlider.maxValue;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            MonsterSystem.Instance.MonsterDied();
            Destroy(gameObject);
        }
        UpdateHealthUI();
    }

    private void OnMouseDown() 
    {
        TakeDamage(DamageSystem.Instance.Damage);
    }

    private void Start()
    {
        UpdateHealthUI();
    }

}