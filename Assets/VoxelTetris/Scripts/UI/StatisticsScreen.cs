using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YG;

public class StatisticsScreen : MonoBehaviour
{
    [SerializeField] private Slider _previousGamePointsSlider;
    [SerializeField] private Slider _allDayBestPointsSlider;
    [SerializeField] private Slider _allTimeBestPointsSlider;
    [Space]
    [SerializeField] private Color _minStatColor;
    [SerializeField] private Color _middleStatColor;
    [SerializeField] private Color _maxStatColor;
    
    private void OnEnable()
    {
        YandexGame.GetDataEvent += SetupScreen;

        if (YandexGame.SDKEnabled)
            SetupScreen();
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= SetupScreen;
    }

    private void SetupScreen()
    {
        if (YandexGame.savesData == null)
        {
            Debug.LogError("SavesData не загружены!");
            return;
        }

        var saves = YandexGame.savesData;

        int previousGamePoints = saves.previousGameScore;
        int allDayBestPoints = saves.dailyBestScore;
        int allTimeBestPoints = saves.allTimeBestScore;

        Debug.Log($"Загружено: Previous={previousGamePoints}, Daily={allDayBestPoints}, AllTime={allTimeBestPoints}");

        int minPoints = Mathf.Min(previousGamePoints, allDayBestPoints, allTimeBestPoints);
        int maxPoints = Mathf.Max(previousGamePoints, allDayBestPoints, allTimeBestPoints);
        
        SetupSlider(_previousGamePointsSlider, previousGamePoints, minPoints, maxPoints);
        SetupSlider(_allDayBestPointsSlider, allDayBestPoints, minPoints, maxPoints);
        SetupSlider(_allTimeBestPointsSlider, allTimeBestPoints, minPoints, maxPoints);
    }

    private void SetupSlider(Slider slider, int points, int minPoints, int maxPoints)
    {
        slider.value = GetSliderValue(points, maxPoints);
        slider.fillRect.GetComponentInChildren<TextMeshProUGUI>().text = points.ToString();
        slider.fillRect.GetComponent<Image>().color = GetSliderColor(points, minPoints, maxPoints);
    }

    private float GetSliderValue(int points, int maxPoints)
    {
        if (maxPoints == 0)
            return 0f;
        return Mathf.Clamp01(points / (float)maxPoints);
    }

    private Color GetSliderColor(int points, int minPoints, int maxPoints)
    {
        Color colorToDraw = _middleStatColor;
        if (points == minPoints)
        {
            colorToDraw = _minStatColor;
        }
        if (points == maxPoints)
        {
            colorToDraw = _maxStatColor;
        }
        return colorToDraw;
    }
}
