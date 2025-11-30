using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private readonly BossAIController _controller;
    private readonly BossCombat _combat;

    private readonly float _rotationSpeed = 5f; 

    public AttackState(BossAIController controller, BossCombat combat)
    {
        _controller = controller;
        _combat = combat;
    }

    public void Enter()
    {
        _controller.agent.isStopped = true;
        _combat.PerformAttack();
    }

    public void Update()
    {

        Vector3 directionToPlayer = (_controller.playerTarget.position - _controller.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        _controller.transform.rotation = Quaternion.Slerp(
            _controller.transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime);

        if (!_combat.isAttacking)
        {
            _controller.SwitchState(_controller.cooldownState);
        }
    }

    public void Exit() { }
}
