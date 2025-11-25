using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController), typeof(AudioSource))]
public class PlayerStatus : MonoBehaviour
{
    [Header("Data Source")]
    public CharacterStats_SO baseStats;

    [Header("Event Channels")]
    public GameEvent onHealthChanged;
    public GameEvent onStaminaChanged;

    [Header("Game Feel & Feedback")]
    [Tooltip("เสียงที่จะเล่นเมื่อผู้เล่นได้รับความเสียหาย")]
    public AudioClip hitSound;
    [Tooltip("เสียงเหวี่ยงอาวุธ (Swoosh)")]
    public AudioClip swingSound;
    [Tooltip("เสียงกลิ้งหลบ")]
    public AudioClip dodgeSound;
    [Tooltip("เสียงเมื่อพยายามใช้ Stamina แต่ไม่พอ")]
    public AudioClip noStaminaSound;

    [HideInInspector]
    public CharacterStats_SO runtimeStats;

    private float _currentHealth;
    private float _currentStamina;
    private bool _isDead = false;

    private PlayerController _playerController;
    private Animator _animator;
    private PlayerInput _playerInput;
    private AudioSource _audioSource;

    public float GetCurrentHealth() => _currentHealth;
    public float GetCurrentStamina() => _currentStamina;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _audioSource = GetComponent<AudioSource>();
        runtimeStats = Instantiate(baseStats);
    }

    private void Start()
    {
        _currentHealth = runtimeStats.maxHealth;
        _currentStamina = runtimeStats.maxStamina;
        if (onHealthChanged != null) onHealthChanged.Raise();
        if (onStaminaChanged != null) onStaminaChanged.Raise();
    }

    private void Update()
    {
        if (_isDead) return;
        float oldStamina = _currentStamina;
        if (_currentStamina < runtimeStats.maxStamina)
        {
            _currentStamina += runtimeStats.staminaRegenRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, runtimeStats.maxStamina);
        }
        if (oldStamina != _currentStamina && onStaminaChanged != null)
        {
            onStaminaChanged.Raise();
        }
    }

    public bool HasEnoughStamina(float amount)
    {
        return _currentStamina >= amount;
    }

    public void UseStamina(float amount)
    {
        if (HasEnoughStamina(amount))
        {
            _currentStamina -= amount;
            if (onStaminaChanged != null) onStaminaChanged.Raise();
        }
    }

    public void TakeDamage(float incomingDamage)
    {
        if (_isDead) return;
        float finalDamage = Mathf.Max(1f, incomingDamage - runtimeStats.defense);
        _currentHealth -= finalDamage;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        if (hitSound != null && _audioSource != null) _audioSource.PlayOneShot(hitSound);
        if (onHealthChanged != null) onHealthChanged.Raise();
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            _playerController.PlayGetHitAnimation();
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        Debug.LogError("PLAYER HAS DIED.");
        if (_animator != null) _animator.SetTrigger("Death");
        if (_playerController != null) _playerController.enabled = false;
        if (_playerInput != null) _playerInput.DeactivateInput();
        if (GetComponent<CharacterController>() != null) GetComponent<CharacterController>().enabled = false;
        Invoke(nameof(ShowGameOverScreen), 2f);
    }

    private void ShowGameOverScreen()
    {
        GameUIManager.Instance.ShowEndScreen(false);
        GameManager.Instance.EnterUIMode();
    }

    public void PlaySwingSound()
    {
        if (swingSound != null) _audioSource.PlayOneShot(swingSound);
    }

    public void PlayDodgeSound()
    {
        if (dodgeSound != null) _audioSource.PlayOneShot(dodgeSound);
    }

    public void PlayNoStaminaSound()
    {
        if (noStaminaSound != null) _audioSource.PlayOneShot(noStaminaSound);
    }

    #region Stat Upgrades
    public void UpgradeMaxHealth(float amount)
    {
        runtimeStats.maxHealth += amount;
        _currentHealth += amount;
        if (onHealthChanged != null) onHealthChanged.Raise();
    }
    public void UpgradeMaxStamina(float amount)
    {
        runtimeStats.maxStamina += amount;
        _currentStamina += amount;
        if (onStaminaChanged != null) onStaminaChanged.Raise();
    }
    public void UpgradeStaminaRegen(float amount)
    {
        runtimeStats.staminaRegenRate += amount;
    }
    public void UpgradeAttackPower(float amount)
    {
        runtimeStats.attackPower += amount;
    }
    public void UpgradeDefense(float amount)
    {
        runtimeStats.defense += amount;
    }
    #endregion
}