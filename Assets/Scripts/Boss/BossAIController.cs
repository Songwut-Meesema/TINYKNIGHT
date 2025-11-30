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

    [Header("Game Feel Settings")]
    public float hitStunDuration = 0.15f;
    private Coroutine _hitStunCoroutine;

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

    public void TriggerHitStun()
    {
        if (_hitStunCoroutine != null)
        {
            StopCoroutine(_hitStunCoroutine);
        }
        _hitStunCoroutine = StartCoroutine(HitStunCoroutine());
    }

    private IEnumerator HitStunCoroutine()
    {
        {
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(hitStunDuration);

        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }
    }
}
