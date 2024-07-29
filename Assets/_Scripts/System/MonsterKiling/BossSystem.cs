using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boss
{
    public string Name;
    public double BaseHealth;

    public double Health
    {
        get
        {
            return DifficultySystem.Instance.GetBossHealth(BaseHealth);
        }
    }

    public GameObject Prefab;
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

    [Header("Boss Spawning")]
    [SerializeField] private GameObject bossSpawnParent;
    private bool isSpawning = false;
    private GameObject currentBoss;
    private bool pauseBoss = false;

    public bool IsSpawning { get => isSpawning;}
    public bool PauseBoss { get => pauseBoss; set => pauseBoss = value;}

    public void AttackBoss(double damage)
    {
        if (currentBoss == null)
        {
            return;
        }

        BossObject bossObject = currentBoss.GetComponent<BossObject>();
        bossObject.AttackBoss(damage);
    }

    public void SpawnBoss()
    {
        if (!isSpawning)
        {
            Biomes biome = BiomeSystem.Instance.Bioms.Find(biome => biome.Name == BiomeSystem.Instance.CurrentBiome);
            isSpawning = true;
            int randomIndex = Random.Range(0, biome.Bosses.Count);
            Boss boss = biome.Bosses[randomIndex];
            if (boss.Prefab == null)
            {
                Debug.LogError("Boss prefab is null");
                return;
            }
            GameObject bossGO = Instantiate(boss.Prefab);
            bossGO.transform.SetParent(bossSpawnParent.transform);
            currentBoss = boss.Prefab;
            bossGO.GetComponent<BossObject>().SetBoss(boss);
        }
    }

    public void BossDied()
    {
        BossObject bossObject = currentBoss.GetComponent<BossObject>();
        LevelSystem.Instance.ResetStage();
        GoldSystem.Instance.AddGold(DifficultySystem.Instance.GetBossDrop(bossObject.MaxHealth));
        currentBoss = null;
        isSpawning = false;
    }

    public void FailedToKill()
    {
        isSpawning = false;
    }
}