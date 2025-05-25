using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Action StartGame;
    public Action PlayerPause;
    public Action UIResume;
    public Action ReachLimit;
    public Action EndGame;

    public bool GameStarted { get; private set; }

    private void Start()
    {
        ServiceLocator.Instance.Board.OnReachLimit += OnReachLimit;
        ServiceLocator.Instance.InputManager.PlayerPause += OnPlayerPause;
        ServiceLocator.Instance.InputManager.UIResume += OnUIResume;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.Board.OnReachLimit -= OnReachLimit;
        ServiceLocator.Instance.InputManager.PlayerPause -= OnPlayerPause;
        ServiceLocator.Instance.InputManager.UIResume -= OnUIResume;
    }

    private void OnReachLimit()
    {
        if (!GameStarted)
        {
            return;
        }

        GameStarted = false;
        ReachLimit?.Invoke();
    }

    private void OnPlayerPause()
    {
        UIController uiController = ServiceLocator.Instance.UIController;
        if (uiController.HaveQueue())
        {
            uiController.CloseCurrentScreen();
            return;
        }
        
        FigureController figureController = ServiceLocator.Instance.FigureController;
        if (!GameStarted || !figureController.Active)
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
        }
        
        if (!GameStarted)
        {
            return;
        }
        
        UIResume?.Invoke();
    }

    public void ProcessStartGame()
    {
        GameStarted = true;
        StartGame?.Invoke();
    }

    public void ProcessPlayerPause()
    {
        if (GameStarted)
        {
            PlayerPause?.Invoke();
        }
    }

    public void ProcessUIResume()
    {
        if (GameStarted)
        {
            UIResume?.Invoke();
        }
    }

    public void ProcessEndGame()
    {
        GameStarted = false;
        EndGame?.Invoke();
    }

    public void ProcessRestartGame()
    {
        ProcessEndGame();
        ProcessStartGame();
    }
}