using UnityEngine;
using System.Collections.Generic;

public class RockSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject parentObject; // Obiekt, do którego będą przypisywane nowe skały
    [SerializeField]
    private GameObject rockPrefab; // Prefabrykat skały do pojedynczego spawnu
    [SerializeField]
    private Vector3 spawnPosition = Vector3.zero; // Pozycja, gdzie mają pojawiać się nowe skały
    [System.Serializable]
    public struct RockType
    {
        public GameObject rockPrefab;
        public float spawnChance; // Procent szansy na pojawienie się
    }

    [Header("Rock Types")]
    [SerializeField]
    private RockType[] rockTypes;

    private Dictionary<GameObject, float> rockChances;
    private Rock currentRock; // Referencja do aktualnie istniejącego kamienia

    void Start()
    {
        // Inicjalizacja słownika szans na pojawienie się skał
        rockChances = new Dictionary<GameObject, float>();
        foreach (RockType rockType in rockTypes)
        {
            rockChances[rockType.rockPrefab] = rockType.spawnChance;
        }

        // Początkowe ustawienie kamienia na scenie
        SpawnNewRock();
    }

    public void SpawnResource(Rock.ResourceDrop[] resourceDrops)
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (Rock.ResourceDrop resourceDrop in resourceDrops)
        {
            cumulativeChance += resourceDrop.dropChance;
            if (randomValue <= cumulativeChance)
            {
                InventorySystem.Instance.AddItem(resourceDrop.resourceName);
                return;
            }
        }
    }

    public void SpawnNewRockOrResource()
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (KeyValuePair<GameObject, float> entry in rockChances)
        {
            cumulativeChance += entry.Value;
            if (randomValue <= cumulativeChance)
            {
                GameObject newRockObject = Instantiate(entry.Key, spawnPosition, Quaternion.identity, parentObject.transform);
                currentRock = newRockObject.GetComponent<Rock>();
                return;
            }
        }

        // Jeśli nie wylosowano żadnej skały, spawnuje domyślną skałę
        SpawnNewRock();
    }

    public void SpawnNewRock()
    {
        GameObject newRockObject = Instantiate(rockPrefab, spawnPosition, Quaternion.identity, parentObject.transform);
        currentRock = newRockObject.GetComponent<Rock>();
    }

    public void DamageCurrentRock(float damage)
    {
        if (currentRock != null)
        {
            currentRock.TakeDamage(damage);
        }
    }
}
