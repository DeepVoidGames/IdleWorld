using UnityEngine;
using UnityEngine.UI;

public class Rock : MonoBehaviour
{
    public int maxHP = 10;
    private int currentHP;
    private RockSystem rockSystem;
    [SerializeField]
    private Slider healthSlider; // Referencja do paska zdrowia

    [System.Serializable]
    public struct ResourceDrop
    {
        public string resourceName;
        public float dropChance; // Procent szansy na wypadnięcie
    }

    public ResourceDrop[] resourceDrops; // Szanse na drop dla tego kamienia

    void Start()
    {
        currentHP = maxHP;
        rockSystem = FindObjectOfType<RockSystem>(); // Znajdź system zarządzający kamieniami
        healthSlider = GetComponentInChildren<Slider>(); // Znajdź pasek zdrowia w dziecku tego obiektu
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }
    }

    void OnMouseDown()
    {
        TakeDamage(1);
    }

    void TakeDamage(int amount)
    {
        currentHP -= amount;
        UpdateHealthBar();

        if (currentHP <= 0)
        {
            DestroyRock();
        }
    }

    void DestroyRock()
    {
        rockSystem.SpawnResource(resourceDrops); // Dodanie surowca do ekwipunku
        Destroy(gameObject); // Usunięcie kamienia
        rockSystem.SpawnNewRockOrResource(); // Powiadomienie systemu o konieczności pojawienia się nowego kamienia lub surowca
    }
}
