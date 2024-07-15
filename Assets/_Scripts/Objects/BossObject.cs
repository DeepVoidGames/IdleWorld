using UnityEngine;
using UnityEngine.UI;

public class BossObject : MonoBehaviour 
{
    [SerializeField] private string bossName;
    [SerializeField] private double health;
    [SerializeField] private double maxHealth;
    [SerializeField] private float _timer = 0f;

    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;
    [SerializeField] private Text timerText;
    private MessageSpawner _messageSpawner;

    public string BossName { get => bossName; set => bossName = value;}
    public double Health { get => health; set => health = value;}
    public double MaxHealth { get => maxHealth; set => maxHealth = value;}

    public void SetBoss(Boss boss)
    {
        bossName = boss.Name;
        health = boss.Health;
        maxHealth = boss.Health;
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        healthSlider.value = (float)health;
        healthSlider.maxValue = (float)maxHealth;
        healthText.text = UISystem.Instance.NumberFormat(health) + " / " + UISystem.Instance.NumberFormat(maxHealth);
    }

    public void TakeDamage(double damage)
    {
        health -= damage;
        _messageSpawner.SpawnMessage("-" + UISystem.Instance.NumberFormat(damage));
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
        _messageSpawner = GetComponent<MessageSpawner>();
        UpdateHealthUI();
        _timer = BossSystem.Instance.MaxTimeToKillBoss;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        timerText.text = "Time: " + _timer.ToString("F2") + "s";
        if (_timer <= 0)
        {
            BossSystem.Instance.FailedToKill();
            Destroy(gameObject);
        }
    }

}