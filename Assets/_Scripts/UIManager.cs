using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Description("Inventory")]
    
    public Inventory inventory;

    [Description("Basic Resources")]
    public Text stoneText;


    private void Start()
    {
        // Przypisanie obiektów do zmiennych
    }

    private void Update()
    {
        // Aktualizacja tekstu w UI
        stoneText.text = inventory.GetItemCount("Stone").ToString();
    }

}
