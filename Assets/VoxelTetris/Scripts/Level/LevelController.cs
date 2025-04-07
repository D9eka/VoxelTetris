using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Action StartGame;
    public Action PlayerPause;
    public Action UIResume;
    public Action EndGame;

    private bool _gameStarted;
    
    private void Start()
    {
        ServiceLocator.Instance.GridController.OnReachLimit += OnReachLimit;
        
        ServiceLocator.Instance.InputManager.PlayerPause += OnPlayerPause;
        ServiceLocator.Instance.InputManager.UIResume += OnUIResume;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.GridController.OnReachLimit -= OnReachLimit;
        
        ServiceLocator.Instance.InputManager.PlayerPause -= OnPlayerPause;
        ServiceLocator.Instance.InputManager.UIResume -= OnUIResume;
    }

    private void OnReachLimit()
    {
        _gameStarted = false;
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
        PlayerPause?.Invoke();
    }

    public void ProcessUIResume()
    {
        UIResume?.Invoke();
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