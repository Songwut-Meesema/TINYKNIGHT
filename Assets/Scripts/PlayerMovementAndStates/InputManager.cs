using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerController _playerController;
    private PlayerInput _playerInput;
    private InputAction _lightClickAction;

    // --- [เพิ่มใหม่] ---
    // LEAD COMMENT: เราจะสร้าง Singleton สำหรับ InputManager ด้วย
    // เพื่อให้ GameManager สามารถเข้าถึง "สวิตช์" ของเราได้อย่างง่ายดาย
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        // --- [เพิ่มใหม่] ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _playerController = GetComponent<PlayerController>();
        _playerInput = GetComponent<PlayerInput>();
        _lightClickAction = new InputAction(binding: "<Mouse>/leftButton");
        _lightClickAction.performed += OnLightAttackPerformed;
    }

    // --- [เพิ่มใหม่] ---
    // LEAD COMMENT: สร้าง "สวิตช์" Public สำหรับเปิด/ปิด "สายลับ" ของเรา
    public void EnableLightClickAction(bool enable)
    {
        if (enable)
        {
            _lightClickAction.Enable();
            Debug.Log("Direct Light Click Action ENABLED");
        }
        else
        {
            _lightClickAction.Disable();
            Debug.Log("Direct Light Click Action DISABLED");
        }
    }

    private void OnEnable()
    {
        // เปิดใช้งาน "สายลับ" เป็นค่าเริ่มต้น
        EnableLightClickAction(true);

        _playerInput.actions["HeavyAttack"].performed += OnHeavyAttack;
        _playerInput.actions["Move"].performed += OnMove;
        _playerInput.actions["Move"].canceled += OnMove;
        _playerInput.actions["Jump"].performed += OnJump;
        _playerInput.actions["Dash"].performed += OnDash;
    }

    private void OnDisable()
    {
        // ปิดใช้งาน "สายลับ" เสมอเมื่อออกจาก Scene
        EnableLightClickAction(false);

        _playerInput.actions["HeavyAttack"].performed -= OnHeavyAttack;
        _playerInput.actions["Move"].performed -= OnMove;
        _playerInput.actions["Move"].canceled -= OnMove;
        _playerInput.actions["Jump"].performed -= OnJump;
        _playerInput.actions["Dash"].performed -= OnDash;
    }

    private void OnLightAttackPerformed(InputAction.CallbackContext context)
    {
        _playerController.OnLightAttack(context);
    }

    private void OnMove(InputAction.CallbackContext context) { _playerController.OnMove(context); }
    private void OnJump(InputAction.CallbackContext context) { _playerController.OnJump(context); }
    private void OnHeavyAttack(InputAction.CallbackContext context) { _playerController.OnHeavyAttack(context); }
    private void OnDash(InputAction.CallbackContext context) { _playerController.OnDash(context); }
}