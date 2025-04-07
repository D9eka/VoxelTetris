using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Action StartGame;
    public Action PlayerPause;
    public Action UIResume;
    public Action EndGame;
    
    private void Start()
    {
        ServiceLocator.Instance.InputManager.PlayerPause += OnPlayerPause;
        ServiceLocator.Instance.InputManager.UIResume += OnUIResume;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.InputManager.PlayerPause -= OnPlayerPause;
        ServiceLocator.Instance.InputManager.UIResume -= OnUIResume;
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
        if (!figuresController.Active)
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
        
        UIResume?.Invoke();
    }

    public void ProcessStartGame()
    {
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
        EndGame?.Invoke();
    }

    public void ProcessRestartGame()
    {
        ProcessEndGame();
        ProcessStartGame();
    }
}