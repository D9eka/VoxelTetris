using System;
using UnityEngine;
using YG;

public class ADManager : MonoBehaviour
{
    public Action<ADRewardType> RewardVideoEvent;
    
    private FiguresController _figuresController;

    private bool _needToStopGame;
    
    private void Start()
    {
        _figuresController = ServiceLocator.Instance.FiguresController;
        
        YandexGame.RewardVideoEvent += OnRewardVideoEvent;
        YandexGame.CloseVideoEvent += OnCloseVideoEvent;
    }

    private void OnDestroy()
    {
        YandexGame.RewardVideoEvent -= OnRewardVideoEvent;
        YandexGame.CloseVideoEvent -= OnCloseVideoEvent;
    }

    public void StartVideoEvent(ADRewardType adRewardType)
    {
        Debug.Log("StartVideoEvent");
        _needToStopGame = _figuresController.Active;
        if (_needToStopGame)
        {
            ServiceLocator.Instance.FiguresController.StopSpawning();
        }
        
        YandexGame.RewVideoShow((int)adRewardType);

#if  UNITY_EDITOR
        OnRewardVideoEvent((int)adRewardType);
#endif
    }

    private void OnRewardVideoEvent(int id)
    {
        Debug.Log("EndVideoEvent");
        OnVideoEnd();
        RewardVideoEvent?.Invoke((ADRewardType)id);
    }
    
    private void OnCloseVideoEvent()
    {
        OnVideoEnd();
    }

    private void OnVideoEnd()
    {
        if (_needToStopGame)
        {
            ServiceLocator.Instance.FiguresController.StartSpawning();
            _needToStopGame = false;
        }
    }
}