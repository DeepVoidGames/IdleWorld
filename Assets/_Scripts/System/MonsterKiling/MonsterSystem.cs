using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    public string Name;
    public double BaseHealth;

    public double Health
    {
        get
        {
            return DifficultySystem.Instance.GetMonsterHealth(BaseHealth);
        }
    }

    public GameObject Prefab;
}

public class MonsterSystem : MonoBehaviour
{
    private static MonsterSystem _instance;
    public static MonsterSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MonsterSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MonsterSystem");
                    _instance = go.AddComponent<MonsterSystem>();
                }
            }
            return _instance;
        }
    }

    [Header("Monster Spawning")]
    [SerializeField] private GameObject monsterSpawnParent;
    private bool isSpawning = false;
    private GameObject currentMonster;

    public GameObject CurrentMonster { get => currentMonster; }

    private ParticleSystem killEffect;

    public void MonsterDied()
    {
        LevelSystem.Instance.NextStage();
        if (killEffect != null && SwitchMode.Instance.CurrentMode == 0)
            killEffect.Play();
        MonsterDrop();
    }

    private void MonsterDrop()
    {
        // Get the monster object
        MonsterObject monsterObject = currentMonster.GetComponent<MonsterObject>();

        // Gold drop
        double m = DifficultySystem.Instance.GetMonsterDrop(monsterObject.MaxHealth);
        GoldSystem.Instance.AddGold(m);
        UISystem.Instance.MoneyIndicator(m);
    }

    public void SpawnMonster()
    {
        if (isSpawning)
        {
            return;
        }

        if (BossSystem.Instance.IsSpawning)
        {
            return;
        }

        if (BossSystem.Instance.CurrentBoss != null)
        {
            return;
        }

        Biomes biome = BiomeSystem.Instance.Bioms.Find(x => x.Name == BiomeSystem.Instance.CurrentBiome);
        if (biome == null)
        {
            Debug.LogError("Biome not found");
            return;
        }

        if (biome.Monsters.Count == 0)
        {
            Debug.LogError("No monsters in biome");
            return;
        }
        // Get a random monster from the biome
        int index = UnityEngine.Random.Range(0, biome.Monsters.Count);

        isSpawning = true;
        StartCoroutine(SpawnMonsterCoroutine(index));
    }

    private void Update()
    {
        if(currentMonster != null)
        {
            MonsterObject monsterObject = currentMonster.GetComponent<MonsterObject>();
            if (monsterObject.Health <= 0)
            {
                currentMonster = null;
            }
        }
        else
        {
            SpawnMonster();
        }
    }

    private void Awake()
    {
        killEffect = GameObject.Find("KillEffect").GetComponent<ParticleSystem>();
    }

    private IEnumerator SpawnMonsterCoroutine(int index)
    {
        Monster monster = BiomeSystem.Instance.Bioms.Find(x => x.Name == BiomeSystem.Instance.CurrentBiome).Monsters[index];
        if (monster.Prefab == null)
        {
            Debug.LogError("Monster prefab not found: " + monster.Name);
            yield break;
        }
        GameObject go = Instantiate(monster.Prefab, monsterSpawnParent.transform.position, Quaternion.identity);
        go.transform.SetParent(monsterSpawnParent.transform);
        MonsterObject monsterObject = go.GetComponent<MonsterObject>();
        currentMonster = go;
        monsterObject.SetMonster(monster);
        yield return new WaitForSeconds(0.5f);
        isSpawning = false;
    }

}
