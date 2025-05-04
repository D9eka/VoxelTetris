using System.Collections.Generic;
using UnityEngine;

public class FiguresController : MonoBehaviour
{
    public bool Active { get; private set; }
    
    [SerializeField] private FiguresControllerData _data;
    
    private FigureSpawner _figureSpawner;

    private List<FigureController> _figuresToMove;
    private FigureController _activeFigure;

    private float _timeToDropFigure;
    private float _timeFromLastMove;

    private void Awake()
    {
        _figuresToMove = new();
        _timeToDropFigure = _data.TimeToDropFigure;
    }

    private void Start()
    {
        GridModel gridModel = ServiceLocator.Instance.GridController.Model;
        Vector3Int spawnPosition = Vector3Int.RoundToInt(
            new Vector3(gridModel.Width / 2f, gridModel.Height - _data.SpawnOffsetY, gridModel.Depth / 2f));
        FigureCube figureCube = _data.FigureCubes[Random.Range(0, _data.FigureCubes.Length)];
        _figureSpawner = new FigureSpawner(_data.FigurePrefabs, spawnPosition, 
            ServiceLocator.Instance.GridController.transform, figureCube, _data.FigureColors);
        
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility += OnEndSlowDropAbility;
        
        ServiceLocator.Instance.GridController.OnReachLimit += OnReachLimit;
        
        ServiceLocator.Instance.InputManager.PlayerMoveFigure += OnPlayerMoveFigure;
        ServiceLocator.Instance.InputManager.PlayerRotateFigure += OnPlayerRotateFigure;
        ServiceLocator.Instance.InputManager.PlayerDropFigure += OnPlayerDropFigure;
        
        ServiceLocator.Instance.LevelController.StartGame += OnStartGame;
        ServiceLocator.Instance.LevelController.PlayerPause += OnPlayerPause;
        ServiceLocator.Instance.LevelController.UIResume += OnUIResume;
        ServiceLocator.Instance.LevelController.EndGame += OnEndGame;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility -= OnEndSlowDropAbility;
        
        ServiceLocator.Instance.GridController.OnReachLimit -= OnReachLimit;
        
        ServiceLocator.Instance.InputManager.PlayerMoveFigure -= OnPlayerMoveFigure;
        ServiceLocator.Instance.InputManager.PlayerRotateFigure -= OnPlayerRotateFigure;
        ServiceLocator.Instance.InputManager.PlayerDropFigure -= OnPlayerDropFigure;
        
        ServiceLocator.Instance.LevelController.StartGame -= OnStartGame;
        ServiceLocator.Instance.LevelController.PlayerPause -= OnPlayerPause;
        ServiceLocator.Instance.LevelController.UIResume -= OnUIResume;
        ServiceLocator.Instance.LevelController.EndGame -= OnEndGame;
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
        Active = true;
    }
    
    [ContextMenu("StopSpawning")]
    public void StopSpawning()
    {
        Active = false;
    }

    public void Clear()
    {
        _figuresToMove = new List<FigureController>();
        _activeFigure = null;
    }

    public void AddFigures(IEnumerable<FigureController> figures)
    {
        foreach (FigureController figure in figures)
        {
            AddFigure(figure);
        }
    }

    public void AddFigure(FigureController figure, bool setActive = false)
    {
        _figuresToMove.Add(figure);
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
    
        Vector3Int directionInt = Vector3Int.down;
        while (ServiceLocator.Instance.GridController.TryMoveFigure(_activeFigure.Model, directionInt))
        {
            _activeFigure.Move(directionInt);
        }
    
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
                partController.transform.position = partController.Model.Position;
            }
        }
    }
    
    public void RemoveFigures(IEnumerable<FigureController> figures)
    {
        foreach (var figure in figures)
        {
            if (figure == _activeFigure)
            {
                _activeFigure = null;
            }
            _figuresToMove.Remove(figure);
        
            if (figure != null && figure.gameObject != null)
            {
                Destroy(figure.gameObject);
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

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _timeToDropFigure *= timeModifier;
    }

    private void OnEndSlowDropAbility()
    {
        _timeToDropFigure = _data.TimeToDropFigure;
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
        bool canMove = ServiceLocator.Instance.GridController.TryMoveFigure(figure.Model, directionInt);
    
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