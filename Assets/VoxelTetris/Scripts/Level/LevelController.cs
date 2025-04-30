using System;
using UnityEngine;
using YG;

public class LevelController : MonoBehaviour
{
    public Action StartGame;
    public Action PlayerPause;
    public Action UIResume;
    public Action EndGame;
    public Action ShowAdBeforeGameOver;

    private bool _gameStarted;

    private void Start()
    {
        ServiceLocator.Instance.GridController.OnReachLimit += OnReachLimit;
        ServiceLocator.Instance.InputManager.PlayerPause += OnPlayerPause;
        ServiceLocator.Instance.InputManager.UIResume += OnUIResume;

        YandexGame.RewardVideoEvent += OnRewardVideoEvent;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.GridController.OnReachLimit -= OnReachLimit;
        ServiceLocator.Instance.InputManager.PlayerPause -= OnPlayerPause;
        ServiceLocator.Instance.InputManager.UIResume -= OnUIResume;

        YandexGame.RewardVideoEvent -= OnRewardVideoEvent;
    }

    private void OnReachLimit()
    {
        if (!_gameStarted) return;

        _gameStarted = false;

        YandexGame.RewVideoShow(3);

        ShowAdBeforeGameOver?.Invoke();
    }

    private void OnRewardVideoEvent(int id)
    {
        if (id == 3)
        {
            EndGame?.Invoke();
        }
    }

    private void OnPlayerPause()
    {
        UIController uiController = ServiceLocator.Instance.UIController;
        if (uiController.HaveQueue())
        {
            uiController.CloseCurrentScreen();
            return;
        }
        
        FiguresController figuresController = ServiceLocator.Instance.FiguresController;
        if (!_gameStarted || !figuresController.Active)
        {
            return;
        }
        
        PlayerPause?.Invoke();
    }

    private void OnUIResume()
    {
        UIController uiController = ServiceLocator.Instance.UIController;
        if (uiController.HaveQueue())
        {
            uiController.CloseCurrentScreen();
            return;
        }
        
        FiguresController figuresController = ServiceLocator.Instance.FiguresController;
        if (!_gameStarted || figuresController.Active)
        {
            return;
        }
        
        UIResume?.Invoke();
    }

    public void ProcessStartGame()
    {
        _gameStarted = true;
        StartGame?.Invoke();
    }

    public void ProcessPlayerPause()
    {
        if (_gameStarted)
        {
            PlayerPause?.Invoke();
        }
    }

    public void ProcessUIResume()
    {
        if (_gameStarted)
        {
            UIResume?.Invoke();
        }
    }

    public void ProcessEndGame()
    {
        _gameStarted = false;
        EndGame?.Invoke();
    }

    public void ProcessRestartGame()
    {
        ProcessEndGame();
        ProcessStartGame();
    }
}