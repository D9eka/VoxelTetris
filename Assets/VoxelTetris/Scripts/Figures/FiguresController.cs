using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FiguresController : MonoBehaviour
{
    public bool Active { get; private set; }
    
    [SerializeField] private FiguresControllerData _data;
    
    private FigureSpawner _figureSpawner;

    private List<FigureController> _figuresToMove;
    private FigureController _activeFigure;
    
    private float _defaultTimeToDropFigure;
    private float _timeToDropFigure;
    private float _timeFromLastMove;
    
    private AbilityManager _abilityManager;
    private GridController _gridController;
    private InputManager _inputManager;
    private LevelController _levelController;
    private ScoreManager _scoreManager;

    private void Awake()
    {
        _figuresToMove = new();
        _defaultTimeToDropFigure = _data.LowTimeToDropFigure;
    }

    private void Start()
    {
        _abilityManager = ServiceLocator.Instance.AbilityManager;
        _gridController = ServiceLocator.Instance.GridController;
        _inputManager = ServiceLocator.Instance.InputManager;
        _levelController = ServiceLocator.Instance.LevelController;
        _scoreManager = ServiceLocator.Instance.ScoreManager;
        
        _timeToDropFigure = _defaultTimeToDropFigure;
        
        GridModel gridModel = _gridController.Model;
        Vector3Int spawnPosition = Vector3Int.RoundToInt(
            new Vector3(gridModel.Width / 2f, gridModel.Height - _data.SpawnOffsetY, gridModel.Depth / 2f));
        FigureCube figureCube = _data.FigureCubes[Random.Range(0, _data.FigureCubes.Length)];
        _figureSpawner = new FigureSpawner(_data.FigurePrefabs, spawnPosition, 
            _gridController.transform, figureCube, _data.FigureColors);
        
        _abilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        _abilityManager.OnEndSlowDropAbility += OnEndSlowDropAbility;
        
        _gridController.OnReachLimit += OnReachLimit;
        
        _inputManager.PlayerMoveFigure += OnPlayerMoveFigure;
        _inputManager.PlayerRotateFigure += OnPlayerRotateFigure;
        _inputManager.PlayerDropFigure += OnPlayerDropFigure;
        
        _levelController.StartGame += OnStartGame;
        _levelController.PlayerPause += OnPlayerPause;
        _levelController.UIResume += OnUIResume;
        _levelController.EndGame += OnEndGame;
        
        _scoreManager.OnScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        _abilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        _abilityManager.OnEndSlowDropAbility -= OnEndSlowDropAbility;
        
        _gridController.OnReachLimit -= OnReachLimit;
        
        _inputManager.PlayerMoveFigure -= OnPlayerMoveFigure;
        _inputManager.PlayerRotateFigure -= OnPlayerRotateFigure;
        _inputManager.PlayerDropFigure -= OnPlayerDropFigure;
        
        _levelController.StartGame -= OnStartGame;
        _levelController.PlayerPause -= OnPlayerPause;
        _levelController.UIResume -= OnUIResume;
        _levelController.EndGame -= OnEndGame;
    }

    private void Update()
    {
        if (!Active)
        {
            return;
        }
        
        if (_activeFigure is null)
        {
            FigureController newActiveFigure = _figureSpawner.SpawnFigure();
            AddFigure(newActiveFigure, true);
        }

        MoveFigures();
    }

    [ContextMenu("StartSpawning")]
    public void StartSpawning()
    {
        Debug.Log("StartSpawning");
        Active = true;
    }
    
    [ContextMenu("StopSpawning")]
    public void StopSpawning()
    {
        Debug.Log("StopSpawning");
        Active = false;
    }

    public void Clear()
    {
        _figuresToMove = new List<FigureController>();
        _activeFigure = null;
    }

    public void MoveFiguresToBottom(IEnumerable<FigureController> figures)
    {
        foreach (FigureController figure in figures)
        {
            if (figure == _activeFigure)
            {
                continue;
            }
            MoveToBottom(figure);
        }
    }

    public void AddFigure(FigureController figure, bool setActive = false)
    {
        _figuresToMove.Add(figure);
        figure.IsMoving = true;
        if (setActive)
        {
            _activeFigure = figure;
        }
    }

    public void MoveToBottom()
    {
        if (_activeFigure == null)
        {
            return;
        }

        _figuresToMove.Remove(_activeFigure);
        MoveToBottom(_activeFigure);
        _activeFigure = null;
    }

    public void Move(Vector2 input)
    {
        float cameraAngle = CameraController.Instance.Angle;
        float angleRad = -cameraAngle * Mathf.Deg2Rad;
    
        float x = input.x;
        float z = input.y;
    
        float rotatedX = x * Mathf.Cos(angleRad) + z * Mathf.Sin(angleRad);
        float rotatedZ = -x * Mathf.Sin(angleRad) + z * Mathf.Cos(angleRad);
    
        Vector3Int directionInt = new Vector3Int(
            Mathf.RoundToInt(rotatedX),
            0,
            Mathf.RoundToInt(rotatedZ)
        );
    
        TryMove(_activeFigure, directionInt);
    }

    public void Rotate(Vector3 axis)
    {
        Quaternion rotation = Quaternion.Euler(axis * 90f);
        Vector3 centerPosition = _activeFigure.Model.Center.Position;

        List<Vector3Int> newPositions = new List<Vector3Int>();
        foreach (FigurePartModel part in _activeFigure.Model.Parts)
        {
            Vector3 relativePosition = part.Position - centerPosition;
            Vector3 rotatedPosition = rotation * relativePosition;
            Vector3Int newPosition = Vector3Int.RoundToInt(rotatedPosition + centerPosition);
            newPositions.Add(newPosition);
        }

        GridController gridController = ServiceLocator.Instance.GridController;
        if (gridController.CanPlaceFigureAtPositions(_activeFigure.Model, newPositions))
        {
            List<Vector3Int> oldPositions = new List<Vector3Int>();
            foreach (FigurePartModel part in _activeFigure.Model.Parts)
            {
                oldPositions.Add(part.Position);
            }

            gridController.UpdateFigurePositions(_activeFigure.Model, oldPositions, newPositions);

            foreach (FigurePartController partController in _activeFigure.Parts)
            {
                partController.transform.DOMove(partController.Model.Position, 0.3f);
            }
        }
    }

    public void RemoveFiguresPartAtPlane(IEnumerable<FigureController> figures, int planePosY)
    {
        foreach (FigureController figure in figures)
        {
            List<FigurePartModel> figureParts = figure.Model.Parts;
            for (int i = figureParts.Count - 1; i >= 0; i--)
            {
                if (figureParts[i].Position.y == planePosY)
                {
                    figure.DeleteFigurePart(figureParts[i].Controller);
                }
            }
        }
    }

    private void MoveToBottom(FigureController figure)
    {
        if (figure == null)
        {
            return;
        }
        Vector3Int directionInt = Vector3Int.down;
        
        while (_gridController.TryMoveFigure(figure.Model, directionInt))
        {
            figure.Move(directionInt);
        }
        figure.IsMoving = false;
    }

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _timeToDropFigure *= timeModifier;
    }

    private void OnEndSlowDropAbility()
    {
        _timeToDropFigure = _defaultTimeToDropFigure;
    }
    
    private void OnPlayerMoveFigure(Vector2 input)
    {
        Move(input);
    }

    private void OnPlayerRotateFigure(Vector3 axis)
    {
        Rotate(axis);
    }

    private void OnPlayerDropFigure()
    {
        MoveToBottom();
    }

    private void OnStartGame()
    {
        StartSpawning();
    }

    private void OnPlayerPause()
    {
        StopSpawning();
    }

    private void OnUIResume()
    {
        StartSpawning();
    }

    private void OnEndGame()
    {
        StopSpawning();
        Clear();
    }

    private void OnReachLimit()
    {
        StopSpawning();
        Debug.Log("Reach Limit");
    }
    
    private void OnScoreChanged(int score)
    {
        if (score <= _data.ScoreToMediumTimeToDropFigure)
        {
            _defaultTimeToDropFigure = _data.LowTimeToDropFigure;
        }
        if (score <= _data.ScoreToHighTimeToDropFigure)
        {
            _defaultTimeToDropFigure = _data.MediumTimeToDropFigure;
        }
        if (score > _data.ScoreToHighTimeToDropFigure)
        {
            _defaultTimeToDropFigure = _data.HighTimeToDropFigure;
        }
    }

    private void MoveFigures()
    {
        if (_timeFromLastMove < _timeToDropFigure)
        {
            _timeFromLastMove += Time.deltaTime;
            return;
        }

        var figuresToProcess = new List<FigureController>(_figuresToMove);
    
        foreach (var figure in figuresToProcess)
        {
            if (figure == null || figure.Model == null)
            {
                _figuresToMove.Remove(figure);
                continue;
            }

            bool canMove = TryMove(figure, Vector3Int.down);
            if (!canMove)
            {
                figure.IsMoving = false;
                if (figure == _activeFigure)
                {
                    _activeFigure = null;
                }
                _figuresToMove.Remove(figure);
            }
        }
    
        _timeFromLastMove = 0;
    }
    
    private bool TryMove(FigureController figure, Vector3Int directionInt)
    {
        if (figure == null)
        {
            return false;
        }

        bool wasMoved = false;
        bool canMove = _gridController.TryMoveFigure(figure.Model, directionInt);
    
        if (canMove)
        {
            figure.Move(directionInt);
            wasMoved = true;
        }
        else if (directionInt == Vector3Int.down)
        {
            _figuresToMove.Remove(figure);
            if (figure == _activeFigure)
            {
                _activeFigure = null;
            }
        }
    
        return wasMoved;
    }
}