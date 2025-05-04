using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using YG;


public class HUDScreen : MonoBehaviour
{
    [SerializeField] private ScoreView _scoreView;
    [Space]
    [SerializeField] private Image _slowDropStartAbilityIcon;
    [Space]
    [SerializeField] private GameObject _scoreDeltaPrefab;
    [SerializeField] private float _scoreDeltaDuration;
    
    public Action ActivateSlowDropAbility;
    public Action ActivateDeletePlaneAbility;
    
    private float _rectWidth;
    private float _rectHeight;

    private float _prefabWidth;
    private float _prefabHeight;

    private List<GameObject> _spawnedScores;

    private void Awake()
    {
        Vector2 rectSizes = GetSizes(GetComponent<RectTransform>());
        _rectWidth = rectSizes.x;
        _rectHeight = rectSizes.y;
        
        Vector2 prefabRectSizes = GetSizes(_scoreDeltaPrefab.GetComponent<RectTransform>());
        _prefabWidth = prefabRectSizes.x;
        _prefabHeight = prefabRectSizes.y;
        
        _spawnedScores = new List<GameObject>();
    }

    private void Start()
    {
        _scoreView.OnScoreChanged += OnScoreChanged;
    }

    private void OnEnable()
    {
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility += OnEndSlowDownAbility;
        YandexGame.RewardVideoEvent += OnRewardVideoEvent;
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        foreach (GameObject spawnedScore in _spawnedScores)
        {
            Destroy(spawnedScore);
        }
        _spawnedScores.Clear();
        
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility -= OnEndSlowDownAbility;
        YandexGame.RewardVideoEvent -= OnRewardVideoEvent;
    }

    private Vector2 GetSizes(RectTransform rectTransform)
    {
        Rect rect = rectTransform.rect;
        return new Vector2(rect.width, rect.height);
    }

    private void OnScoreChanged(int scoreDelta)
    {
        StartCoroutine(SpawnScoreDelta(scoreDelta));
    }

    public void OnActivateSlowDropAbility()
    {
        ServiceLocator.Instance.FiguresController.StopSpawning();
        YandexGame.RewVideoShow(1);
    }
    
    public void OnActivateDeletePlaneAbility()
    {
        ServiceLocator.Instance.FiguresController.StopSpawning();
        YandexGame.RewVideoShow(2);
    }

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _slowDropStartAbilityIcon.gameObject.SetActive(true);
    }

    private void OnEndSlowDownAbility()
    {
        _slowDropStartAbilityIcon.gameObject.SetActive(false);
    }

    private void OnRewardVideoEvent(int id)
    {
        if (id == 1)
        {
            ActivateSlowDropAbility?.Invoke();
        }
        else if (id == 2)
        {
            ActivateDeletePlaneAbility?.Invoke();
        }
        
        ServiceLocator.Instance.FiguresController.StartSpawning();
    }

    private IEnumerator SpawnScoreDelta(int delta)
    {
        Vector3 spawnPos = new Vector3
            (
                Random.Range(0, _rectWidth - _prefabWidth), 
                -Random.Range(0, _rectHeight - _prefabHeight),
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