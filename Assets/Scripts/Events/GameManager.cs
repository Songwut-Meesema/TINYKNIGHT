using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerInput _playerInput;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        FindAndSetupPlayerInput();
        EnterGameplayMode();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeFromPauseEvent();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndSetupPlayerInput();
        EnterGameplayMode();
        Time.timeScale = 1f;
    }

    private void FindAndSetupPlayerInput()
    {
        UnsubscribeFromPauseEvent();
        _playerInput = FindObjectOfType<PlayerInput>();
        SubscribeToPauseEvent();
    }

    private void SubscribeToPauseEvent()
    {
        if (_playerInput != null)
        {
            // --- [THE FIX] ---
            // LEAD COMMENT: เราจะเปิดใช้งาน Action Map "Global" อย่างถาวร
            // และจะดึง Action "Pause" มาจาก Map นี้เท่านั้น
            _playerInput.actions.FindActionMap("Global").Enable();
            _playerInput.actions["Pause"].performed += OnPause;
            Debug.Log("GameManager subscribed to Pause action from GLOBAL map.");
        }
        else { Debug.LogError("GameManager could not find a PlayerInput component!"); }
    }

    private void UnsubscribeFromPauseEvent()
    {
        if (_playerInput != null)
        {
            _playerInput.actions["Pause"].performed -= OnPause;
        }
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (GameUIManager.Instance == null) { return; }
        GameUIManager.Instance.TogglePauseScreen();
    }

    public void TogglePauseState(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            EnterUIMode();
        }
        else
        {
            Time.timeScale = 1f;
            EnterGameplayMode();
        }
    }

    public void EnterUIMode()
    {
        if (_playerInput == null) return;
        _playerInput.SwitchCurrentActionMap("UI");
        if (InputManager.Instance != null) InputManager.Instance.EnableLightClickAction(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnterGameplayMode()
    {
        if (_playerInput == null) return;
        _playerInput.SwitchCurrentActionMap("PlayerControls");
        if (InputManager.Instance != null) InputManager.Instance.EnableLightClickAction(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}