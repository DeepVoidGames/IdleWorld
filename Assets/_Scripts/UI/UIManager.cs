using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Description("Basic Resources")]
    public Text stoneText;


    private void Start()
    {
        // Przypisanie obiektów do zmiennych
    }

    private void Update()
    {
        // Aktualizacja tekstu w UI
        stoneText.text = InventorySystem.Instance.items["Stone"].amount.ToString();
    }

}
