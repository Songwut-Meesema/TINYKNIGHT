using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// LEAD COMMENT: นี่คือ Script ที่ควบคุม UI แถบเลือดของบอสโดยเฉพาะ (ฉบับแก้ไขสมบูรณ์)
// Script นี้จะ Active อยู่ตลอดเวลาเพื่อ "ฟัง" Event
// และจะทำการเปิด/ปิดเฉพาะ Child GameObjects ที่เป็นส่วนแสดงผลภาพเท่านั้น
public class BossHealthBarUI : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("ลาก GameObject ของบอสในฉากมาใส่ที่นี่ เพื่อดึงข้อมูลตั้งต้น")]
    public BossStatus bossStatus;

    [Header("UI Elements")]
    [Tooltip("Slider ที่จะแสดงค่าพลังชีวิต")]
    public Slider healthBarSlider;
    [Tooltip("Text ที่จะแสดงชื่อของบอส")]
    public TextMeshProUGUI bossNameText;

    private void Awake()
    {
        if (bossStatus == null || healthBarSlider == null)
        {
            Debug.LogError("BossHealthBarUI is missing dependencies!", this.gameObject);
            return;
        }

        // ตั้งค่า MaxValue และชื่อบอส
        healthBarSlider.maxValue = bossStatus.baseStats.maxHealth;
        if (bossNameText != null)
        {
            bossNameText.text = bossStatus.gameObject.name;
        }

        // --- [แก้ไข] ---
        // ไม่ต้องตั้งค่า value ที่นี่แล้ว เพราะค่า CurrentHealth อาจจะยังไม่ถูกตั้ง
        // เราจะไปตั้งค่า value ใน Show() แทน
        // และซ่อนเฉพาะส่วนที่มองเห็นได้
        healthBarSlider.gameObject.SetActive(false);
        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(false);
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกโดย Event "OnBossAggro"
    public void Show()
    {
        if (healthBarSlider.gameObject.activeSelf) return;

        // --- [THE FIX & UPGRADE] ---
        // 1. อัปเดตค่าเลือด "ทันที" ที่จะแสดง UI
        // เพื่อให้แน่ใจว่าเราได้ค่าเลือดล่าสุดจาก BossStatus (ที่ตั้งค่าใน Awake ของมันแล้ว)
        // นี่คือการการันตีว่าแถบเลือดจะเต็มเสมอเมื่อปรากฏครั้งแรก
        UpdateHealth();

        // 2. เปิดการมองเห็นของ Slider และ Text
        healthBarSlider.gameObject.SetActive(true);
        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(true);
        }
        Debug.Log("Boss Health Bar is now visible and updated.");
    }

    // ฟังก์ชันนี้จะถูกเรียกโดย Event "onBossDamaged"
    public void UpdateHealth()
    {
        if (bossStatus != null && healthBarSlider != null)
        {
            healthBarSlider.value = bossStatus.CurrentHealth;
        }
    }

    // (Optional) ฟังก์ชันสำหรับซ่อน UI เมื่อบอสตาย
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