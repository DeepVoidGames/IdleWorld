using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spell 
{
    public string name;
    public string description;
    public float cooldown;
    public float duration;
    public float damage;


}

public class SpellSystem : MonoBehaviour
{

    private static SpellSystem _instance;
    public static SpellSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SpellSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SpellSystem");
                    _instance = go.AddComponent<SpellSystem>();
                }
            }
            return _instance;
        }
    }

    [Header("Spells")]
    [SerializeField] private List<Spell> spells = new List<Spell>();

    
}
