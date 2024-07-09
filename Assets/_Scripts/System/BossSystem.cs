using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boss
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

    [SerializeField] private List<Boss> Bosses = new List<Boss>();

    [Header("Boss Spawning")]
    private bool isSpawning = false;

    public bool IsSpawning { get => isSpawning;}

    public void SpawnBoss()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            int randomIndex = Random.Range(0, Bosses.Count);
            Boss boss = Bosses[randomIndex];
            GameObject bossGO = Instantiate(boss.Prefab);
            bossGO.GetComponent<BossObject>().SetBoss(boss);
        }
    }

    public void BossDied()
    {
        LevelSystem.Instance.ResetStage();
        isSpawning = false;
    }
    
}