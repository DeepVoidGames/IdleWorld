using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdleSystem : MonoBehaviour
{
    private static IdleSystem instance;
    public static IdleSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<IdleSystem>();
            }
            return instance;
        }
    }

    [Header("Idle System")]
    [SerializeField] private float idleTime = 0;
    [SerializeField] private float minIdleTime = 300;

    [SerializeField] private float idleChestTime = 0;

    public float IdleTime { get => idleTime;}

    public float IdleChestTime { get => idleChestTime;}
    public float MinIdleTime { get => minIdleTime;}

    [Header("UI")]
    [SerializeField] private Text textIdleTime;
    [SerializeField] private GameObject chestObject;

    public void RestartIdleChestTime()
    {
        idleChestTime = 0;
        UIUpdate();
    }

    private void UIUpdate()
    {
        // textIdleTime.text = "Idle Time: " + UISystem.Instance.NumberFormat(idleChestTime / 60 / 60) + " hours";
        if (idleChestTime >= minIdleTime)
        {
            Color tempColor = chestObject.GetComponent<SpriteRenderer>().color;
            tempColor.a = 1f;
            chestObject.GetComponent<SpriteRenderer>().color = tempColor;
        }
    }

    private IEnumerator Idle()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (DifficultySystem.Instance.GetDPS() == 0)
                continue;
            idleChestTime += 1;
            UIUpdate();
        }
    }

    IEnumerator IdleRewards()
    {
        yield return new WaitForSeconds(5);
        ManaSystem.Instance.IdleReward(idleTime);
        idleTime = 0;
    }

    private void GetIdleReward()
    {
        StartCoroutine(IdleRewards());
    }

    private void Start()
    {
        idleTime = PlayerPrefs.GetFloat("IdleTime", 0);
        idleChestTime = PlayerPrefs.GetFloat("IdleChestTime", 0);
        string lastExitTime = PlayerPrefs.GetString("LastExitTime", System.DateTime.Now.ToString());
        System.DateTime lastExit = System.DateTime.Parse(lastExitTime);
        System.TimeSpan timeSpan = System.DateTime.Now - lastExit;
        idleTime += (float)timeSpan.TotalSeconds;
        idleChestTime += (float)timeSpan.TotalSeconds;

        UIUpdate();
        StartCoroutine(Idle());
        GetIdleReward();
    }

    private void OnApplicationQuit() 
    {
        PlayerPrefs.SetFloat("IdleTime", idleTime);
        PlayerPrefs.SetFloat("IdleChestTime", idleChestTime);
        PlayerPrefs.SetString("LastExitTime", System.DateTime.Now.ToString());
    }

    private void OnApplicationPause(bool pauseStatus) 
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetFloat("IdleTime", idleTime);
            PlayerPrefs.SetFloat("IdleChestTime", idleChestTime);
            PlayerPrefs.SetString("LastExitTime", System.DateTime.Now.ToString());
        }
    }
}
