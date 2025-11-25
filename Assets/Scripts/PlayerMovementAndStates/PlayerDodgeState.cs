using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeState : IPlayerState
{
    private readonly PlayerController _playerController;
    private float _dodgeDuration = 0.7f;
    private float _dodgeSpeed = 8f;
    private float _stateTimer;
    private Vector3 _dodgeDirection;
    private int _playerLayer;
    private int _enemyLayer;

    public PlayerDodgeState(PlayerController playerController)
    {
        _playerController = playerController;
        _playerLayer = LayerMask.NameToLayer("Player");
        _enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    public void Enter()
    {
        // --- [THE FIX] ---
        _playerController.PlayerStatus.PlayDodgeSound();

        Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, true);

        float staminaCost = _playerController.PlayerStatus.baseStats.dodgeStaminaCost;
        _playerController.PlayerStatus.UseStamina(staminaCost);

        _stateTimer = _dodgeDuration;
        _playerController.GetAnimator().SetTrigger(_playerController.DodgeHash);

        if (_playerController.MoveInput.sqrMagnitude > 0.01f)
        {
            Vector2 moveInput = _playerController.MoveInput;
            _dodgeDirection = new Vector3(moveInput.x, 0, moveInput.y);
            Transform cameraTransform = Camera.main.transform;
            _dodgeDirection = cameraTransform.forward * _dodgeDirection.z + cameraTransform.right * _dodgeDirection.x;
            _dodgeDirection.y = 0;
            _dodgeDirection.Normalize();
        }
        else
        {
            _dodgeDirection = _playerController.transform.forward;
        }
    }

    public void Update()
    {
        _playerController.GetComponent<CharacterController>().Move(_dodgeDirection * _dodgeSpeed * Time.deltaTime);

        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f)
        {
            _playerController.SwitchState(_playerController.GroundedState);
        }
    }

    public void Exit()
    {
        Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
    }

    public void OnJump() { }
    public void OnDash() { }
    public void OnLightAttack() { }
    public void OnHeavyAttack() { }
}