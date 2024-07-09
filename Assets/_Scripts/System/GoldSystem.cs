using UnityEngine;

public class GoldSystem : MonoBehaviour
{
    private static GoldSystem _instance;
    public static GoldSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GoldSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GoldSystem");
                    _instance = go.AddComponent<GoldSystem>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private float gold;

    public float Gold
    {
        get => gold;
        private set => gold = value;
    }

    public void AddGold(float amount)
    {
        Gold += amount;
        UISystem.Instance.UpdateGoldText();
        SaveSystem.Instance.Save();
    }

    public void SetGold(float amount)
    {
        Gold = amount;
        UISystem.Instance.UpdateGoldText();
    }

    public void SpendGold(float amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            UISystem.Instance.UpdateGoldText();
            SaveSystem.Instance.Save();
        }
    }
}
