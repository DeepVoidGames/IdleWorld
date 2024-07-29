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

    public void AtackMonster(double damage)
    {
        if (currentMonster == null)
        {
            return;
        }

        MonsterObject monsterObject = currentMonster.GetComponent<MonsterObject>();
        monsterObject.AttackMonster(damage);
    }

    public void MonsterDied()
    {
        LevelSystem.Instance.NextStage();
        MonsterDrop();
    }

    private void MonsterDrop()
    {
        // Get the monster object
        MonsterObject monsterObject = currentMonster.GetComponent<MonsterObject>();

        // Gold drop
        GoldSystem.Instance.AddGold(DifficultySystem.Instance.GetMonsterDrop(monsterObject.MaxHealth));
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

    private IEnumerator SpawnMonsterCoroutine(int index)
    {
        Monster monster = BiomeSystem.Instance.Bioms.Find(x => x.Name == BiomeSystem.Instance.CurrentBiome).Monsters[index];
        if (monster.Prefab == null)
        {
            Debug.LogError("Monster prefab not found: " + monster.Name);
            yield break;
        }
        GameObject go = Instantiate(monster.Prefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(monsterSpawnParent.transform);
        MonsterObject monsterObject = go.GetComponent<MonsterObject>();
        currentMonster = go;
        monsterObject.SetMonster(monster);
        yield return new WaitForSeconds(1f);
        isSpawning = false;
    }

}
