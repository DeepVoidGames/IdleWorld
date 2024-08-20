using UnityEngine;

public class Toggle : MonoBehaviour
{
    [SerializeField] private string _name;

    private void Start()
    {
        if (PlayerPrefs.HasKey(_name))
        {
            GetComponent<UnityEngine.UI.Toggle>().isOn = PlayerPrefs.GetInt(_name) == 1;
        }
    }

    public void Set(bool value)
    {
        PlayerPrefs.SetInt(_name, value ? 1 : 0);
    }
}
