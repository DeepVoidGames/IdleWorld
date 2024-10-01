using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MiningSkill
{
    public int id;
    public string name;
    public int level;
    public int maxLevel;
    public double cost;
    public bool isPurchased;
}


public class MiningSkillTree : MonoBehaviour
{
    private static MiningSkillTree _instance;
    public static MiningSkillTree Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MiningSkillTree>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MiningSkillTree");
                    _instance = go.AddComponent<MiningSkillTree>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private List<MiningSkill> skills;

    [Header("Skill Tree")]
    [SerializeField] private int requiredMiningLevel = 1000;
    [SerializeField] private int skillPoints;

    [Header("UI Skill Points")]
    [SerializeField] private Text SkillPointsText;
    [SerializeField] private GameObject SkillPointsButton;
    [Header("UI Skill")]
    [SerializeField] private Text BonusText;
    [SerializeField] private Text CostText;
    [SerializeField] private Button BuyButton;

    private void UpdateUI()
    {
        SkillPointsText.text = "Skill Points: " + UISystem.Instance.NumberFormatInt(skillPoints);
    }

    private void UpdateSkillPointsButton(double experienceAdded = 0)
    {
        if(MiningSystem.Instance.MiningLevel >= requiredMiningLevel)
        {
            SkillPointsButton.GetComponent<Image>().color = UISystem.Instance.buyButtonColor;
            SkillPointsButton.GetComponent<Button>().interactable = true;

            SkillPointsButton.GetComponent<Button>().onClick.RemoveAllListeners();
            SkillPointsButton.GetComponent<Button>().onClick.AddListener(AddSkillPoint);
        }
        else
        {
            SkillPointsButton.GetComponent<Image>().color = UISystem.Instance.buyButtonDisabledColor;
            SkillPointsButton.GetComponent<Button>().interactable = false;
            SkillPointsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void AddSkillPoint()
    {
        Debug.Log($"Add skill point to {requiredMiningLevel} | {MiningSystem.Instance.MiningLevel}");
        if(MiningSystem.Instance.MiningLevel >= requiredMiningLevel)
        {
            MiningSystem.Instance.RemoveMiningLevel(requiredMiningLevel);
            skillPoints++;
            UpdateUI();
            UpdateSkillPointsButton();
        }
    }

    public void ShowSkillData(int id)
    {
        BonusText.gameObject.SetActive(true);
        CostText.gameObject.SetActive(true);
        BuyButton.gameObject.SetActive(true);

        BonusText.text = skills[id].name + " (" + skills[id].level + "/" + skills[id].maxLevel + ")";
        CostText.text = $"Cost: {UISystem.Instance.NumberFormat(skills[id].cost)}";

        if(skillPoints <= 0)
        {
            BuyButton.GetComponent<Image>().color = UISystem.Instance.buyButtonDisabledColor;
            BuyButton.interactable = false;
            BuyButton.GetComponentInChildren<Text>().text = "No Skill Points";
        }
        else if(skills[id].level >= skills[id].maxLevel)
        {
            BuyButton.GetComponent<Image>().color = UISystem.Instance.buyButtonMaxedColor;
            BuyButton.interactable = false;
            BuyButton.GetComponentInChildren<Text>().text = "Maxed";
        }
        else
        {
            BuyButton.GetComponent<Image>().color = UISystem.Instance.buyButtonColor;
            BuyButton.interactable = true;
            BuyButton.GetComponentInChildren<Text>().text = "Purchase";
        } 

        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(() => BuySkill(id));

    }

    public void BuySkill(int id)
    {
        if(skillPoints <= 0)
            return;
        if(skills[id].level >= skills[id].maxLevel)
            return;
        if(skillPoints < skills[id].cost)
            return;

        skills[id].level++;
        skillPoints -= (int)skills[id].cost;
        if(skills[id].level != 0)
            skills[id].isPurchased = true;

        AddBonus(id);
        UpdateUI();
        ShowSkillData(id);
        SaveSkill(id);
    }

    //TODO
    private void AddBonus(int id)
    {
        switch (id)
        {
            case 0:
                Debug.Log("Add bonus to skill 0");
                break;
            default:
                break;
        }
    }


    private void SaveSkillPoints()
    {
        PlayerPrefs.SetInt("MiningSkillPoints", skillPoints);
    }

    private void SaveSkill(int id)
    {
        PlayerPrefs.SetInt("MiningSkillTree_" + id, skills[id].level);
    }

    private void LoadSkill(int id)
    {
        skills[id].level = PlayerPrefs.GetInt("MiningSkillTree_" + id, 0);
        if (skills[id].level != 0)
            skills[id].isPurchased = true;
    }

    private void Start()
    {
        UpdateUI();
        StartCoroutine(UpdateSkillPointsButtonCoroutine());
        MiningSystem.Instance.OnExperienceAdded += UpdateSkillPointsButton;

        foreach (var skill in skills)
        {
            LoadSkill(skill.id);
        }

        if (PlayerPrefs.HasKey("MiningSkillPoints"))
        {
            skillPoints = PlayerPrefs.GetInt("MiningSkillPoints", 0);
            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        MiningSystem.Instance.OnExperienceAdded -= UpdateSkillPointsButton;
        SaveSkillPoints();
    }

    private void OnApplicationQuit()
    {
        SaveSkillPoints();
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
            SaveSkillPoints();
    }

    IEnumerator UpdateSkillPointsButtonCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateSkillPointsButton();
    }
}
