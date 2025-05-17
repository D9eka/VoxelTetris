using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private GameObject[] _headerNewScoreElems;
    [Space]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [Space]
    [SerializeField] private AudioClip _endGameClip;
    
    private ScoreManager _scoreManager;
    private SavesManager _savesManager;
    
    private void OnEnable()
    {
        _scoreManager = ServiceLocator.Instance.ScoreManager;
        _savesManager = ServiceLocator.Instance.SavesManager;
        
        int currentScore = _scoreManager.Score;
        _scoreText.text = currentScore.ToString();
        SetHeader(currentScore);
        _savesManager.SaveScore(currentScore);

        ServiceLocator.Instance.ADManager.StartVideoEvent(ADRewardType.None);
    }

    private void SetHeader(int currentScore)
    {
        bool isDailyBest = currentScore > _savesManager.GetDailyBestScore();
        bool isAllTimeBest = currentScore > _savesManager.GetAllTimeBestScore();

        bool haveNewBest = isDailyBest || isAllTimeBest;
        foreach (GameObject headerNewScoreElem in _headerNewScoreElems)
        {
            headerNewScoreElem.SetActive(haveNewBest);
        }
        if (isDailyBest)
        {
            _headerText.text = "Новый дневной рекорд!";
        }
        if (isAllTimeBest)
        {
            _headerText.text = "Новый рекорд!";
        }
    }
}