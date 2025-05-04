using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject _audioObjectPrefab;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioClip _initialMusic;
    [Space]
    [SerializeField] private AudioClip _lowSoundClip;
    [Space] 
    [SerializeField] private int _mediumSoundScore;
    [SerializeField] private AudioClip _mediumSoundClip;
    [Space] 
    [SerializeField] private int _highSoundScore;
    [SerializeField] private AudioClip _highSoundClip;

    public Action<float> ChangeSoundVolume;
    public Action<float> ChangeMusicVolume;
    
    private ScoreManager _scoreManager;
    
    private void Awake()
    {
        _musicAudioSource.loop = true;
    }

    private void Start()
    {
        _scoreManager = ServiceLocator.Instance.ScoreManager;
        _scoreManager.OnScoreChanged += OnScoreChanged;
        
        PlayMusic(_initialMusic);
        
        LevelController levelController = ServiceLocator.Instance.LevelController;
        levelController.EndGame += EndGame;
    }

    private void OnScoreChanged(int score)
    {
        if (score < _mediumSoundScore)
        {
            PlayMusic(_lowSoundClip);
        }
        if (score < _highSoundScore)
        {
            PlayMusic(_mediumSoundClip);
        }
        if (score >= _highSoundScore)
        {
            PlayMusic(_highSoundClip);
        }
    }

    private void EndGame()
    {
        PlayMusic(_initialMusic);
    }

    public void OnChangeSoundVolume(float volume)
    {
        ChangeSoundVolume?.Invoke(volume);
    }

    public void OnChangeMusicVolume(float volume)
    {
        ChangeMusicVolume?.Invoke(volume);
    }

    public void PlayMusic(AudioClip[] clips)
    {
        PlayMusic(clips[Random.Range(0, clips.Length)]);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (_musicAudioSource.clip == clip)
            return;
        _musicAudioSource.Stop();
        _musicAudioSource.clip = clip;
        _musicAudioSource.Play();
    }

    public void PlaySound(AudioClip[] clips, Vector3 position)
    {
        PlaySound(clips[Random.Range(0, clips.Length)], position);
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        AudioObject soundGO = Instantiate(_audioObjectPrefab, position, Quaternion.identity).GetComponent<AudioObject>();
        soundGO.Initialize(clip, 1f);
    }
}