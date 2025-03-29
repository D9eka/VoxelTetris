using System;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public InputManager InputManager => _inputManager;
    public CameraController CameraController => _cameraController;
    public FiguresController FiguresController => _figuresController;
    public GridController GridController => _gridController;
    
    public static ServiceLocator Instance { get; private set; }
    
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private CameraController _cameraController;
    [Space]
    [SerializeField] private FiguresController _figuresController;
    [SerializeField] private GridController _gridController;

    private void Awake()
    {
        Instance = this;
    }
}