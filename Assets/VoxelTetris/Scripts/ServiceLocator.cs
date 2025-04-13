using System;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public LevelController LevelController => _levelController;
    public InputManager InputManager => _inputManager;
    public CameraController CameraController => _cameraController;
    public UIController UIController => _uiController;
    public FiguresController FiguresController => _figuresController;
    public GridController GridController => _gridController;
    public AbilityManager AbilityManager => _abilityManager;
    
    public static ServiceLocator Instance { get; private set; }
    
    [SerializeField] private LevelController _levelController;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private UIController _uiController;
    [Space]
    [SerializeField] private FiguresController _figuresController;
    [SerializeField] private GridController _gridController;
    [Space]
    [SerializeField] private AbilityManager _abilityManager;

    private void Awake()
    {
        Instance = this;
    }
}