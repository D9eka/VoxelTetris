using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

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
    private DateTime _currentDate;

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
        ServiceLocator.Instance.LevelController.EndGame += SaveScores;

        ServiceLocator.Instance.GridController.OnPlaceFigure += OnPlaceFigure;
        ServiceLocator.Instance.GridController.OnClearPlane += OnClearPlane;

        _currentDate = DateTime.Now.Date;
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
    private void SaveScores()
    {
        var saves = YandexGame.savesData;

        saves.previousGameScore = _score;

        if (_score > saves.allTimeBestScore)
        {
            saves.allTimeBestScore = _score;
            YandexGame.NewLeaderboardScores("bestScore", saves.allTimeBestScore);
        }

        DateTime lastDate;
        if (!DateTime.TryParse(saves.lastPlayDate, out lastDate))
            lastDate = DateTime.MinValue;

        if (lastDate.Date == _currentDate.Date)
        {
            if (_score > saves.dailyBestScore)
            {
                saves.dailyBestScore = _score;
            }
        }
        else
        {
            saves.dailyBestScore = _score;
        }

        saves.lastPlayDate = _currentDate.ToString();

        YandexGame.SaveProgress();
    }
}