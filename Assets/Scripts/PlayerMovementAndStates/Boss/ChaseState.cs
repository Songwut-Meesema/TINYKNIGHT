using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private readonly BossAIController _controller;

    public ChaseState(BossAIController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        if (_controller.OnBossAggro != null)
        {
            _controller.OnBossAggro.Raise();
        }

        _controller.agent.isStopped = false;
        _controller.agent.speed = _controller.CurrentChaseSpeed;
        _controller.animator.SetFloat("Speed", 1);
    }

    public void Update()
    {
        _controller.agent.SetDestination(_controller.playerTarget.position);
        float distanceToPlayer = Vector3.Distance(_controller.transform.position, _controller.playerTarget.position);
        if (distanceToPlayer <= _controller.attackRange)
        {
            _controller.SwitchState(_controller.attackState);
        }
        else if (distanceToPlayer > _controller.aggroRange * 1.5f)
        {
            _controller.SwitchState(_controller.idleState);
        }
    }

    public void Exit()
    {
        _controller.agent.isStopped = true;
    }
}