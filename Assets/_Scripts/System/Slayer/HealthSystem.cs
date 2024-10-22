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

    private double baseMaxHealth = 100;

    public bool IsDead => health <= 0;
    public double Health { get => health; set => health = value; }

    public bool reviveSlayer = false;


    [Header("UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;

    [SerializeField] private GameObject gameOverScreen;

    

    private void UpdateUI()
    {
        const float MaxBarWidth = 490f;

        // Clamp health between 0 and maxHealth
        health = Mathf.Clamp((float)health, 0f, (float)maxHealth);
        
        // Calculate the width of the health bar based on current health
        float clampedWidth = Mathf.Clamp((float)health / (float)maxHealth * MaxBarWidth, 0f, MaxBarWidth);
        
        // Update health bar size
        healthBar.rectTransform.sizeDelta = new Vector2(clampedWidth, 30);
        
        // Update health text
        healthText.text = $"Health: {UISystem.Instance.NumberFormat(health)} / {UISystem.Instance.NumberFormat(maxHealth)}";
    }


    public void AddHealthBoost(double value)
    {
        health += value;
        maxHealth += value;
        baseMaxHealth += value;
        UpdateUI();
    }

    public void RemoveHealthBoost(double value)
    {
        health -= value;
        maxHealth -= value;
        baseMaxHealth -= value;
        UpdateUI();
    }

    public void TakeDamage(double damage)
    {
        health -= damage;
        UpdateUI();
        if (health <= 0)
            Death();
    }

    public void HealOverTime(double healAmount, float duration)
    {
        StartCoroutine(HealOverTimeCoroutine(healAmount, duration));
    }

    private IEnumerator HealOverTimeCoroutine(double healAmount, float duration)
    {
        float time = 0;
        float interval = 1f; // Heal every 1 second
        double healPerInterval = healAmount / (duration / interval);

        while (time < duration)
        {
            yield return new WaitForSeconds(interval);
            health += healPerInterval;
            UpdateUI();
            time += interval;
        }
    }

    IEnumerator SlayerDied()
    {

        LevelSystem.Instance.RestetSlayer();
        yield return new WaitForSeconds(1f);
        gameOverScreen.SetActive(false);
        maxHealth = baseMaxHealth;
        health = maxHealth;
        UpdateUI();
    }

    private void SaveHighscore()
    {
        if (LevelSystem.Instance.Level > PlayerPrefs.GetInt("HighscoreLevel", 0))
                PlayerPrefs.SetInt("HighscoreLevel", LevelSystem.Instance.Level);
        if (LevelSystem.Instance.Stage > PlayerPrefs.GetInt("HighscoreStage", 0))
            PlayerPrefs.SetInt("HighscoreStage", LevelSystem.Instance.Stage);
        gameOverScreen.transform.Find("LevelText").GetComponent<Text>().text = $"Highest Level: {PlayerPrefs.GetInt("HighscoreLevel")}";
        gameOverScreen.transform.Find("StageText").GetComponent<Text>().text = $"Highest Stage: {PlayerPrefs.GetInt("HighscoreStage")}";
    }

    IEnumerator Revive()
    {
        gameOverScreen.SetActive(true);
        
        float _timer = 0f;
        while (true)
        {
            _timer += Time.deltaTime;
            if (reviveSlayer || _timer >= 15f)
            {
                break;
            }
            yield return null;
        }

        if(reviveSlayer)
        {
            health = maxHealth;
            reviveSlayer = false;
            gameOverScreen.SetActive(false);
        }
        else
        {
            SaveHighscore();
            BonusSystem.Instance.RestartBonuses();
            StartCoroutine(SlayerDied());
        }
        UpdateUI();
    }

    private void Death()
    {
        if (IsDead)
        {
            if(MonsterSystem.Instance.CurrentMonster != null)
                MonsterSystem.Instance.DestroyMonster();
            
            if (BossSystem.Instance.CurrentBoss != null)
                BossSystem.Instance.DestroyBoss();

            StartCoroutine(Revive());
        }
    }
}