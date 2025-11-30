using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class BossStatus : MonoBehaviour
{
    [Header("Data Source")]
    public CharacterStats_SO baseStats;

    [Header("Event Channels")]
    public GameEvent onBossDamaged;
    public GameEvent onCameraShake;

    [Header("Direct Events")]
    public UnityEvent OnBossDefeated;

    [Header("Game Feel & Feedback")]
    public AudioClip hitSound;
    public GameObject hitVFX; 

    private float _currentHealth;
    public float CurrentHealth => _currentHealth;
    private Animator _animator;
    private BossAIController _aiController;
    private AudioSource _audioSource;
    private bool _isDead = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _aiController = GetComponent<BossAIController>();
        _audioSource = GetComponent<AudioSource>();
        _currentHealth = baseStats.maxHealth;
    }

    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        TriggerHitFeedback(hitPoint);

        if (onBossDamaged != null) onBossDamaged.Raise();
        if (onCameraShake != null) onCameraShake.Raise();

        if (_currentHealth <= 0) Die();
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, transform.position);
    }

    private void TriggerHitFeedback(Vector3 hitPoint)
    {
        if (hitSound != null && _audioSource != null) _audioSource.PlayOneShot(hitSound);
        if (_aiController != null) _aiController.TriggerHitStun();

        if (hitVFX != null)
        {
            Quaternion hitRotation = Quaternion.LookRotation(hitPoint - transform.position);
            Instantiate(hitVFX, hitPoint, hitRotation);
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        Debug.LogWarning("Boss has been defeated!");

        if (_aiController != null) _aiController.enabled = false;
        if (GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        }

        if (_animator != null)
        {
            _animator.Play("Death");
        }

        Invoke(nameof(TriggerVictorySequence), 2f);
    }

    private void TriggerVictorySequence()
    {
        OnBossDefeated.Invoke();
    }
}
