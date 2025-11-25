using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private readonly PlayerController _playerController;
    private float _attackDuration;
    private float _stateTimer;
    private bool _isHeavyAttack;
    private float _attackSearchRadius = 5f;
    private LayerMask _enemyLayer;

    public void SetAttackType(bool isHeavy, float lightAttackAnimTime, float heavyAttackAnimTime)
    {
        _isHeavyAttack = isHeavy;
        _attackDuration = isHeavy ? heavyAttackAnimTime : lightAttackAnimTime;
    }

    public PlayerAttackState(PlayerController playerController)
    {
        _playerController = playerController;
        _enemyLayer = LayerMask.GetMask("Enemy");
    }

    public void Enter()
    {
        _playerController.PlayerStatus.PlaySwingSound();

        RotateTowardsClosestEnemy();

        float staminaCost = _isHeavyAttack ?
            _playerController.PlayerStatus.runtimeStats.heavyAttackStaminaCost :
            _playerController.PlayerStatus.runtimeStats.lightAttackStaminaCost;
        _playerController.PlayerStatus.UseStamina(staminaCost);

        _stateTimer = _attackDuration;

        if (_isHeavyAttack)
            _playerController.GetAnimator().SetTrigger(_playerController.HeavyAttackHash);
        else
            _playerController.GetAnimator().SetTrigger(_playerController.LightAttackHash);
    }

    public void Update()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f)
        {
            _playerController.SwitchState(_playerController.GroundedState);
        }
    }

    public void Exit() { }

    private void RotateTowardsClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(
            _playerController.transform.position,
            _attackSearchRadius,
            _enemyLayer
        );

        if (enemies.Length > 0)
        {
            Transform closestEnemy = enemies[0].transform;
            Vector3 directionToEnemy = (closestEnemy.position - _playerController.transform.position).normalized;
            _playerController.transform.rotation = Quaternion.LookRotation(new Vector3(directionToEnemy.x, 0, directionToEnemy.z));
        }
    }

    public void OnJump() { }
    public void OnDash() { }
    public void OnLightAttack() { }
    public void OnHeavyAttack() { }
}