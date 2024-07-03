using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Description("Inventory")]
    
    public Inventory inventory;

    [Description("Basic Resources")]
    public Text stoneText;
    public Image stoneImage;


    private void Start()
    {
        // Przypisanie obiektów do zmiennych
        stoneImage.sprite = inventory.GetItemSprite("Stone");
    }

    private void Update()
    {
        // Aktualizacja tekstu w UI
        stoneText.text = inventory.GetItemCount("Stone").ToString();
    }

}
