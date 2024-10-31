using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boss
{
    public string Name;
    public double BaseHealth;
    public double BaseDamage;

    public double Health
    {
        get
        {
            return DifficultySystem.Instance.GetBossHealth(BaseHealth);
        }
    }

    public double Damage
    {
        get
        {
            return DifficultySystem.Instance.GetMobsDamage(BaseDamage);
        }
    }

    public GameObject Prefab;

    public List<Drop> Drops; 
}

public class BossSystem : MonoBehaviour 
{
    private static BossSystem _instance;
    public static BossSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BossSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("BossSystem");
                    _instance = go.AddComponent<BossSystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private float maxTimeToKillBoss = 30f;

    public float MaxTimeToKillBoss { get => maxTimeToKillBoss;}

    [SerializeField] private GameObject bossSpawnParent;

    [Header("Boss Spawning")]
    private bool isSpawning = false;
    private GameObject currentBoss;
    private BossObject bossObject;
    private bool pauseBoss = false; //! Not used 

    public bool IsSpawning { get => isSpawning;}
    public bool PauseBoss { get => pauseBoss; set => pauseBoss = value;}
    public GameObject CurrentBoss { get => currentBoss;}
    public BossObject BossObject { get => bossObject;}

    private ParticleSystem killEffect;

    public void SpawnBoss()
    {
        if (!isSpawning)
        {
            if (currentBoss != null)
            {
                Destroy(currentBoss);
            }
            if (MonsterSystem.Instance.CurrentMonster != null)
            {
                Destroy(MonsterSystem.Instance.CurrentMonster);
            }
            isSpawning = true;
            StartCoroutine(SpawnBossCoroutine());
        }
    }

    public Drop DropCalculator()
    {
        Biomes biome = BiomeSystem.Instance.Bioms.Find(biome => biome.Name == BiomeSystem.Instance.CurrentBiome);
        Boss boss = biome.Bosses.Find(boss => boss.Name == bossObject.BossName);
        if (boss == null)
        {
            Debug.LogError("Boss not found");
            return null;
        }
        if (boss.Drops.Count == 0)
        {
            // Debug.LogError("Drops not found");
            return null;
        }
        if (boss.Drops.Count == 1)
        {
            return boss.Drops[0];
        }
        List<Drop> drops = boss.Drops;
        float random = Random.Range(0f, 100f);
        float sum = 0f;
        foreach (Drop drop in drops)
        {
            sum += drop.chance;
            if (random <= sum)
            {
                return drop;
            }
        }
        return null;
    }

    public void BossDied()
    {
        // Calculate the reward
        DestroyBoss();
        currentBoss = null;
        isSpawning = false;

        Drop drop = DropCalculator();
        if (drop != null)
            InventorySystem.Instance.AddItemByName(drop.resourceName, 1f);
        LevelSystem.Instance.ResetStage();
        double m = DifficultySystem.Instance.GetBossDrop(bossObject.MaxHealth);
        GoldSystem.Instance.AddGold(m);
        UISystem.Instance.MoneyIndicator(m);
        if (killEffect != null && SwitchMode.Instance.CurrentMode == 0)
            killEffect.Play();
        BonusSystem.Instance.ShowBonus();
    }

    public void DestroyBoss()
    {
        if (currentBoss != null)
        {
            Destroy(currentBoss); // Destroy the instantiated GameObject
            currentBoss = null;
        }
    }

    public void FailedToKill()
    {
        DestroyBoss(); // Destroy the instantiated GameObject
        isSpawning = false;
        currentBoss = null;
    }

    private void Awake()
    {
        killEffect = GameObject.Find("KillEffect").GetComponent<ParticleSystem>();
    }

    private IEnumerator SpawnBossCoroutine()
    {
        Biomes biome = BiomeSystem.Instance.Bioms.Find(biome => biome.Name == BiomeSystem.Instance.CurrentBiome);
        int randomIndex = Random.Range(0, biome.Bosses.Count);
        Boss boss = biome.Bosses[randomIndex];
        if (boss.Prefab == null)
        {
            Debug.LogError("Boss prefab is null");
            yield break;
        }
        GameObject bossGO = Instantiate(boss.Prefab, bossSpawnParent.transform.position, Quaternion.identity);
        bossGO.transform.SetParent(bossSpawnParent.transform);
        currentBoss = bossGO; // Assign the instantiated GameObject to currentBoss
        bossGO.GetComponent<BossObject>().SetBoss(boss);
        bossObject = bossGO.GetComponent<BossObject>();
        yield return new WaitForSeconds(maxTimeToKillBoss);
        isSpawning = false;
    }
}