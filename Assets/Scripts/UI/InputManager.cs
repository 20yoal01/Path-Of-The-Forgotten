using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField]
    public GameObject pauseMenu;

    PauseMenu pauseMenuManager;

    public static PlayerInput PlayerInput;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        pauseMenuManager = pauseMenu.GetComponent<PauseMenu>();

        // Initialize playerInput when the script is loaded
        if (PlayerInput == null)
        {
            PlayerInput = GetComponent<PlayerInput>();
        }
    }
    public void togglePauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenuManager.Resume();
            }
            else
            {
                pauseMenuManager.Pause();
            }
        }
    }

    public void ActivatePlayerControls()
    {
        PlayerInput.currentActionMap.Enable();
    }
    public void DeactivatePlayerControls()
    {
        PlayerInput.currentActionMap.Disable();
    }

    public void SwitchToBowActionMap()
    {
        PlayerInput.SwitchCurrentActionMap("Bow");
    }

    public void SwitchToPlayerActionMap()
    {
        PlayerInput.SwitchCurrentActionMap("Player");
    }

    public InputActionMap GetCurrentActionMap()
    {
        return PlayerInput.currentActionMap;
    }

    public void SaveGame(InputAction.CallbackContext context)
    {
        if (!_isSaving)
            SaveAsync();
    }

    public void LoadGame(InputAction.CallbackContext context)
    {
        if (!_isLoading)
            LoadAsync();
    }

    private bool _isSaving;
    private bool _isLoading;

    public async void SaveAsync()
    {
        _isSaving = true;
        await SaveSystem.SaveAsynchronously();
        _isSaving = false;
    }

    public async void LoadAsync()
    {
        _isLoading = true;
        await SaveSystem.LoadAync();
        _isLoading = false;
    }
}


