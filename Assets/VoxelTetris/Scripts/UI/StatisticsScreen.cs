using UnityEngine;
using UnityEngine.UI;

public class StatisticsScreen : MonoBehaviour
{
    [SerializeField] private Slider _previousGamePointsSlider;
    [SerializeField] private Slider _allDayBestPointsSlider;
    [SerializeField] private Slider _allTimeBestPointsSlider;
    
    private void OnEnable()
    {
        SetupScreen();
    }

    private void SetupScreen()
    {
        int previuosGamePoints = Random.Range(1, 101);
        int allDayBestPoints = Random.Range(previuosGamePoints, 201);
        int allTimeBestPoints = Random.Range(allDayBestPoints, 301);
        
        float sumPoints = Mathf.Max(previuosGamePoints, allDayBestPoints, allTimeBestPoints);
        _previousGamePointsSlider.value = previuosGamePoints / sumPoints;
        _allDayBestPointsSlider.value = allDayBestPoints / sumPoints;
        _allTimeBestPointsSlider.value = allTimeBestPoints / sumPoints;
        
        Debug.Log($"{previuosGamePoints} {allTimeBestPoints} {allDayBestPoints}");
    }
}
