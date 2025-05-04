using System;
using UnityEngine;
using YG;

public class SavesManager : MonoBehaviour
{
    private SavesYG _saves;
        
    private void Awake()
    {
        GetSaves();
    }

    public int GetPreviousScore()
    {
        return _saves?.previousGameScore ?? 0;
    }

    public int GetDailyBestScore()
    {
        return _saves?.dailyBestScore ?? 0;
    }

    public int GetAllTimeBestScore()
    {
        return _saves?.allTimeBestScore ?? 0;
    }
    
    public void SaveScore(int score)
    {
        if (_saves == null)
        {
            GetSaves();
        }

        if (_saves == null)
        {
            return;
        }
        
        SavePreviousScore(score);

        DateTime currentDate = DateTime.Now.Date;
        SaveDailyBestScore(score, currentDate);

        SaveAllTimeBestScore(score);

        _saves.lastPlayDate = currentDate.ToString();
        YandexGame.SaveProgress();
    }

    private void SavePreviousScore(int score)
    {
        _saves.previousGameScore = score;
    }

    private void SaveDailyBestScore(int score, DateTime currentDate)
    {
        if (!DateTime.TryParse(_saves.lastPlayDate, out DateTime lastDate))
        {
            lastDate = DateTime.MinValue;
        }

        if (lastDate.Date == currentDate.Date)
        {
            if (score > _saves.dailyBestScore)
                _saves.dailyBestScore = score;
        }
        else
        {
            _saves.dailyBestScore = score;
        }
    }

    private void SaveAllTimeBestScore(int score)
    {
        if (score > _saves.allTimeBestScore)
        {
            _saves.allTimeBestScore = score;
            YandexGame.NewLeaderboardScores("bestScore", _saves.allTimeBestScore);
        }
    }

    private void GetSaves()
    {
        if (YandexGame.savesData == null)
        {
            Debug.LogError("SavesData �� ���������!");
            return;
        }

        _saves = YandexGame.savesData;
    }
}