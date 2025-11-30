using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private readonly BossAIController _controller;

    public IdleState(BossAIController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _controller.agent.isStopped = true;
         _controller.animator.SetFloat("Speed", 0);
    }

    public void Update()
    {
        float distanceToPlayer = Vector3.Distance(_controller.transform.position, _controller.playerTarget.position);

        if (distanceToPlayer <= _controller.aggroRange)
        {
            _controller.SwitchState(_controller.chaseState);
        }
    }

    public void Exit() { }
}

