using UnityEngine;
using UnityEngine.UI;

public class MapTile : MonoBehaviour 
{
    [SerializeField] private string caveName;
    [SerializeField] private Text title;
    [SerializeField] private Text cost;
    [SerializeField] private Button button;
    [SerializeField] private GameObject image;

    private Cave cave;

    public void BuyTile()
    {
        
    }

    private void UpdateUI()
    {
        title.text = caveName;
        if(cave.isUnlocked)
        {
            // Change text to "Enter"
            Text text = button.GetComponentInChildren<Text>();

            image.SetActive(false);
            text.text = "Enter";

            cost.text = "Unlocked";
        }
        else
        {
            // Change text to "Unlock"
            Text text = button.GetComponentInChildren<Text>();

            image.SetActive(true);
            text.text = "Unlock";

            cost.text = UISystem.Instance.NumberFormat(cave.costToEnter);
        }
    }

    private void Start() 
    {
        cave = CaveSystem.Instance.GetCave(caveName);
        if (cave == null)
        {
            Debug.LogError("Cave not found");
            return;
        }

        UpdateUI();
    }
}