using System;
using System.Collections.Generic;
using UnityEngine;

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

    private int _score;
    private bool _timeSlowdownActive;
    private DateTime _currentDate;

    private FigureController _figureController;

    private void Start()
    {
        _figureController = ServiceLocator.Instance.FigureController;
        _figureController.OnPlaceFigure += OnPlaceFigure;
        _figureController.OnClearPlanes += OnClearPlane;
        
        ServiceLocator.Instance.LevelController.StartGame += StartGame;
        
        AbilityManager abilityManager = ServiceLocator.Instance.AbilityManager;
        abilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        abilityManager.OnEndSlowDropAbility += OnEndSlowDropAbility;
        abilityManager.OnLayersDeleted += OnLayersDeleted;
    }

    private void OnDisable()
    {
        _figureController.OnPlaceFigure -= OnPlaceFigure;
        _figureController.OnClearPlanes -= OnClearPlane;
        
        ServiceLocator.Instance.LevelController.StartGame -= StartGame;
        
        AbilityManager abilityManager = ServiceLocator.Instance.AbilityManager;
        abilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        abilityManager.OnEndSlowDropAbility -= OnEndSlowDropAbility;
        abilityManager.OnLayersDeleted -= OnLayersDeleted;
    }

    private void StartGame()
    {
        Score = 0;
    }

    private void OnPlaceFigure(FigureSO figureData, int centerPosY)
    {
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

        int calculatedScore = Mathf.RoundToInt(figureData.PlaceScore * bonusMultiplier);
        Score += calculatedScore;
    }

    private void OnClearPlane(FigureSO figureData, List<int> planePosY)
    {
        if (planePosY.Count == 0)
        {
            return;
        }

        int comboMultiplier = planePosY.Count;
        int baseScore = figureData.ClearPlaneScore + figureData.ComboBonusScore * comboMultiplier;
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