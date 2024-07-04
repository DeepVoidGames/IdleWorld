using UnityEngine;
using UnityEngine.UI;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private float maxHP = 10;
    private float currentHP;
    private RockSystem rockSystem;
    [SerializeField]
    private Text healthText; // Referencja do paska zdrowia

    [System.Serializable]
    public struct ResourceDrop
    {
        public string resourceName;
        public float dropChance; // Procent szansy na wypadnięcie
    }

    public ResourceDrop[] resourceDrops; // Szanse na drop dla tego kamienia

    // Zdarzenie, które zostanie wywołane, gdy kamień zostanie uszkodzony   
    public delegate void RockDamagedEventHandler(float damage);
    // Subskrybenci tego zdarzenia
    public event RockDamagedEventHandler OnRockDamaged;

    void Start()
    {
        currentHP = maxHP;
        rockSystem = FindObjectOfType<RockSystem>(); // Znajdź system zarządzający kamieniami
        healthText = GetComponentInChildren<Text>(); // Znajdź pasek zdrowia w dziecku tego obiektu
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHP + "/" + maxHP; // Aktualizacja tekstu paska zdrowia
        }
    }

    void OnMouseDown()
    {
        TakeDamage(1);
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        UpdateHealthText();

        if (currentHP <= 0)
        {
            DestroyRock();
        }

        if (OnRockDamaged != null)
        {
            OnRockDamaged(amount);
        }
    }

    void DestroyRock()
    {
        rockSystem.SpawnResource(resourceDrops); // Dodanie surowca do ekwipunku
        Destroy(gameObject); // Usunięcie kamienia
        rockSystem.SpawnNewRockOrResource(); // Powiadomienie systemu o konieczności pojawienia się nowego kamienia lub surowca
    }
}
