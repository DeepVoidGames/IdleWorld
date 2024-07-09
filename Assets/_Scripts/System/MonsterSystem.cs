using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Monster
{
    public string Name;
    public float BaseHealth;

    public float Health
    {
        get
        {
            return BaseHealth * LevelSystem.Instance.Level;
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
    private bool isSpawning = false;
    private GameObject currentMonster;

    [Header("UI")]
    [SerializeField] private Text levelText;

    public void MonsterDied()
    {
        LevelSystem.Instance.NextStage();
        MonsterDrop();
        UpdateUI();

        SaveSystem.Instance.Save();
    }

    private void MonsterDrop()
    {
        // Get the monster object
        MonsterObject monsterObject = currentMonster.GetComponent<MonsterObject>();

        // Gold drop
        GoldSystem.Instance.AddGold((monsterObject.MaxHealth * 0.1f) * LevelSystem.Instance.Level);
    }

    public void SpawnMonster(int index)
    {
        if (isSpawning)
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

    private void UpdateUI()
    {
        levelText.text = String.Format("Level: {0}  Stage: {1}", LevelSystem.Instance.Level, LevelSystem.Instance.Stage);
    } 

    private void Start()
    {
        Debug.Log("Monster System Started");
        Debug.Log("Monster Count: " + Monsters.Count);
        UpdateUI();
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
        MonsterObject monsterObject = go.GetComponent<MonsterObject>();
        currentMonster = go;
        monsterObject.Health = monster.Health;
        monsterObject.MaxHealth = monster.Health;
        monsterObject.UpdateHealthUI();
        yield return new WaitForSeconds(1f);
        isSpawning = false;
    }

}
