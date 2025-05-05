﻿using System;
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
    [Space]
    [SerializeField] private AudioClip _activateAbilityClip;
    [SerializeField] private AudioClip _slowDropAbilityClip;
    [SerializeField] private AudioClip _deletePlainClip;
    
    public Action ActivateSlowDropAbility;
    public Action ActivateDeletePlaneAbility;
    
    private float _rectWidth;
    private float _rectHeight;

    private float _prefabWidth;
    private float _prefabHeight;

    private List<GameObject> _spawnedScores;

    private AbilityManager _abilityManager;
    private AudioManager _audioManager;

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
        _abilityManager = ServiceLocator.Instance.AbilityManager;
        _audioManager = ServiceLocator.Instance.AudioManager;
        
        _abilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        _abilityManager.OnEndSlowDropAbility += OnEndSlowDownAbility;
        YandexGame.RewardVideoEvent += OnRewardVideoEvent;
        YandexGame.CloseVideoEvent += OnCloseVideoEvent;
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        foreach (GameObject spawnedScore in _spawnedScores)
        {
            Destroy(spawnedScore);
        }
        _spawnedScores.Clear();
        
        _abilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        _abilityManager.OnEndSlowDropAbility -= OnEndSlowDownAbility;
        YandexGame.RewardVideoEvent -= OnRewardVideoEvent;
        YandexGame.CloseVideoEvent -= OnCloseVideoEvent;
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
        OnStartVideoEvent();
        YandexGame.RewVideoShow(1);
    }
    
    public void OnActivateDeletePlaneAbility()
    {
        OnStartVideoEvent();
        YandexGame.RewVideoShow(2);
    }

    private void OnStartVideoEvent()
    {
        _audioManager.PlaySound(_activateAbilityClip, Vector3.zero);
        ServiceLocator.Instance.FiguresController.StopSpawning();

#if  UNITY_EDITOR
        OnRewardVideoEvent(0);
#endif
    }
    private void OnCloseVideoEvent()
    {
        ServiceLocator.Instance.FiguresController.StartSpawning();
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
        ServiceLocator.Instance.FiguresController.StartSpawning();
        if (id == 1)
        {
            ActivateSlowDropAbility?.Invoke();
            _audioManager.PlaySound(_slowDropAbilityClip, Vector3.zero);
        }
        else if (id == 2)
        {
            ActivateDeletePlaneAbility?.Invoke();
            _audioManager.PlaySound(_deletePlainClip, Vector3.zero);
        }
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