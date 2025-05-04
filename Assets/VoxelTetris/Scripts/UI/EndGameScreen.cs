using System;
using TMPro;
using UnityEngine;
using YG;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] private GameObject _header;
    [SerializeField] private TextMeshProUGUI _headerText;
    
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void OnEnable()
    {
        int currentScore = ServiceLocator.Instance.ScoreManager.Score;
        _scoreText.text = currentScore.ToString();
        SetHeader(currentScore);
        ServiceLocator.Instance.SavesManager.SaveScore(currentScore);

        YandexGame.RewVideoShow(3);
    }

    private void SetHeader(int currentScore)
    {
        SavesManager savesManager = ServiceLocator.Instance.SavesManager;
        bool isDailyBest = currentScore > savesManager.GetDailyBestScore();
        bool isAllTimeBest = currentScore > savesManager.GetAllTimeBestScore();
        
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