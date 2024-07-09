using UnityEngine;
using UnityEngine.UI;

public class BossObject : MonoBehaviour 
{
    [SerializeField] private string bossName;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float _timer = 0f;

    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;

    public string BossName { get => bossName; set => bossName = value;}
    public float Health { get => health; set => health = value;}
    public float MaxHealth { get => maxHealth; set => maxHealth = value;}

    public void SetBoss(Boss boss)
    {
        bossName = boss.Name;
        health = boss.Health;
        maxHealth = boss.Health;
        UpdateHealthUI();
    }

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
            BossSystem.Instance.BossDied();
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

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= BossSystem.Instance.MaxTimeToKillBoss)
        {
            BossSystem.Instance.FailedToKill();
            Destroy(gameObject);
        }
    }

}