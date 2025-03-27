using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputActions _inputActions;

    public static InputManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;

        _inputActions = new InputActions();

        _inputActions.Player.MoveFigure.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => FiguresController.Instance.Move(context.ReadValue<Vector2>());
        _inputActions.Player.DropFigure.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => FiguresController.Instance.MoveToBottom();

        _inputActions.Player.RotateFigureX.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => FiguresController.Instance.Rotate(new Vector3(1, 0, 0));
        _inputActions.Player.RotateFigureY.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => FiguresController.Instance.Rotate(new Vector3(0, 1, 0));
        _inputActions.Player.RotateFigureZ.performed += (UnityEngine.InputSystem.InputAction.CallbackContext context)
            => FiguresController.Instance.Rotate(new Vector3(0, 0, 1));
        
        _inputActions.Player.Enable();
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
