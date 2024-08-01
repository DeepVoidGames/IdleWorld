using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionControl : MonoBehaviour
{
    [SerializeField] private Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private List<Resolution> resolutionList = new List<Resolution>();

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        HashSet<string> addedResolutions = new HashSet<string>(); // To track added resolutions
        int currentResolutionIndex = 0;

        // Retrieve the saved resolution index, default to -1 if not found
        int savedResolutionIndex = PlayerPrefs.GetInt("Resolution", -1);

        // Flag to check if default resolution 1920x1080 is found
        bool defaultResolutionFound = false;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution resolution = resolutions[i];
            string resolutionIdentifier = $"{resolution.width}x{resolution.height}";

            // Check if this resolution has already been added
            if (!addedResolutions.Contains(resolutionIdentifier))
            {
                resolutionList.Add(resolution);
                options.Add(resolutionIdentifier);
                addedResolutions.Add(resolutionIdentifier); // Mark this resolution as added

                // If there's a saved resolution, use it
                if (savedResolutionIndex == resolutionList.Count - 1)
                {
                    currentResolutionIndex = savedResolutionIndex;
                }

                // Check if this is the default resolution 1920x1080
                if (resolution.width == 1920 && resolution.height == 1080)
                {
                    defaultResolutionFound = true;
                    if (savedResolutionIndex == -1) // If no saved resolution, use 1920x1080
                    {
                        currentResolutionIndex = resolutionList.Count;
                    }
                }
            }
        }

        // If default resolution 1920x1080 is not available, fallback to current screen resolution
        if (savedResolutionIndex == -1 && !defaultResolutionFound)
        {
            Resolution currentResolution = Screen.currentResolution;
            for (int i = 0; i < resolutionList.Count; i++)
            {
                if (resolutionList[i].width == currentResolution.width && resolutionList[i].height == currentResolution.height)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(resolutionDropdown.value); });
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < resolutionList.Count)
        {
            Resolution resolution = resolutionList[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("Resolution", resolutionIndex);
        }
        else
        {
            Debug.LogError("Invalid resolution index");
        }
    }
}
