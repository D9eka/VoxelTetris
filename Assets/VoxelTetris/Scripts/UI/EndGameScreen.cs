using System;
using TMPro;
using UnityEngine;
using YG;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] private GameObject _header;
    [SerializeField] private TextMeshProUGUI _headerText;
    [Space]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [Space]
    [SerializeField] private AudioClip _endGameClip;
    
    private AudioManager _audioManager;
    private ScoreManager _scoreManager;
    private SavesManager _savesManager;
    
    private void OnEnable()
    {
        _audioManager = ServiceLocator.Instance.AudioManager;
        _scoreManager = ServiceLocator.Instance.ScoreManager;
        _savesManager = ServiceLocator.Instance.SavesManager;
        
        int currentScore = _scoreManager.Score;
        _scoreText.text = currentScore.ToString();
        SetHeader(currentScore);
        _savesManager.SaveScore(currentScore);

        _audioManager.PlaySound(_endGameClip, Vector3.zero);
        YandexGame.RewVideoShow(3);
    }

    private void SetHeader(int currentScore)
    {
        bool isDailyBest = currentScore > _savesManager.GetDailyBestScore();
        bool isAllTimeBest = currentScore > _savesManager.GetAllTimeBestScore();
        
        _header.SetActive(isDailyBest || isAllTimeBest);
        if (isDailyBest)
        {
            _headerText.text = "Новый дневной рекорд";
        }

        if (isAllTimeBest)
        {
            _headerText.text = "Новый рекорд";
        }
    }
}