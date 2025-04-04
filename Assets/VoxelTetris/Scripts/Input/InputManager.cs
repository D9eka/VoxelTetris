using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputActions _inputActions;

    public Action<float> OnRotateCamera;

    public void EnableInput()
    {
        _inputActions.Player.Enable();
    }

    public void DisableInput()
    {
        _inputActions.Player.Disable();
    }
    
    private void Awake()
    {
        _inputActions = new InputActions();

        _inputActions.Player.MoveFigure.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => ServiceLocator.Instance.FiguresController.Move(context.ReadValue<Vector2>());
        _inputActions.Player.DropFigure.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => ServiceLocator.Instance.FiguresController.MoveToBottom();

        _inputActions.Player.RotateFigureX.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => ServiceLocator.Instance.FiguresController.Rotate(new Vector3(1, 0, 0));
        _inputActions.Player.RotateFigureY.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => ServiceLocator.Instance.FiguresController.Rotate(new Vector3(0, 1, 0));
        _inputActions.Player.RotateFigureZ.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => ServiceLocator.Instance.FiguresController.Rotate(new Vector3(0, 0, 1));
        _inputActions.Player.RotateCamera.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => OnRotateCamera?.Invoke(context.ReadValue<float>());
    }

    /*
    public Action PlayerMoveFigure;
    public Action PlayerRotateFigureX;
    public Action PlayerRotateFigureY;
    public Action PlayerRotateFigureZ;
    public Action PlayerDropFigure;

    public Action PlayerRotateCamera;
    public Action PlayerZoomCamera;

    public Action PlayerPause;

    public Action UIInteract;
    public Action UIResume;
    */
}
