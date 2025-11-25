using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Soul-like/Character Stats", order = 1)]
public class CharacterStats_SO : ScriptableObject
{
    [Header("Core Stats")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;

    [Header("Stamina Costs & Regeneration")]
    public float staminaRegenRate = 10f;
    public float dodgeStaminaCost = 15f;
    public float lightAttackStaminaCost = 10f;
    public float heavyAttackStaminaCost = 25f;

    // --- [เพิ่มใหม่] ---
    // LEAD COMMENT: เราเพิ่มค่าพลังใหม่เข้าไปใน ScriptableObject โดยตรง
    // ทำให้ Game Designer สามารถตั้งค่าพลังโจมตีและป้องกันพื้นฐานของตัวละครได้จากไฟล์ .asset
    // โดยไม่ต้องแตะโค้ดเลย นี่คือพลังของ Data-Oriented Design
    [Header("Combat Stats")]
    [Tooltip("พลังโจมตีพื้นฐาน (จะถูกบวกเพิ่มเข้าไปกับดาเมจของอาวุธ)")]
    public float attackPower = 0f;

    [Tooltip("พลังป้องกันพื้นฐาน (จะถูกนำไปหักลบกับดาเมจที่ได้รับ)")]
    public float defense = 0f;
}