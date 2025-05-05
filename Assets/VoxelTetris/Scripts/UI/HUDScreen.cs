using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HUDScreen : MonoBehaviour
{
    [SerializeField] private ScoreView _scoreView;
    [Space] 
    [SerializeField] private Button _activateSlowDropAbilityButton;
    [SerializeField] private Button _activateDeletePlainAbilityButton;
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
    private ADManager _adManager;

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
        _adManager = ServiceLocator.Instance.ADManager;
        
        _abilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        _abilityManager.OnEndSlowDropAbility += OnEndSlowDownAbility;
        _adManager.RewardVideoEvent += RewardVideoEvent;
        
        _activateSlowDropAbilityButton.onClick.AddListener(() => ActivateAbility(ADRewardType.SlowDropAbility));
        _activateDeletePlainAbilityButton.onClick.AddListener(() => ActivateAbility(ADRewardType.DeletePlaneAbility));
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
        _adManager.RewardVideoEvent -= RewardVideoEvent;
        
        _activateSlowDropAbilityButton.onClick.RemoveAllListeners();
        _activateDeletePlainAbilityButton.onClick.RemoveAllListeners();
    }

    private void ActivateAbility(ADRewardType rewardType)
    {
        _audioManager.PlaySound(_activateAbilityClip, Vector3.zero);
        _adManager.RewardVideoEvent(rewardType);
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
    
    private void RewardVideoEvent(ADRewardType adRewardType)
    {
        if (adRewardType is ADRewardType.SlowDropAbility or ADRewardType.DeletePlaneAbility)
        {
            switch (adRewardType)
            {
                case ADRewardType.SlowDropAbility:
                    _audioManager.PlaySound(_slowDropAbilityClip, Vector3.zero);
                    ActivateSlowDropAbility?.Invoke();
                    break;
                case ADRewardType.DeletePlaneAbility:
                    _audioManager.PlaySound(_deletePlainClip, Vector3.zero);
                    ActivateDeletePlaneAbility?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
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