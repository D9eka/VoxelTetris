using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Score 
    {
        get => _score;
        private set
        {
            _score = value;
            OnScoreChanged?.Invoke(_score);
        }
    }
    
    public Action<int> OnScoreChanged;

    private Dictionary<FigureType, ScoreData> _scoreRules;
    private int _score;
    private int _comboMultiplier = 1;
    private bool _timeSlowdownActive;
    private DateTime _currentDate;

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
        ServiceLocator.Instance.LevelController.StartGame += StartGame;
        ServiceLocator.Instance.GridController.OnPlaceFigure += OnPlaceFigure;
        ServiceLocator.Instance.GridController.OnClearPlane += OnClearPlane;
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility += OnEndSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnLayersDeleted += OnLayersDeleted;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.LevelController.StartGame -= StartGame;
        ServiceLocator.Instance.GridController.OnPlaceFigure -= OnPlaceFigure;
        ServiceLocator.Instance.GridController.OnClearPlane -= OnClearPlane;
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility -= OnEndSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnLayersDeleted -= OnLayersDeleted;
    }

    private void StartGame()
    {
        Score = 0;
        _comboMultiplier = 1;
    }

    private void OnPlaceFigure(FigureController figure, int planePosY)
    {
        if (!_scoreRules.TryGetValue(figure.Type, out var data))
        {
            return;
        }

        float bonusMultiplier = 1f;
        
        // Бонус верхней половины поля
        if (planePosY >= 4)
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

    private void OnClearPlane(FigureController figure, int planePosY)
    {
        if (!figure || !_scoreRules.TryGetValue(figure.Type, out var data))
        {
            return;
        }

        int baseScore = data.PlaneClear + data.ComboBonus * _comboMultiplier;
        Score += baseScore;
        _comboMultiplier++;
    }

    private void OnLayersDeleted(int deletedLayers, int fullLayers)
    {
        int score = deletedLayers * 300 + fullLayers * 50;
        Score += score;
        _comboMultiplier = 1;
    }

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _timeSlowdownActive = true;
    }

    private void OnEndSlowDropAbility()
    {
        _timeSlowdownActive = false;
    }
}