using UnityEngine;
using System.Collections.Generic;

public class RockSystem : MonoBehaviour
{
    public GameObject[] resourcePrefabs; // Prefaby surowców, które mogą wypaść
    public GameObject rockPrefab; // Prefabrykat skały do pojedynczego spawnu
    public Vector3 spawnPosition = Vector3.zero; // Pozycja, gdzie mają pojawiać się nowe skały
    public Inventory playerInventory; // Referencja do ekwipunku gracza

    [System.Serializable]
    public struct RockType
    {
        public GameObject rockPrefab;
        public float spawnChance; // Procent szansy na pojawienie się
    }

    public RockType[] rockTypes;

    private Dictionary<GameObject, float> rockChances;

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
                playerInventory.AddItem(resourceDrop.resourceName);
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
                Instantiate(entry.Key, spawnPosition, Quaternion.identity);
                return;
            }
        }

        // Jeśli nie wylosowano żadnej skały, spawnuje domyślną skałę
        SpawnNewRock();
    }

    public void SpawnNewRock()
    {
        Instantiate(rockPrefab, spawnPosition, Quaternion.identity);
    }
}
