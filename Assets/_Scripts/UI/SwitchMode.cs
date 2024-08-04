using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mode
{
    public GameObject _object;
    public Vector3 _position;
    public GameObject _UIPanel;
}

public class SwitchMode : MonoBehaviour
{
    [SerializeField] private List<Mode> modes;

    private int currentMode = 0;

    public void SetMode(int mode)
    {
        
        if (mode < 0 || mode >= modes.Count || mode == currentMode)
            return;
        
        if (mode != 0)
            BossSystem.Instance.PauseBoss = true;
        else
            BossSystem.Instance.PauseBoss = false;

        if (modes[currentMode]._UIPanel != null)
            modes[currentMode]._UIPanel.SetActive(false);

        modes[currentMode]._object.transform.position = new Vector3(modes[currentMode]._position.x, modes[currentMode]._position.y, 100);
        currentMode = mode;
        modes[currentMode]._UIPanel.SetActive(true);
        modes[currentMode]._object.transform.position = new Vector3(0, 0, 100);
        return;
    }
}
