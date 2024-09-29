using UnityEngine;

public class CraftingLevel : MonoBehaviour 
{   
    private static CraftingLevel _instance;
    public static CraftingLevel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CraftingLevel>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CraftingLevel");
                    _instance = go.AddComponent<CraftingLevel>();
                }
            }
            return _instance;
        }
    }

    
}