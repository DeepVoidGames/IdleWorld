using UnityEngine;

public class SwitchMode : MonoBehaviour
{
    [SerializeField] private GameObject[] modes;
    [SerializeField] private Vector3[] basePositions;

    private int currentMode = 0;

    public void SetMode(int mode)
    {
        if (mode < 0 || mode >= modes.Length)
            return;
        
        if (mode != 0)
            BossSystem.Instance.PauseBoss = true;
        else
            BossSystem.Instance.PauseBoss = false;
        modes[currentMode].transform.position = new Vector3(basePositions[currentMode].x, basePositions[currentMode].y, 100);
        currentMode = mode;
        modes[currentMode].transform.position = new Vector3(0, 0, 100);
        return;
    }
}
