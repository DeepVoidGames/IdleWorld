using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Content
{
    public Button button;
    public string textBefore;
    public bool isLocked;
    public bool isDisabled;
    public int levelNeeded;
}

public class ContentLocker : MonoBehaviour
{
    private static ContentLocker _instance;
    public static ContentLocker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ContentLocker>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ContentLocker");
                    _instance = go.AddComponent<ContentLocker>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private Content[] contents;
    
    private void Start() 
    {
        for (int i = 0; i < contents.Length; i++)
        {
            if (contents[i].isLocked)
            {
                if (contents[i].isDisabled)
                {
                    contents[i].button.interactable = false;
                    contents[i].button.GetComponentInChildren<Text>().text = "Disabled";
                    if (contents[i].button.transform.childCount > 1)
                        contents[i].button.transform.GetChild(1).gameObject.SetActive(false);
                    continue;
                }

                // contents[i].button.gameObject.SetActive(false);
                contents[i].button.interactable = false;
                contents[i].button.GetComponentInChildren<Text>().text = "Level " + contents[i].levelNeeded + " Needed";
                // Find game object in children of button and set it to false
                if (contents[i].button.transform.childCount > 1)
                    contents[i].button.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void CheckContent(int level)
    {
        for (int i = 0; i < contents.Length; i++)
        {
            if (contents[i].levelNeeded > level && contents[i].levelNeeded > LevelSystem.Instance.HighestLevel)
                continue;

            // contents[i].button.gameObject.SetActive(true);

            contents[i].button.interactable = true;
            contents[i].button.GetComponentInChildren<Text>().text = contents[i].textBefore;
            contents[i].isLocked = false;
            if (contents[i].button.transform.childCount > 1)
                contents[i].button.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
