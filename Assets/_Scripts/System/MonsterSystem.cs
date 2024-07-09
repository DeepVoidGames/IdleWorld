using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    public string Name;
    public float BaseHealth;

    public float Health
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

    [SerializeField] private List<Monster> Monsters = new List<Monster>();

    [Header("Monster Spawning")]
    [SerializeField] private GameObject monsterSpawnParent;
    private bool isSpawning = false;
    private GameObject currentMonster;

    public void MonsterDied()
    {
        LevelSystem.Instance.NextStage();
        MonsterDrop();
        SaveSystem.Instance.Save();
    }

    private void MonsterDrop()
    {
        // Get the monster object
        MonsterObject monsterObject = currentMonster.GetComponent<MonsterObject>();

        // Gold drop
        GoldSystem.Instance.AddGold(DifficultySystem.Instance.GetMonsterDrop(monsterObject.MaxHealth));
    }

    public void SpawnMonster(int index)
    {
        if (isSpawning)
        {
            return;
        }

        if (BossSystem.Instance.IsSpawning)
        {
            return;
        }

        if (index < 0 || index >= Monsters.Count)
        {
            return;
        }

        isSpawning = true;
        StartCoroutine(SpawnMonsterCoroutine(index));
    }

    private void Start()
    {
        Debug.Log("Monster System Started");
        Debug.Log("Monster Count: " + Monsters.Count);
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
            int index = UnityEngine.Random.Range(0, Monsters.Count);
            SpawnMonster(index);
        }
    }

    private IEnumerator SpawnMonsterCoroutine(int index)
    {
        Monster monster = Monsters[index];
        GameObject go = Instantiate(monster.Prefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(monsterSpawnParent.transform);
        MonsterObject monsterObject = go.GetComponent<MonsterObject>();
        currentMonster = go;
        monsterObject.SetMonster(monster);
        yield return new WaitForSeconds(1f);
        isSpawning = false;
    }

}
