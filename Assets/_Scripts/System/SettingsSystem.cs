using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSystem : MonoBehaviour
{

    private static SettingsSystem _instance;
    public static SettingsSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SettingsSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SettingsSystem");
                    _instance = go.AddComponent<SettingsSystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private float _playTime;
    [SerializeField] private float startTime;
    [Header("UI")]
    [SerializeField] private Text playTimeText;

    private float _timer;

    private void Load()
    {
        _playTime = PlayerPrefs.GetFloat("PlayTime");
    }

    public void Save()
    {
        _playTime += Time.time - startTime;
        PlayerPrefs.SetFloat("PlayTime", _playTime);
        startTime = Time.time;
    }

    private void UpdateUI()
    {
        if (_playTime < 60)
        {
            playTimeText.text = "Play Time: " + _playTime.ToString("F0") + "s";
        }
        else if (_playTime < 3600)
        {
            playTimeText.text = "Play Time: " + (_playTime / 60).ToString("F0") + "m";
        }
        else
        {
            playTimeText.text = "Play Time: " + (_playTime / 3600).ToString("F0") + "h";
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution[] resolutions = Screen.resolutions;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void Start()
    {
        // if (PlayerPrefs.HasKey("Fullscreen"))
        //     SetFullscreen(PlayerPrefs.GetInt("Fullscreen") == 1);
        // else
        //     SetFullscreen(false);

        // if (PlayerPrefs.HasKey("Resolution"))
        //     SetResolution(PlayerPrefs.GetInt("Resolution"));
        // else
        //     Screen.SetResolution(1920, 1080, false);
    
        startTime = Time.time;
        if (!PlayerPrefs.HasKey("PlayTime"))
        {
            PlayerPrefs.SetFloat("PlayTime", 0);
        }
        Load();
        UpdateUI();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= 60)
        {
            Save();
            UpdateUI();
            _timer = 0;
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
