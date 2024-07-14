using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Biomes
{
    public string Name;

    public Sprite background;

    public List<Monster> Monsters = new List<Monster>();
    public List<Boss> Bosses = new List<Boss>();
}

public class BiomeDataWrapper
{
    public List<Biomes> biomes = new List<Biomes>();
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

    [Header("UI")]
    [SerializeField] private Image imageBackground;

    public string CurrentBiome { get => currentBiome; set => currentBiome = value;}

    public void SetCurrentBiomes(List<Biomes> biomes)
    {
        foreach (var biome in biomes)
        {
            biome.background = Resources.Load<Sprite>($"Sprites/Biomes/{biome.Name}");
            foreach (var monster in biome.Monsters)
            {
                monster.Prefab = Resources.Load<GameObject>($"Prefab/Monster/{biome.Name}/{monster.Name}");
            }
            foreach (var boss in biome.Bosses)
            {
                boss.Prefab = Resources.Load<GameObject>($"Prefab/Boss/{biome.Name}/{boss.Name}");
            }
        }
        Bioms = biomes;
    }

    public void SetCurrentBiome(string biome)
    {
        currentBiome = biome;
        UpdateUI();
    }

    public void NextBiome()
    {
        int index = Bioms.FindIndex(biome => biome.Name == currentBiome);
        index++;
        if (index >= Bioms.Count)
        {
            return;
        }
        currentBiome = Bioms[index].Name;
        imageBackground.sprite = Bioms[index].background;
    }

    private void UpdateUI()
    {
        imageBackground.sprite = Bioms.Find(biome => biome.Name == currentBiome).background;
    }

    private void Start()
    {
        UpdateUI();
    }
}
