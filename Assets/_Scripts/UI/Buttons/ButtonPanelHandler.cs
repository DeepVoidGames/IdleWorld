using UnityEngine;

public class ButtonPanelHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void ToogleOffPanel()
    {
        panel.SetActive(false);
    }
}
