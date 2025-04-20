using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public enum ScoreType
    {
        PlaceFigure,
        DeletePlane
    }
    
    public int Score 
    {
        get
        {
            return _score;
        }
        private set
        {
            _score = value;
            OnScoreChanged?.Invoke(_score);
        }
    }
    
    public Action<int> OnScoreChanged;

    private Dictionary<FigureType, int> _figurePrice;
    
    private int _score;

    private ScoreType _previousScoreType;
    private int _previousScoreTypeLength;

    private void Awake()
    {
        _figurePrice = new Dictionary<FigureType, int>();
        _figurePrice.Add(FigureType.I, 100);
        _figurePrice.Add(FigureType.L, 80);
        _figurePrice.Add(FigureType.T, 90);
        _figurePrice.Add(FigureType.O, 70);
        _figurePrice.Add(FigureType.J, 80);
        _figurePrice.Add(FigureType.S, 80);
        _figurePrice.Add(FigureType.Z, 80);
    }

    private void Start()
    {
        ServiceLocator.Instance.LevelController.StartGame += StartGame;
        
        ServiceLocator.Instance.GridController.OnPlaceFigure += OnPlaceFigure;
        ServiceLocator.Instance.GridController.OnClearPlane += OnClearPlane;
    }

    private void StartGame()
    {
        Score = 0;
    }

    private void OnPlaceFigure(FigureController figure, int planePosY)
    {
        Score += GetAdditionalScore(_figurePrice[figure.Type], ScoreType.PlaceFigure, planePosY);
    }

    private void OnClearPlane(FigureController figure, int planePosY)
    {
        ScoreType type = ScoreType.DeletePlane;
        int additionalScore = GetAdditionalScore(_figurePrice[figure.Type] * 2, type, planePosY);

        additionalScore += 300;
        if (_previousScoreTypeLength >= 1)
        {
            additionalScore += 50;
        }
        Score += additionalScore;
    }

    private int GetAdditionalScore(int score, ScoreType type, int planePosY)
    {
        int additionalScore = score;
        if (planePosY >= 4)
        {
            additionalScore += 30;
        }
        if (_previousScoreType == type)
        {
            _previousScoreTypeLength++;
        }
        else
        {
            _previousScoreType = type;
            _previousScoreTypeLength = 1;
        }

        return additionalScore;
    }
}