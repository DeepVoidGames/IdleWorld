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

    [SerializeField] private double gold;

    public event System.Action<double> OnGoldChanged;

    public double Gold
    {
        get => gold;
        private set => gold = value;
    }

    public void AddGold(double amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(gold);
        UISystem.Instance.UpdateGoldText();
    }

    public void SetGold(double amount)
    {
        Gold = amount;
        UISystem.Instance.UpdateGoldText();
    }

    public void SpendGold(double amount)
    {
        if (gold >= amount)
        {
            gold = gold - amount;
            OnGoldChanged?.Invoke(gold);
            UISystem.Instance.UpdateGoldText();
            UISystem.Instance.MoneyIndicator(amount, "-");
        }
    }
}
