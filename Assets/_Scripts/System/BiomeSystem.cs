using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biomes
{
    public string Name;

    public Sprite background;

    public List<Monster> Monsters = new List<Monster>();
    public List<Boss> Bosses = new List<Boss>();
}

public class BiomeSystem : MonoBehaviour 
{
    private static BiomeSystem _instance;
    public static BiomeSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BiomeSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("BiomSystem");
                    _instance = go.AddComponent<BiomeSystem>();
                }
            }
            return _instance;
        }
    }

    public List<Biomes> Bioms = new List<Biomes>();

    [SerializeField] private string currentBiome = "Forest";

    public string CurrentBiome { get => currentBiome; set => currentBiome = value;}

    public void NextBiome()
    {
        int index = Bioms.FindIndex(biome => biome.Name == currentBiome);
        index++;
        if (index >= Bioms.Count)
        {
            index = 0;
        }
        currentBiome = Bioms[index].Name;
    }
}