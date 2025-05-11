using TMPro;
using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] private GameObject _recordHeader;
    [SerializeField] private GameObject _defaultHeader;
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

        ServiceLocator.Instance.ADManager.StartVideoEvent(ADRewardType.None);
    }

    private void SetHeader(int currentScore)
    {
        bool isDailyBest = currentScore >= _savesManager.GetDailyBestScore();
        bool isAllTimeBest = currentScore >= _savesManager.GetAllTimeBestScore();

        _recordHeader.SetActive(isDailyBest || isAllTimeBest);
        _defaultHeader.SetActive(!(isDailyBest || isAllTimeBest));

        if (isAllTimeBest)
        {
            _headerText.text = "Новый рекорд";
        }
        else if (isDailyBest)
        {
            _headerText.text = "Новый дневной рекорд";
        }
    }
}