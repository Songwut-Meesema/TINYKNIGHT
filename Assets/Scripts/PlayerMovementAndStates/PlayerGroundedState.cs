using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : IPlayerState
{
    private readonly PlayerController _playerController;

    // --- [อัปเกรด] ---
    // Cache ค่า Stamina cost ไว้ใน Constructor เพื่อประสิทธิภาพที่ดีขึ้น
    // จะได้ไม่ต้องไปดึงค่าจาก ScriptableObject ทุกครั้งที่กดปุ่ม
    private readonly float _lightAttackStaminaCost;
    private readonly float _heavyAttackStaminaCost;
    private readonly float _dodgeStaminaCost;

    public PlayerGroundedState(PlayerController playerController)
    {
        _playerController = playerController;
        // ดึงค่ามาเก็บไว้ที่นี่แค่ครั้งเดียว
        _lightAttackStaminaCost = playerController.PlayerStatus.baseStats.lightAttackStaminaCost;
        _heavyAttackStaminaCost = playerController.PlayerStatus.baseStats.heavyAttackStaminaCost;
        _dodgeStaminaCost = playerController.PlayerStatus.baseStats.dodgeStaminaCost;
    }

    public void Enter() { }

    public void Update()
    {
        // --- [อัปเกรด] ---
        // ใช้โค้ดการเคลื่อนไหวและการอัปเดต Animator ที่มีการหน่วง (smoothing)
        // เพื่อให้การเปลี่ยนจากท่าหยุดเป็นท่าเดินดูนุ่มนวลขึ้น
        _playerController.MoveCharacter();
        _playerController.GetAnimator().SetFloat(_playerController.SpeedHash, _playerController.MoveInput.magnitude, 0.1f, Time.deltaTime);
    }

    public void Exit()
    {
        _playerController.GetAnimator().SetFloat(_playerController.SpeedHash, 0f);
    }

    public void OnJump()
    {
        Debug.Log("Jump pressed, but JumpingState is not implemented yet.");
    }

    public void OnDash()
    {
        if (_playerController.PlayerStatus.HasEnoughStamina(_dodgeStaminaCost))
        {
            _playerController.SwitchState(_playerController.DodgeState);
        }
        else
        {
            // --- [THE FIX] ---
            _playerController.PlayerStatus.PlayNoStaminaSound();
        }
    }

    public void OnLightAttack()
    {
        if (_playerController.PlayerStatus.HasEnoughStamina(_lightAttackStaminaCost))
        {
            _playerController.AttackState.SetAttackType(false, 0.7f, 0.8f);
            _playerController.SwitchState(_playerController.AttackState);
        }
        else
        {
            // --- [THE FIX] ---
            _playerController.PlayerStatus.PlayNoStaminaSound();
        }
    }

    public void OnHeavyAttack()
    {
        if (_playerController.PlayerStatus.HasEnoughStamina(_heavyAttackStaminaCost))
        {
            _playerController.AttackState.SetAttackType(true, 0.7f, 0.8f);
            _playerController.SwitchState(_playerController.AttackState);
        }
        else
        {
            // --- [THE FIX] ---
            _playerController.PlayerStatus.PlayNoStaminaSound();
        }
    }
}