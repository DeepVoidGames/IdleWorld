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

    public void SetBiomesCollection(List<Biomes> biomes)
    {
        List<Biomes> biomesToRemove = new List<Biomes>();
    
        foreach (var biome in biomes)
        {
            biome.background = Resources.Load<Sprite>($"Sprites/Biomes/{biome.Name}");
    
            if (biome.background == null)
            {
                // If the sprite is not found remove the biome from the list
                Debug.LogWarning($"Biome {biome.Name} background sprite not found, removing biome from list");
                biomesToRemove.Add(biome);
                continue;
            }
    
            foreach (var monster in biome.Monsters)
            {
                monster.Prefab = Resources.Load<GameObject>($"Prefabs/Monster/{biome.Name}/{monster.Name}");
            }
            foreach (var boss in biome.Bosses)
            {
                boss.Prefab = Resources.Load<GameObject>($"Prefabs/Boss/{biome.Name}/{boss.Name}");
            }
        }
    
        foreach (var biomeToRemove in biomesToRemove)
        {
            biomes.Remove(biomeToRemove);
        }
    
        Bioms = biomes;
        UpdateBiome();
    }

    public void SetCurrentBiome(string biome)
    {
        currentBiome = biome;
        UpdateUI();
        MonsterSystem.Instance.ReloadMonster();
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

    public void UpdateBiome()
    {
        // Get level and set current biome, but next biome every 10 levels
        // So 0-9 levels is Forest, 10-19 is Desert, etc. 
        int level = LevelSystem.Instance.Level;

        if (level > 0)
        {
            int biomeIndex = level / 10;
            if (biomeIndex >= Bioms.Count)
            {
                biomeIndex = Bioms.Count - 1; // Clamp the biome index to the last available biome
            }
            currentBiome = Bioms[biomeIndex].Name;
        }
        imageBackground.sprite = Bioms.Find(biome => biome.Name == currentBiome).background;
        MonsterSystem.Instance.ReloadMonster();
    }

    private void UpdateUI()
    {
        imageBackground.sprite = Bioms.Find(biome => biome.Name == currentBiome).background;
    }

    private void Start()
    {
        LoadGameData.Instance.Biomes();
        UpdateUI();
    }
}
