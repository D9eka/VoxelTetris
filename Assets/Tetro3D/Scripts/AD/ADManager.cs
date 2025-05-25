using System;
using UnityEngine;
using YG;

public class ADManager : MonoBehaviour
{
    public Action<ADRewardType> RewardVideoEvent;
    
    private FigureController _figureController;

    private bool _needToStopGame;

    public Action PauseSound;
    public Action ResumeSound;
    
    private void Start()
    {
        _figureController = ServiceLocator.Instance.FigureController;
        
        LevelController levelController = ServiceLocator.Instance.LevelController;
        levelController.PlayerPause += StartFullAd;
        levelController.EndGame += StartFullAd;
        
        YandexGame.RewardVideoEvent += OnRewardVideoEvent;
        YandexGame.CloseVideoEvent += OnCloseVideoEvent;

        YandexGame.CloseFullAdEvent += OnCloseVideoEvent;
    }

    private void OnDestroy()
    {
        LevelController levelController = ServiceLocator.Instance.LevelController;
        levelController.PlayerPause -= StartFullAd;
        levelController.EndGame -= StartFullAd;
        
        YandexGame.RewardVideoEvent -= OnRewardVideoEvent;
        YandexGame.CloseVideoEvent -= OnCloseVideoEvent;

        YandexGame.CloseFullAdEvent -= OnCloseVideoEvent;
    }

    public void StartRewardVideoEvent(ADRewardType adRewardType)
    {
        Debug.Log("StartRewardVideoEvent");
        PauseGame();
        
        YandexGame.RewVideoShow((int)adRewardType);

#if  UNITY_EDITOR
        OnRewardVideoEvent((int)adRewardType);
#endif
    }

    public void StartFullAd()
    {
        Debug.Log("StartFullVideoEvent");
        PauseGame();

        YandexGame.FullscreenShow();
    }

    private void OnRewardVideoEvent(int id)
    {
        Debug.Log("EndRewardVideoEvent");
        OnVideoEnd();
        RewardVideoEvent?.Invoke((ADRewardType)id);
    }
    
    private void OnCloseVideoEvent()
    {
        OnVideoEnd();
    }

    private void OnVideoEnd()
    {
        ResumeGame();
    }

    private void PauseGame()
    {
        _needToStopGame = ServiceLocator.Instance.LevelController.GameStarted && _figureController.Active;
        if (_needToStopGame)
        {
            ServiceLocator.Instance.FigureController.StopSpawning();
        }
        PauseSound?.Invoke();
    }

    private void ResumeGame()
    {
        if (_needToStopGame)
        {
            ServiceLocator.Instance.FigureController.StartSpawning();
            _needToStopGame = false;
        }
        ResumeSound?.Invoke();
    }
}