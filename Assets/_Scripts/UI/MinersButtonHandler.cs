using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MinersButtonHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject miners_panel;

    public void HandleMiners()
    {
        miners_panel.SetActive(!miners_panel.activeSelf);
    }
}
