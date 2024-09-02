using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour 
{
    private static HealthSystem _instance;
    public static HealthSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HealthSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("HealthSystem");
                    _instance = go.AddComponent<HealthSystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private double health = 100;
    [SerializeField] private double maxHealth = 100;

    public bool IsDead => health <= 0;
    public double Health { get => health; set => health = value; }

    [Header("UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;

    [SerializeField] private GameObject gameOverScreen;

    private void UpdateUI()
    {
        // 100% = X: 0, Y: 0 | W: 490, H: 30
        // 0% = X: -245, Y: 0 | W: 0, H: 30
        healthBar.rectTransform.sizeDelta = new Vector2((float)health * 4.9f, 30);
        healthText.text = $"Health: {UISystem.Instance.NumberFormat(health)} / {UISystem.Instance.NumberFormat(maxHealth)}";
    }

    public void AddHealthBoost(double value)
    {
        health += value;
        maxHealth += value;
        UpdateUI();
    }

    public void RemoveHealthBoost(double value)
    {
        health -= value;
        maxHealth -= value;
        UpdateUI();
    }

    public void TakeDamage(double damage)
    {
        health -= damage;
        UpdateUI();
        if (health <= 0)
            Death();
    }

    IEnumerator SlayerDied()
    {
        LevelSystem.Instance.RestetSlayer();
        yield return new WaitForSeconds(5f);
        gameOverScreen.SetActive(false);
        health = 100;
        maxHealth = 100;
    }

    private void Death()
    {
        if (IsDead)
        {
            if(MonsterSystem.Instance.CurrentMonster != null)
                MonsterSystem.Instance.DestroyMonster();
            
            if (BossSystem.Instance.CurrentBoss != null)
                BossSystem.Instance.DestroyBoss();

            //TODO Show Game Over Screen
            gameOverScreen.SetActive(true);
            BonusSystem.Instance.RestartBonuses();
            StartCoroutine(SlayerDied());
        }
    }
}