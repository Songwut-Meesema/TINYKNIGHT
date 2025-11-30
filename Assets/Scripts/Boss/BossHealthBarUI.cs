using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("Dependencies")]
    public BossStatus bossStatus;

    [Header("UI Elements")]
    public Slider healthBarSlider;
    public TextMeshProUGUI bossNameText;

    private void Awake()
    {
        if (bossStatus == null || healthBarSlider == null)
        {
            Debug.LogError("BossHealthBarUI is missing dependencies!", this.gameObject);
            return;
        }

        healthBarSlider.maxValue = bossStatus.baseStats.maxHealth;
        if (bossNameText != null)
        {
            bossNameText.text = bossStatus.gameObject.name;
        }

        healthBarSlider.gameObject.SetActive(false);
        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        if (healthBarSlider.gameObject.activeSelf) return;

        UpdateHealth();

        healthBarSlider.gameObject.SetActive(true);
        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(true);
        }
        Debug.Log("Boss Health Bar is now visible and updated.");
    }

    public void UpdateHealth()
    {
        if (bossStatus != null && healthBarSlider != null)
        {
            healthBarSlider.value = bossStatus.CurrentHealth;
        }
    }

    public void Hide()
    {
        if (!healthBarSlider.gameObject.activeSelf) return;

        healthBarSlider.gameObject.SetActive(false);
        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(false);
        }
        Debug.Log("Boss Health Bar is now hidden.");
    }
}
