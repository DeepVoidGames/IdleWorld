using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public Sprite sprite;
    public List<String> listString = new List<String>();
    public Vector2 size;
    public int mode;
}

public class DialogueSystem : MonoBehaviour
{
    private static DialogueSystem _instance;
    public static DialogueSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogueSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("DialogueSystem");
                    _instance = go.AddComponent<DialogueSystem>();
                }
            }
            return _instance;
        }
    }

    public List<Dialogue> dialogues = new List<Dialogue>();

    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private Text dialogText;
    [SerializeField] private Image dialogImage;
    [SerializeField] private Button nextButton;

    int _count = 0;

    private void DialogueButton(Dialogue obj)
    {
        int index = obj.listString.Count - 1;
        if (index > _count)
        {
            _count += 1;
            dialogText.text = obj.listString[_count];
        }
        else
        {
            dialogPanel.SetActive(false);
        }
            

        if(index == _count)
        {
            nextButton.GetComponentInChildren<Text>().text = "Close";
            SaveDialogueShowed(obj.mode);
        }
        else if (index > _count)
        {
            nextButton.GetComponentInChildren<Text>().text = "Next";
        }
        else if (index < _count)
        {
            nextButton.GetComponentInChildren<Text>().text = "Close";
            SaveDialogueShowed(obj.mode);
        }
    }

    private void SetDialogue(Dialogue obj)
    {
        if (PlayerPrefs.GetInt("DialogueShowed" + obj.mode) == 1)
            return;
        dialogImage.sprite = obj.sprite;
        dialogImage.gameObject.transform.GetComponent<RectTransform>().sizeDelta = obj.size;
        dialogText.text = obj.listString[0];

        nextButton.onClick.RemoveAllListeners();
        _count = 0;
        nextButton.onClick.AddListener(() => DialogueButton(obj));
        nextButton.GetComponentInChildren<Text>().text = "Next";

        dialogPanel.SetActive(true);
    }
   
    public void SetByMode(int mode)
    {
        foreach (var dialogue in dialogues)
        {
            if (dialogue.mode == mode)
            {
                SetDialogue(dialogue);
            }
        }
    }

    private void SaveDialogueShowed(int mode)
    {
        PlayerPrefs.SetInt("DialogueShowed" + mode, 1);
    }

    private void Start() 
    {
        for (int i = 0; i < dialogues.Count; i++)
        {
            PlayerPrefs.SetInt("DialogueShowed" + dialogues[i].mode, 0);
        }
        SetByMode(SwitchMode.Instance.CurrentMode);
    }
}
