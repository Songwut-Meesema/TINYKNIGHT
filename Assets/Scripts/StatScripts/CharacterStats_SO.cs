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

    [Header("Combat Stats")]
    public float attackPower = 0.5f;

    public float defense = 0.5f;
}
