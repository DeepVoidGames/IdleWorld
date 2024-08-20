using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Toggle fullscreenToggle;

    void Start()
    {
        // Initialize the toggle's state based on the current fullscreen status
        fullscreenToggle.isOn = Screen.fullScreen;

        // Add listener for when the value of the toggle changes
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        // Set the game's fullscreen mode based on the toggle's value
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
}