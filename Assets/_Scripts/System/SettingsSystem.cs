using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSystem : MonoBehaviour
{
    [SerializeField] private float _playTime;
    [SerializeField] private float startTime;

    private void Load()
    {
        _playTime = PlayerPrefs.GetFloat("PlayTime");
    }

    public void Save()
    {
        _playTime += Time.time - startTime;
        PlayerPrefs.SetFloat("PlayTime", _playTime);
    }

    private void Start()
    {
        startTime = Time.time;
        if (!PlayerPrefs.HasKey("PlayTime"))
        {
            PlayerPrefs.SetFloat("PlayTime", 0);
        }
        Load();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
