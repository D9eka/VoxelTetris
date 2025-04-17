using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HUDScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Image _slowDropStartAbilityIcon;
    [Space]
    [SerializeField] private GameObject _scoreDeltaPrefab;
    [SerializeField] private float _scoreDeltaDuration;
    
    public Action ActivateSlowDropAbility;
    public Action ActivateDeletePlaneAbility;

    private int _previousScore;
    
    private float rectWidth;
    private float rectHeight;

    private float prefabWidth;
    private float prefabHeight;

    private List<GameObject> _spawnedScores;

    private void Awake()
    {
        Vector2 rectSizes = GetSizes(GetComponent<RectTransform>());
        rectWidth = rectSizes.x;
        rectHeight = rectSizes.y;
        
        Vector2 prefabRectSizes = GetSizes(_scoreDeltaPrefab.GetComponent<RectTransform>());
        prefabWidth = prefabRectSizes.x;
        prefabHeight = prefabRectSizes.y;
        
        _spawnedScores = new List<GameObject>();
    }

    private Vector2 GetSizes(RectTransform rectTransform)
    {
        Rect rect = rectTransform.rect;
        return new Vector2(rect.width, rect.height);
    }

    private void OnEnable()
    {
        ServiceLocator.Instance.ScoreManager.OnScoreChanged += OnScoreChanged;
        
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility += OnEndSlowDownAbility;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        foreach (GameObject spawnedScore in _spawnedScores)
        {
            Destroy(spawnedScore);
        }
        _spawnedScores.Clear();
        
        ServiceLocator.Instance.ScoreManager.OnScoreChanged -= OnScoreChanged;
        
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility -= OnEndSlowDownAbility;
    }

    public void OnActivateSlowDropAbility()
    {
        ActivateSlowDropAbility?.Invoke();
    }
    
    public void OnActivateDeletePlaneAbility()
    {
        ActivateDeletePlaneAbility?.Invoke();
    }

    private void OnScoreChanged(int score)
    {
        _scoreText.text = score.ToString();
        
        int delta = score - _previousScore;
        StartCoroutine(SpawnScoreDelta(delta));
        _previousScore = score;
    }

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _slowDropStartAbilityIcon.gameObject.SetActive(true);
    }

    private void OnEndSlowDownAbility()
    {
        _slowDropStartAbilityIcon.gameObject.SetActive(false);
    }

    private IEnumerator SpawnScoreDelta(int delta)
    {
        Vector3 spawnPos = new Vector3
            (
                Random.Range(0, rectWidth - prefabWidth), 
                -Random.Range(0, rectHeight - prefabHeight),
                transform.position.z
            );
        TextMeshProUGUI scoreDelta = Instantiate(_scoreDeltaPrefab, transform).GetComponent<TextMeshProUGUI>();
        scoreDelta.rectTransform.anchoredPosition3D = spawnPos;
        scoreDelta.text = "+" + delta;
        _spawnedScores.Add(scoreDelta.gameObject);
        
        float initialTime = Time.time;
        while (Time.time - initialTime < _scoreDeltaDuration)
        {
            scoreDelta.alpha = Mathf.Lerp(1f, 0f, (Time.time - initialTime) / _scoreDeltaDuration);
            yield return new WaitForEndOfFrame();
        }
        
        _spawnedScores.Remove(scoreDelta.gameObject);
        Destroy(scoreDelta.gameObject);
    }
}