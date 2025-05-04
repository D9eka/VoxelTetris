using System;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Color _dailyBestScoreColor;
    [SerializeField] private Color _allTimeBestScoreColor;
    
    public Action<int> OnScoreChanged;

    private Color _initialScoreColor;
    private int _currentScore;
    private int _previousScore;
    
    private void Awake()
    {
        _initialScoreColor = _scoreText.color;
    }

    private void OnEnable()
    {
        _scoreText.color = _initialScoreColor;
        
        ScoreManager scoreManager = ServiceLocator.Instance.ScoreManager;
        _scoreText.text = scoreManager.Score.ToString();
        scoreManager.OnScoreChanged += ScoreManagerOnScoreChanged;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.ScoreManager.OnScoreChanged -= ScoreManagerOnScoreChanged;
    }

    private void ScoreManagerOnScoreChanged(int score)
    {
        _currentScore = score;
        _scoreText.text = score.ToString();
        ChangeScoreTextColor();
        
        int delta = score - _previousScore;
        OnScoreChanged?.Invoke(delta);
        
        _previousScore = score;
    }

    private void ChangeScoreTextColor()
    {
        SavesManager savesManager = ServiceLocator.Instance.SavesManager;
        if (_currentScore > savesManager.GetDailyBestScore())
        {
            _scoreText.color = _dailyBestScoreColor;
        }
        if (_currentScore > savesManager.GetAllTimeBestScore())
        {
            _scoreText.color = _allTimeBestScoreColor;
        }
    }
}