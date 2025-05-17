using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioManagerData _data;
    [SerializeField] private GameObject _audioObjectPrefab;
    [SerializeField] private AudioSource _musicAudioSource;

    public Action<float> ChangeSoundVolume;
    public Action<float> ChangeMusicVolume;
    
    private Vector3 _defaultPosition = Vector3.zero;
    
    private void Awake()
    {
        _musicAudioSource.loop = true;
    }

    private void Start()
    {
        ScoreManager scoreManager = ServiceLocator.Instance.ScoreManager;
        scoreManager.OnScoreStateChanged += OnScoreStateChanged;
        
        LevelController levelController = ServiceLocator.Instance.LevelController;
        levelController.ReachLimit += OnReachLimit;
        levelController.EndGame += EndGame;
        
        FigureController figureController = ServiceLocator.Instance.FigureController;
        figureController.OnPlaceFigure += OnPlaceFigure;
        figureController.OnClearPlanes += OnClearPlanes;
        
        AbilityManager abilityManager = ServiceLocator.Instance.AbilityManager;
        abilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        abilityManager.OnDeletePlaneAbility += OnDeletePlaneAbility;
        
        PlayMusic(_data.LowMusicClip);
    }

    private void OnPlaceFigure(FigureType arg1, int arg2)
    {
        PlaySound(_data.PlaceFigureClip, _defaultPosition);
    }

    private void OnClearPlanes(FigureType arg1, List<int> arg2)
    {
        if (arg2.Count > 0)
        {
            PlaySound(_data.ClearPlanesClip, _defaultPosition);
        }
    }

    private void OnReachLimit()
    {
        PlaySound(_data.EndGameClip, _defaultPosition);
    }

    private void OnStartSlowDropAbility(float obj)
    {
        OnActivateAbility();
    }


    private void OnDeletePlaneAbility(int obj)
    {
        OnActivateAbility();
    }

    private void OnActivateAbility()
    {
        PlaySound(_data.UseAbilityClip, _defaultPosition);
    }

    private void OnScoreStateChanged(ScoreState scoreState)
    {
        PlayMusic(scoreState switch
        {
            ScoreState.Low => _data.LowMusicClip,
            ScoreState.Medium => _data.MediumMusicClip,
            ScoreState.High => _data.HighMusicClip,
            _ => _data.LowMusicClip,
        });
    }

    private void EndGame()
    {
        PlayMusic(_data.LowMusicClip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (_musicAudioSource.clip == clip)
            return;
        _musicAudioSource.Stop();
        _musicAudioSource.clip = clip;
        _musicAudioSource.Play();
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        AudioObject soundGO = Instantiate(_audioObjectPrefab, position, Quaternion.identity).GetComponent<AudioObject>();
        soundGO.Initialize(clip, 1f);
    }
}