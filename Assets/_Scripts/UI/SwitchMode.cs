using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Mode
{
    public GameObject _object;
    public Vector3 _position;
    public GameObject _UIPanel;
    public GameObject _Button;
    public bool _canBeDisabled;
}

public class SwitchMode : MonoBehaviour
{
    private static SwitchMode _instance;
    public static SwitchMode Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SwitchMode>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SwitchMode");
                    _instance = go.AddComponent<SwitchMode>();
                }
            }
            return _instance;
        }
    }
    [SerializeField] private List<Mode> modes;

    private int currentMode = 0;

    public int CurrentMode { get => currentMode; }

    public void SetMode(int mode)
    {
        
        if (mode < 0 || mode >= modes.Count || mode == currentMode)
            return;
        
        if (mode != 0)
            BossSystem.Instance.PauseBoss = true;
        else
            BossSystem.Instance.PauseBoss = false;

        if (modes[currentMode]._UIPanel != null)
        {
            modes[currentMode]._UIPanel.SetActive(false);
            // if (modes[currentMode]._canBeDisabled)
            //     modes[currentMode]._object.SetActive(false);
        }

        if (modes[currentMode]._Button != null)
        {
            modes[currentMode]._Button.GetComponent<Button>().interactable = true;
            modes[mode]._Button.GetComponent<Button>().interactable = false;
        }

        modes[currentMode]._object.transform.position = new Vector3(modes[currentMode]._position.x, modes[currentMode]._position.y, 100);
        currentMode = mode;
        // if (modes[currentMode]._canBeDisabled)
        //         modes[currentMode]._object.SetActive(true);
        modes[currentMode]._UIPanel.SetActive(true);
        modes[currentMode]._object.transform.position = new Vector3(0, 0, 100);
        
        DialogueSystem.Instance.SetByMode(currentMode);
        return;
    }

    private void Start() 
    {
        if (modes[currentMode]._Button != null)
        {
            modes[currentMode]._Button.GetComponent<Button>().interactable = false;
        }
    }
}
