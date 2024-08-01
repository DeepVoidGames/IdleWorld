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

    private void UnlockCave()
    {
        double quantity = InventorySystem.Instance.GetResourceByName(cave.resourceRequiredToEnter);
        if (quantity >= cave.costToEnter)
        {
            InventorySystem.Instance.RemoveItemByName(cave.resourceRequiredToEnter, cave.costToEnter);
            CaveSystem.Instance.UnlockCave(caveName);
            UpdateUI();
        }
    }

    private void EnterCave()
    {
        if (cave.isUnlocked)
        {
            CaveSystem.Instance.UpdateCurrentCave(caveName);
            UpdateUI();
        }
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

            if (CaveSystem.Instance.CurrentCave == caveName)
            {
                text.text = "Current";
                button.interactable = false;
                return;
            }
            else
            {
                button.interactable = true;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(EnterCave);
        }
        else
        {
            // Change text to "Unlock"
            Text text = button.GetComponentInChildren<Text>();

            image.SetActive(true);
            text.text = "Unlock";

            cost.text = UISystem.Instance.NumberFormat(cave.costToEnter);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(UnlockCave);
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

    private void OnEnable()
    {
        CaveSystem.Instance.OnCaveChanged += UpdateUI;
    }

    private void OnDisable()
    {
        CaveSystem.Instance.OnCaveChanged -= UpdateUI;
    }

}