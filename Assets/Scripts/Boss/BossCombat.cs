using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossCombat : MonoBehaviour
{
    [Header("Dependencies")]
    public BossAIController controller;
    private Animator _animator;

    [Header("Hitboxes")]
    public BossHitbox rightWeaponHitbox; 
    public BossHitbox leftWeaponHitbox;  

    [Header("Attack Data")]
    public float attack1_Duration = 1.5f;
    public float attack2_Duration = 1.2f;
    public float attack3_Duration = 2.5f;
    private readonly int _attack1Hash = Animator.StringToHash("Attack1");
    private readonly int _attack2Hash = Animator.StringToHash("Attack2");
    private readonly int _attack3Hash = Animator.StringToHash("Attack3");
    public bool isAttacking { get; private set; } = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        if (rightWeaponHitbox == null || leftWeaponHitbox == null)
        {
            Debug.LogError("One or more weapon hitboxes are not assigned in BossCombat!", this.gameObject);
        }
        else
        {
            rightWeaponHitbox.SetActive(false);
            leftWeaponHitbox.SetActive(false);
        }
    }

    public void PerformAttack()
    {
        if (isAttacking) return;
        int attackIndex = controller.currentPhase == 1 ? Random.Range(1, 3) : Random.Range(1, 4);
        switch (attackIndex)
        {
            case 1: StartCoroutine(AttackCoroutine(_attack1Hash, attack1_Duration)); break;
            case 2: StartCoroutine(AttackCoroutine(_attack2Hash, attack2_Duration)); break;
            case 3: StartCoroutine(AttackCoroutine(_attack3Hash, attack3_Duration)); break;
        }
    }
    private IEnumerator AttackCoroutine(int attackHash, float duration)
    {
        isAttacking = true;
        _animator.SetTrigger(attackHash);
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    public void EnableRightWeaponHitbox() { if (rightWeaponHitbox != null) rightWeaponHitbox.SetActive(true); }
    public void DisableRightWeaponHitbox() { if (rightWeaponHitbox != null) rightWeaponHitbox.SetActive(false); }
    public void EnableLeftWeaponHitbox() { if (leftWeaponHitbox != null) leftWeaponHitbox.SetActive(true); }
    public void DisableLeftWeaponHitbox() { if (leftWeaponHitbox != null) leftWeaponHitbox.SetActive(false); }
    public void EnableBothWeaponHitboxes() { EnableRightWeaponHitbox(); EnableLeftWeaponHitbox(); }
    public void DisableBothWeaponHitboxes() { DisableRightWeaponHitbox(); DisableLeftWeaponHitbox(); }
}