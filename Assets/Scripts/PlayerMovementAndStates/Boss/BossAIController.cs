using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(BossStatus), typeof(BossCombat))]
public class BossAIController : MonoBehaviour
{
    [Header("Core Dependencies")]
    public Transform playerTarget;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public BossStatus status;
    [HideInInspector] public Animator animator;
    [HideInInspector] public BossCombat combat;

    [Header("Event Channels")]
    public GameEvent OnBossAggro;

    // --- [เพิ่มใหม่] ---
    [Header("Game Feel Settings")]
    [Tooltip("ระยะเวลาที่บอสจะชะงัก (หน่วยเป็นวินาที) เมื่อถูกโจมตี")]
    public float hitStunDuration = 0.15f;
    private Coroutine _hitStunCoroutine;

    // ... (ส่วน State Machine และ AI Parameters เหมือนเดิมทุกประการ) ...
    private IState _currentState;
    public IdleState idleState;
    public ChaseState chaseState;
    public AttackState attackState;
    public CooldownState cooldownState;
    [Header("AI Parameters")]
    public float aggroRange = 15f;
    public float attackRange = 3f;
    [Header("Phase Logic")]
    public int currentPhase = 1;
    public float phase2HealthThreshold = 0.5f;
    [Header("Phase 1 Settings")]
    public float phase1_ChaseSpeed = 3.5f;
    public float phase1_CooldownTime = 2.0f;
    [Header("Phase 2 Settings")]
    public float phase2_ChaseSpeed = 5.0f;
    public float phase2_CooldownTime = 1.0f;
    public float CurrentChaseSpeed => currentPhase == 1 ? phase1_ChaseSpeed : phase2_ChaseSpeed;
    public float CurrentCooldownTime => currentPhase == 1 ? phase1_CooldownTime : phase2_CooldownTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        status = GetComponent<BossStatus>();
        animator = GetComponent<Animator>();
        combat = GetComponent<BossCombat>();
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this, combat);
        cooldownState = new CooldownState(this);
    }

    void Start()
    {
        SwitchState(idleState);
    }
    void Update()
    {
        _currentState?.Update();
        if (currentPhase == 1 && status.CurrentHealth / status.baseStats.maxHealth <= phase2HealthThreshold)
        {
            SwitchToPhase2();
        }
    }
    public void SwitchState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    private void SwitchToPhase2()
    {
        currentPhase = 2;
        Debug.LogWarning("BOSS HAS ENTERED PHASE 2!");
    }

    // --- [เพิ่มใหม่] ---
    // LEAD COMMENT: นี่คือฟังก์ชัน Public ที่ BossStatus จะเรียกใช้
    // เราใช้ Coroutine เพื่อจัดการกับการ "หยุดเวลา" โดยไม่ไปบล็อกการทำงานของเกมทั้งหมด
    // นี่คือ Pattern ที่ดีมากสำหรับการจัดการ Action ที่ต้องใช้เวลา
    public void TriggerHitStun()
    {
        // หยุด Coroutine เก่า (ถ้ามี) ก่อนเริ่มอันใหม่ เพื่อป้องกันการทำงานซ้อนกัน
        if (_hitStunCoroutine != null)
        {
            StopCoroutine(_hitStunCoroutine);
        }
        _hitStunCoroutine = StartCoroutine(HitStunCoroutine());
    }

    private IEnumerator HitStunCoroutine()
    {
        // 1. หยุดการเคลื่อนที่ของบอสทันที
        if (agent.isOnNavMesh) // เช็คก่อนเพื่อป้องกัน error
        {
            agent.isStopped = true;
        }

        // 2. "รอ" เป็นระยะเวลาสั้นๆ ตามที่เราตั้งค่าไว้
        yield return new WaitForSeconds(hitStunDuration);

        // 3. สั่งให้บอสกลับมาเคลื่อนที่ต่อ (ถ้ามันยังมีชีวิตอยู่)
        if (this.enabled) // เช็คว่า AI Controller ยังทำงานอยู่หรือไม่
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
    }
}