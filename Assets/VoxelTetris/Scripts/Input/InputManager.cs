using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputActions _inputActions;
    
    public Action<Vector2> PlayerMoveFigure;
    public Action<Vector3> PlayerRotateFigure;
    public Action PlayerDropFigure;

    public Action<float> PlayerRotateCamera;
    
    public Action PlayerPause;
    public Action UIResume;

    public void EnablePlayerInput()
    {
        _inputActions.UI.Disable();
        _inputActions.Player.Enable();
    }

    public void DisablePlayerInput()
    {
        _inputActions.Player.Disable();
        _inputActions.UI.Enable();
    }
    
    private void Awake()
    {
        _inputActions = new InputActions();

        _inputActions.Player.MoveFigure.performed += OnMoveFigurePerformed;
        _inputActions.Player.DropFigure.performed += OnDropFigurePerformed;
        _inputActions.Player.RotateFigureX.performed += OnRotateFigureXPerformed;
        _inputActions.Player.RotateFigureY.performed += OnRotateFigureYPerformed;
        _inputActions.Player.RotateFigureZ.performed += OnRotateFigureZPerformed;
        
        _inputActions.Player.RotateCamera.performed += OnPlayerRotateCameraPerformed;
        
        _inputActions.Player.Pause.performed += OnPausePerformed;
        _inputActions.UI.Resume.performed += OnUIResumePerformed;
    }

    private void Start()
    {
        DisablePlayerInput();
        
        ServiceLocator.Instance.GridController.OnReachLimit += OnReachLimit;
        
        ServiceLocator.Instance.LevelController.StartGame += OnStartGame;
        ServiceLocator.Instance.LevelController.PlayerPause += OnPlayerPause;
        ServiceLocator.Instance.LevelController.UIResume += OnUIResume;
        ServiceLocator.Instance.LevelController.EndGame += OnEndGame;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.GridController.OnReachLimit -= OnReachLimit;

        _inputActions.Player.MoveFigure.performed -= OnMoveFigurePerformed;
        _inputActions.Player.DropFigure.performed -= OnDropFigurePerformed;
        _inputActions.Player.RotateFigureX.performed -= OnRotateFigureXPerformed;
        _inputActions.Player.RotateFigureY.performed -= OnRotateFigureYPerformed;
        _inputActions.Player.RotateFigureZ.performed -= OnRotateFigureZPerformed;
        
        _inputActions.Player.RotateCamera.performed -= OnPlayerRotateCameraPerformed;
        
        _inputActions.Player.Pause.performed -= OnPausePerformed;
        _inputActions.UI.Resume.performed -= OnUIResumePerformed;
        
        ServiceLocator.Instance.LevelController.StartGame -= OnStartGame;
        ServiceLocator.Instance.LevelController.PlayerPause -= OnPlayerPause;
        ServiceLocator.Instance.LevelController.UIResume -= OnUIResume;
        ServiceLocator.Instance.LevelController.EndGame -= OnEndGame;
    }
    
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) 
            return false;

        Vector2 screenPos = Pointer.current != null
            ? Pointer.current.position.ReadValue()
            : Mouse.current.position.ReadValue();

        var pointerData = new PointerEventData(EventSystem.current) {
            position = screenPos
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }

    private void OnMoveFigurePerformed(InputAction.CallbackContext context)
    {
        PlayerMoveFigure?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnDropFigurePerformed(InputAction.CallbackContext context)
    {
        PlayerDropFigure?.Invoke();
    }

    private void OnRotateFigureXPerformed(InputAction.CallbackContext context)
    {
        PlayerRotateFigure?.Invoke(new Vector3(1, 0, 0));
    }

    private void OnRotateFigureYPerformed(InputAction.CallbackContext context)
    {
        PlayerRotateFigure?.Invoke(new Vector3(0, 1, 0));
    }

    private void OnRotateFigureZPerformed(InputAction.CallbackContext context)
    {
        PlayerRotateFigure?.Invoke(new Vector3(0, 0, 1));
    }

    private void OnPlayerRotateCameraPerformed(InputAction.CallbackContext context)
    {
        if (IsPointerOverUI())
        {
            return;
        }

        PlayerRotateCamera.Invoke(context.ReadValue<float>());
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        PlayerPause?.Invoke();
    }

    private void OnUIResumePerformed(InputAction.CallbackContext context)
    {
        UIResume?.Invoke();
    }

    private void OnReachLimit()
    {
        DisablePlayerInput();
    }

    private void OnStartGame()
    {
        EnablePlayerInput();
    }

    private void OnPlayerPause()
    {
        DisablePlayerInput();
        _inputActions.UI.Enable();
    }

    private void OnUIResume()
    {
        _inputActions.UI.Disable();
        EnablePlayerInput();
    }

    private void OnEndGame()
    {
        DisablePlayerInput();
    }
}
