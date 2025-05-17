using UnityEngine;
using UnityEngine.Serialization;

public class ServiceLocator : MonoBehaviour
{
    public LevelController LevelController => _levelController;
    public InputManager InputManager => _inputManager;
    public CameraController CameraController => _cameraController;
    public UIController UIController => _uiController;
    public FigureController FigureController => figureController;
    public Board Board => board;
    public ScoreManager ScoreManager => _scoreManager;
    public AbilityManager AbilityManager => _abilityManager;
    public SavesManager SavesManager => _savesManager;
    public AudioManager AudioManager => _audioManager;
    public ADManager ADManager => _adManager;
    
    public static ServiceLocator Instance { get; private set; }
    
    [SerializeField] private LevelController _levelController;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private UIController _uiController;
    [FormerlySerializedAs("_figuresController")]
    [Space]
    [SerializeField] private FigureController figureController;
    [FormerlySerializedAs("_gridController")] [SerializeField] private Board board;
    [Space]
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private AbilityManager _abilityManager;
    [Space]
    [SerializeField] private SavesManager _savesManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private ADManager _adManager;

    private void Awake()
    {
        Instance = this;
    }
}