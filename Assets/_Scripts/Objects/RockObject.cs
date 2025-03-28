using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockObject : MonoBehaviour
{
    public List<Drop> drops;
    private double health;
    private double maxHealth;

    private Text healthText;
    private MessageSpawner _messageSpawner;

    public double Health { get => health; set => health = value; }
    public double MaxHealth { get => maxHealth; set => maxHealth = value; }

    [Header("Hit Animation")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private Material defaultMaterial;

    public void Damage(double damage)
    {
        _messageSpawner.SpawnMessage(String.Format("-{0}", UISystem.Instance.NumberFormat(damage)), color: new Color(1f, 0.33f, 0.33f, 1f));
        // if (hitMaterial != null)
        //     gameObject.GetComponent<Renderer>().material = hitMaterial;
        health -= damage;
        if (health <= 0)
        {
            DestroyRock();
        }
        // if (hitMaterial != null)
        //     Invoke("ResetMaterial", .05f);

        // Material Color Effects -> Fade -> Fade Amount
        // max 0.235 min -0.1
        // Material Color Effects -> Fade -> Fade Amount
        // Shader range: -0.1 to 1
        float fadeAmount = Mathf.Clamp(0.235f - (0.235f * ((float)health / (float)maxHealth)), -0.1f, 1.0f);
        this.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_FadeAmount", fadeAmount);

        UpdateUI();
    }

    private void ResetMaterial()
    {
        gameObject.GetComponent<Renderer>().material = defaultMaterial;
    }

    private void DestroyRock()
    {
        CaveSystem.Instance.DestroyRock(this);
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        healthText.text = UISystem.Instance.NumberFormat(health) + "/" + UISystem.Instance.NumberFormat(maxHealth);
    }

    private void OnMouseDown() 
    {
        Damage(MiningSystem.Instance.MiningEfficiency);
    }

    void Start()
    {
        _messageSpawner = GetComponent<MessageSpawner>();
        healthText = transform.Find("Canvas").Find("HealthText").GetComponent<Text>();
        UpdateUI();
    }
}
