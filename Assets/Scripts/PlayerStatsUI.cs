using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public TMP_Text damageText;
    public TMP_Text critChanceText;
    public TMP_Text critDamageText;
    public TMP_Text healthText;

    public PlayerScript playerScript;
    public PlayerHealth playerHealth;

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        damageText.text = "Damage: " + playerScript.baseDamage.ToString();
        critChanceText.text = "Crit Chance: " + (playerScript.critChance * 100).ToString("F1") + "%";
        critDamageText.text = "Crit Damage: " + (playerScript.critDamageMultiplier * 100).ToString("F1") + "%";
        healthText.text = "Health: " + (playerHealth.maxHealth).ToString();
    }
}
