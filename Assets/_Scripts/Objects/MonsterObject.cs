using UnityEngine;
using UnityEngine.UI;

public class MonsterObject : MonoBehaviour
{
    private string monsterName;
    [SerializeField] private double health;
    private double maxHealth;

    [Header("UI Elements")]
    private Slider healthSlider;
    private Text healthText;

    public string MonsterName { get => monsterName; set => monsterName = value;}
    public double Health { get => health; set => health = value;}
    public double MaxHealth { get => maxHealth; set => maxHealth = value;}

    private MessageSpawner _messageSpawner;

    [Header("Hit Animation")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private Material defaultMaterial;

    public void SetMonster(Monster monster)
    {
        monsterName = monster.Name;
        health = monster.Health;
        maxHealth = monster.Health;
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
        // Hit Animation
        if (hitMaterial != null)
            gameObject.GetComponent<Renderer>().material = hitMaterial;
        _messageSpawner.SpawnMessage("-" + UISystem.Instance.NumberFormat(damage));
        if (health <= 0)
        {
            MonsterSystem.Instance.MonsterDied();
            Destroy(gameObject);
        }
        if (hitMaterial != null)
            Invoke("ResetMaterial", .05f);
        UpdateHealthUI();
    }

    private void ResetMaterial()
    {
        gameObject.GetComponent<Renderer>().material = defaultMaterial;
    }

    private void OnMouseDown() 
    {
        TakeDamage(DamageSystem.Instance.Damage);
    }

    private void Awake()
    {
        // Find in children canvas
        healthSlider = transform.Find("Canvas/HealthBar").GetComponent<Slider>();
        healthText = transform.Find("Canvas/HealthText").GetComponent<Text>();
    }

    private void Start()
    {
        _messageSpawner = GetComponent<MessageSpawner>();
        UpdateHealthUI();
    }

}