using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private GameObject _statisticsScreen;
    [SerializeField] private GameObject _settingsScreen;
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _reloadScreen;
    [SerializeField] private GameObject _confirmExitScreen;

    private Stack<GameObject> _screenStack = new Stack<GameObject>();

    private void Start()
    {
        SetScreen(_startScreen);
        
        ServiceLocator.Instance.GridController.OnReachLimit += OnReachLimit;
        
        ServiceLocator.Instance.LevelController.StartGame += OnStartGame;
        ServiceLocator.Instance.LevelController.PlayerPause += OnPause;
        ServiceLocator.Instance.LevelController.UIResume += OnResume;
        ServiceLocator.Instance.LevelController.EndGame += EndGame;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.GridController.OnReachLimit -= OnReachLimit;
        
        ServiceLocator.Instance.LevelController.StartGame -= OnStartGame;
        ServiceLocator.Instance.LevelController.PlayerPause -= OnPause;
        ServiceLocator.Instance.LevelController.UIResume -= OnResume;
        ServiceLocator.Instance.LevelController.EndGame -= EndGame;
    }

    public bool HaveQueue()
    {
        return _screenStack.Count > 1;
    }
    
    public void SetScreen(GameObject screen)
    {
        if (_screenStack.Count > 0)
        {
            _screenStack.Peek().SetActive(false);
        }
        
        _screenStack.Push(screen);
        screen.SetActive(true);
    }

    public void SetScreenForce(GameObject screen)
    {
        while (_screenStack.Count > 0)
        {
            CloseCurrentScreen();
        }
        SetScreen(screen);
    }

    public void CloseCurrentScreen()
    {
        if (_screenStack.Count > 0)
        {
            GameObject currentScreen = _screenStack.Pop();
            currentScreen.SetActive(false);
            
            if (_screenStack.Count > 0)
            {
                _screenStack.Peek().SetActive(true);
            }
        }
    }

    private void OnReachLimit()
    {
        SetScreen(_reloadScreen);
    }
    
    private void OnStartGame()
    {
        CloseCurrentScreen();
    }
    
    private void OnPause()
    {
        SetScreen(_pauseScreen);
    }

    private void OnResume()
    {
        CloseCurrentScreen();
    }

    private void EndGame()
    {
        SetScreenForce(_startScreen);
    }
}