using System;
using System.Collections.Generic;
using UnityEngine;

public class FigureController : MonoBehaviour 
{
    [SerializeField] private FigureSpawner _figureSpawner;
    [SerializeField] private FigureLight _figureLight;

    public bool Active { get; private set; }
    public Figure ActiveFigure { get; private set; }
    public Action<FigureType, int> OnPlaceFigure;
    public Action<FigureType, List<int>> OnClearPlanes;
    public bool IsLocking;
    

    private FigureMover _figureMover;
    private FigureRotator _figureRotator;
    private FigureDropper _figureDropper;

    private InputManager _inputManager;
    private Board _board;
    private ScoreManager _scoreManager;

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
        _figureMover = new FigureMover(this, _board);
        _figureRotator = new FigureRotator(this, _board);
        _figureDropper = new FigureDropper(this, _figureMover, _board, _scoreManager);
    }

    private void Update() 
    {
        if (!Active || IsLocking)
        {
            return;
        }

        _figureDropper.Update(Time.deltaTime);
        
        _figureLight.UpdateLandingLights(ActiveFigure);
    }

    public void StartGame()
    {
        Debug.Log("---StartGame");
        StartSpawning();

        if (ActiveFigure != null) 
        {
            Destroy(ActiveFigure.gameObject);
        }
        SpawnFigure();
        Debug.Log("StartGame---");
    }

    public void StartSpawning() 
    {
        Debug.Log("---StartSpawning");
        gameObject.SetActive(true);
        Active = true;

        _inputManager.PlayerMoveFigure += Move;
        _inputManager.PlayerRotateFigure += Rotate;
        _inputManager.PlayerDropFigure += Drop;
        _inputManager.PlayerHardDropFigure += HardDrop;

        if (ActiveFigure == null) 
        {
            SpawnFigure();
        }
        Debug.Log("StartSpawning---");
    }

    public void StopSpawning() 
    {
        Debug.Log("---StopSpawning");
        _inputManager.PlayerMoveFigure -= Move;
        _inputManager.PlayerRotateFigure -= Rotate;
        _inputManager.PlayerDropFigure -= Drop;
        _inputManager.PlayerHardDropFigure -= HardDrop;

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
    
    public void LockFigure(Figure figure) 
    {
        if (figure == null)
        {
            IsLocking = false;
            return;
        }

        FigureSO figureData = figure.Data;
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

        SpawnFigure();
        IsLocking = false;
    }

    private void SpawnFigure() 
    {
        ActiveFigure = _figureSpawner.SpawnFigure();
    }

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _figureDropper.ActivateSlowFall(timeModifier);
    }

    private void OnEndSlowDropAbility()
    {
        _figureDropper.DeactivateSlowFall();
    }

    private void Move(Vector3Int direction) 
    {
        if (IsLocking)
        {
            return;
        }
        _figureMover.Move(direction);
    }

    private void Rotate(Vector3 axis)
    {
        if (IsLocking)
        {
            return;
        }
        _figureRotator.Rotate(axis);
    }

    private void Drop()
    {
        _figureDropper.Drop();
    }

    private void HardDrop()
    {
        _figureDropper.HardDrop();
    }
}