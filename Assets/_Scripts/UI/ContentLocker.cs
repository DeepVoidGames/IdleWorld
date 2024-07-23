using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Content
{
    public Button button;
    public Text text;
    public string textBefore;
    public bool isLocked;
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
                contents[i].button.interactable = false;
                contents[i].text.text = "Level " + contents[i].levelNeeded + " Needed";
            }
        }
    }

    public void CheckContent(int level)
    {
        for (int i = 0; i < contents.Length; i++)
        {
            if (contents[i].levelNeeded <= level)
            {
                contents[i].button.interactable = true;
                contents[i].text.text = contents[i].textBefore;
                contents[i].isLocked = false;
            }
        }
    }
}
