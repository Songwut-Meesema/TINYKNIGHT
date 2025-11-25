using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Soul-like/Skill", order = 3)]
public class Skill_SO : ScriptableObject
{
    [Header("Skill Information")]
    public string skillName;
    [TextArea(3, 10)]
    public string description;

    // --- [อัปเกรด] ---
    // LEAD COMMENT: เราเปลี่ยนจาก icon เดียว เป็น 2 icons เพื่อให้ Game Designer
    // สามารถกำหนดภาพที่แตกต่างกันสำหรับแต่ละสถานะได้อย่างอิสระ
    // นี่คือหัวใจของการ Polish ในครั้งนี้
    [Tooltip("ไอคอนที่จะแสดงเมื่อสกิลถูกปลดล็อกแล้ว (ภาพสีเต็ม)")]
    public Sprite unlockedIcon;
    [Tooltip("ไอคอนที่จะแสดงเมื่อสกิลยังล็อกอยู่ (ภาพสีเทา/มีกุญแจล็อก)")]
    public Sprite lockedIcon;


    [Header("Skill Cost & Requirements")]
    public int cost;
    public List<Skill_SO> requiredSkills;

    [Header("Skill Effect")]
    public StatType statToUpgrade;
    public float upgradeValue;

    public enum StatType
    {
        MaxHealth,
        MaxStamina,
        StaminaRegenRate,
        AttackPower,
        Defense
    }
}