using UnityEngine;

public class SwitchMode : MonoBehaviour
{
    [SerializeField] private GameObject[] modes;

    private int currentMode = 0;

    public void SetMode(int mode)
    {
        if (mode < 0 || mode >= modes.Length)
        {
            return;
        }

        if (currentMode == mode)
        {
            modes[currentMode].transform.position = new Vector3(15, 0, 100);
            currentMode = 0;
            modes[currentMode].transform.position = new Vector3(0, 0, 100);
            return;
        }

        modes[currentMode].transform.position = new Vector3(15, 0, 100);
        currentMode = mode;
        modes[currentMode].transform.position = new Vector3(0, 0, 100);
    }
}
