using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private GameObject _settingsScreen;
    [SerializeField] private GameObject _reloadScreen;

    private void Start()
    {
        SetScreen(_startScreen);
        
        ServiceLocator.Instance.GridController.OnReachLimit += () => SetScreen(_reloadScreen);
    }

    public void SetScreen(GameObject screen)
    {
        HideAllScreens();
        screen.SetActive(true);
    }

    public void HideAllScreens()
    {
        _startScreen.SetActive(false);
        _settingsScreen.SetActive(false);
        _reloadScreen.SetActive(false);
    }
}
