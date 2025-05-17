using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreManager : MonoBehaviour
{
    public int Score 
    {
        get => _score;
        set
        {
            _score = value;
            OnScoreChanged?.Invoke(_score);
            ScoreState state = GetScoreState(_score);
            if (ScoreState != state)
            {
                ScoreState = state;
                OnScoreStateChanged?.Invoke(ScoreState);
            }
        }
    }
    public ScoreState ScoreState { get; private set; }
    
    public Action<int> OnScoreChanged;
    public Action<ScoreState> OnScoreStateChanged;

    private Dictionary<FigureType, ScoreData> _scoreRules;
    private int _score;
    private bool _timeSlowdownActive;
    private DateTime _currentDate;

    private FigureController _figureController;

    private class ScoreData
    {
        public int Placement { get; }
        public int PlaneClear { get; }
        public int ComboBonus { get; }

        public ScoreData(int placement, int planeClear, int comboBonus)
        {
            Placement = placement;
            PlaneClear = planeClear;
            ComboBonus = comboBonus;
        }
    }

    private void Awake()
    {
        InitializeScoreRules();
    }

    private void InitializeScoreRules()
    {
        _scoreRules = new Dictionary<FigureType, ScoreData>
        {
            { FigureType.I, new ScoreData(100, 200, 100) },
            { FigureType.L, new ScoreData(80, 160, 80) },
            { FigureType.T, new ScoreData(90, 180, 90) },
            { FigureType.O, new ScoreData(70, 140, 70) },
            { FigureType.J, new ScoreData(80, 160, 80) },
            { FigureType.S, new ScoreData(80, 140, 80) },
            { FigureType.Z, new ScoreData(80, 140, 80) },
        };
    }

    private void Start()
    {
        _figureController = ServiceLocator.Instance.FigureController;
        _figureController.OnPlaceFigure += OnPlaceFigure;
        _figureController.OnClearPlanes += OnClearPlane;
        
        ServiceLocator.Instance.LevelController.StartGame += StartGame;
        
        Board board = ServiceLocator.Instance.Board;
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility += OnEndSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnLayersDeleted += OnLayersDeleted;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.LevelController.StartGame -= StartGame;
        //_figureController.OnPlaceFigure -= OnPlaceFigure;
        //ServiceLocator.Instance.Board.OnClearPlane -= OnClearPlane;
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility -= OnEndSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnLayersDeleted -= OnLayersDeleted;
    }

    private void StartGame()
    {
        Score = 0;
    }

    private void OnPlaceFigure(FigureType figure, int centerPosY)
    {
        if (!_scoreRules.TryGetValue(figure, out var data))
        {
            return;
        }

        float bonusMultiplier = 1f;
        
        // Бонус верхней половины поля
        if (centerPosY >= 4)
        {
            bonusMultiplier += 0.3f;
        }
        
        // Бонус замедления времени
        if (_timeSlowdownActive)
        {
            bonusMultiplier += 0.3f;
        }

        int calculatedScore = Mathf.RoundToInt(data.Placement * bonusMultiplier);
        Score += calculatedScore;
    }

    private void OnClearPlane(FigureType figure, List<int> planePosY)
    {
        if (planePosY.Count == 0 || !_scoreRules.TryGetValue(figure, out var data))
        {
            return;
        }

        int comboMultiplier = planePosY.Count;
        int baseScore = data.PlaneClear + data.ComboBonus * comboMultiplier;
        Score += baseScore;
    }

    private void OnLayersDeleted(int deletedLayers)
    {
        int score = deletedLayers * 300;
        Score += score;
    }

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _timeSlowdownActive = true;
    }

    private void OnEndSlowDropAbility()
    {
        _timeSlowdownActive = false;
    }

    private ScoreState GetScoreState(int score)
    {
        if (score >= (int)ScoreState.High)
            return ScoreState.High;
        if (score >= (int)ScoreState.Medium)
            return ScoreState.Medium;
        return ScoreState.Low;
    }
}