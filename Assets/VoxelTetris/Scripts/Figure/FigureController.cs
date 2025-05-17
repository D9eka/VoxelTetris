using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class FigureController : MonoBehaviour 
{
    [SerializeField] private FigureSpawner _figureSpawner;
    [SerializeField] private FigureLight _figureLight;

    public bool Active { get; private set; }
    public Figure ActiveFigure { get; private set; }
    public Action<FigureType, int> OnPlaceFigure;
    public Action<FigureType, List<int>> OnClearPlanes;

    private float _dropTimer = 0f;
    private AutoDropper _dropper;

    private InputManager _inputManager;
    private Board _board;
    private ScoreManager _scoreManager;

    private bool _isLocking;

    private void Start() 
    {
        _inputManager = ServiceLocator.Instance.InputManager;
        _board = ServiceLocator.Instance.Board;

        LevelController levelController = ServiceLocator.Instance.LevelController;
        levelController.StartGame += StartGame;
        levelController.PlayerPause += StopSpawning;
        levelController.UIResume += StartSpawning;
        levelController.ReachLimit += EndGame;
        levelController.EndGame += EndGame;
        
        AbilityManager abilityManager = ServiceLocator.Instance.AbilityManager;
        abilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        abilityManager.OnEndSlowDropAbility += OnEndSlowDropAbility;
        
        _scoreManager = ServiceLocator.Instance.ScoreManager;
        _dropper = new AutoDropper(_scoreManager);
    }

    public void StartGame()
    {
        Debug.Log("---StartGame");
        StartSpawning();

        if (ActiveFigure != null) 
        {
            Destroy(ActiveFigure.gameObject);
        }
        SpawnNewFigure();
        Debug.Log("StartGame---");
    }

    public void StartSpawning() 
    {
        Debug.Log("---StartSpawning");
        gameObject.SetActive(true);
        Active = true;

        _inputManager.PlayerMoveFigure += Move;
        _inputManager.PlayerRotateFigure += Rotate;
        _inputManager.PlayerDropFigure += HardDrop;

        if (ActiveFigure == null) 
        {
            SpawnNewFigure();
        }
        Debug.Log("StartSpawning---");
    }

    public void StopSpawning() 
    {
        Debug.Log("---StopSpawning");
        _inputManager.PlayerMoveFigure -= Move;
        _inputManager.PlayerRotateFigure -= Rotate;
        _inputManager.PlayerDropFigure -= HardDrop;

        Active = false;
        Debug.Log("StopSpawning---");
    }

    public void EndGame()
    {
        Debug.Log("---EndGame");
        StopSpawning();
        gameObject.SetActive(false);
        foreach (Figure figure in GetComponentsInChildren<Figure>())
        {
            Destroy(figure.gameObject);
        }
        Debug.Log("EndGame---");
    }

    private void Update() 
    {
        if (!Active || _isLocking)
        {
            return;
        }

        _dropTimer += Time.deltaTime;
        if (_dropTimer >= _dropper.GetCurrentDelay()) 
        {
            Drop();
            _dropTimer = 0f;
        }
        
        _figureLight.UpdateLandingLights(ActiveFigure);
    }

    private void SpawnNewFigure() 
    {
        ActiveFigure = _figureSpawner.SpawnFigure();
    }

    private void Drop() 
    {
        if (!Active || ActiveFigure == null || _isLocking)
        {
            return;
        }

        if (CanMove(Vector3Int.down)) 
        {
            Move(Vector3Int.down);
        } 
        else 
        {
            LockFigure(ActiveFigure);
        }
    }

    private void HardDrop() 
    {
        if (!Active || ActiveFigure == null || _isLocking)
        {
            return;
        }

        Figure figure = ActiveFigure;
        _isLocking = true;

        int maxDist = int.MaxValue;
        foreach (Transform cube in figure.Parts) 
        {
            int dist = 0;
            Vector3Int pos = Vector3Int.RoundToInt(cube.position);
            while (_board.IsInside(pos + Vector3Int.down * (dist + 1)) &&
                   !_board.IsOccupied(pos + Vector3Int.down * (dist + 1))) 
            {
                dist++;
            }
            maxDist = Mathf.Min(maxDist, dist);
        }

        Vector3 target = figure.transform.position + Vector3.down * maxDist;
        figure.transform.DOMove(target, 0.1f).SetLink(figure.gameObject).OnComplete(() => 
        {
            LockFigure(figure);
        });
    }
    
    private void LockFigure(Figure figure) 
    {
        if (figure == null)
        {
            _isLocking = false;
            return;
        }

        FigureType figureType = figure.Type;
        int figureCenterPosY = Mathf.RoundToInt(figure.Center.position.y);
        
        _board.PlaceFigure(figure);
        OnPlaceFigure?.Invoke(figureType, figureCenterPosY);
        
        List<int> cleared = _board.ClearFullLayers();
        OnClearPlanes?.Invoke(figureType, cleared);
        
        if (figure == ActiveFigure) 
        {
            Destroy(figure.gameObject);
            ActiveFigure = null;
        } 
        else 
        {
            Destroy(figure.gameObject);
        }

        SpawnNewFigure();
        _isLocking = false;
    }

    public void Rotate(Vector3 axis) 
    {
        if (ActiveFigure != null && CanRotate(axis) && !_isLocking) 
        {
            Quaternion currentRot = ActiveFigure.transform.rotation;
            Quaternion deltaRot = Quaternion.Euler(axis * 90f);
            Quaternion targetRot = deltaRot * currentRot;

            DOTween.Complete(ActiveFigure.transform);

            ActiveFigure.transform
                .DORotateQuaternion(targetRot, 0.2f)
                .OnComplete(() => {
                    ActiveFigure.transform.rotation = targetRot;
                });
        }
    }

    private bool CanRotate(Vector3 axis) 
    {
        if (ActiveFigure == null)
        {
            return false;
        }

        Quaternion deltaRot = Quaternion.Euler(axis * 90f);
        Vector3 pivotPos = ActiveFigure.Center.position;

        foreach (Transform cube in ActiveFigure.Parts) 
        {
            Vector3 dir = cube.position - pivotPos;
            Vector3 rotatedDir = deltaRot * dir;
            Vector3Int newCell = Vector3Int.RoundToInt(pivotPos + rotatedDir);

            if (!_board.IsInside(newCell) || _board.IsOccupied(newCell))
            {
                return false;
            }
        }
        return true;
    }

    public void Move(Vector3Int direction) 
    {
        if (ActiveFigure != null && CanMove(direction) && !_isLocking) 
        {
            Vector3 endValue = ActiveFigure.transform.position + direction;
            DOTween.Complete(ActiveFigure.transform);
            ActiveFigure.transform.DOMove(endValue, 0.1f).SetLink(ActiveFigure.gameObject);
        }
    }

    private bool CanMove(Vector3Int direction)
    {
        if (ActiveFigure == null)
        {
            return false;
        }

        foreach (Transform cube in ActiveFigure.Parts) 
        {
            Vector3Int newPos = Vector3Int.RoundToInt(cube.position + direction);
            if (!_board.IsInside(newPos) || _board.IsOccupied(newPos))
            {
                return false;
            }
        }
        return true;
    }

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _dropper.ActivateSlowFall(timeModifier);
    }

    private void OnEndSlowDropAbility()
    {
        _dropper.DeactivateSlowFall();
    }
}