using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "VoxelTetris/AudioData")]
public class AudioManagerData : ScriptableObject
{
    public AudioClip LowMusicClip => _lowMusicClip;
    public AudioClip MediumMusicClip => _mediumMusicClip;
    public AudioClip HighMusicClip => _highMusicClip;
    public AudioClip PlaceFigureClip => _placeFigureClip;
    public AudioClip ClearPlanesClip => _clearPlanesClip;
    public AudioClip UseAbilityClip => _useAbilityClip;
    public AudioClip BestScoreClip => _bestScoreClip;
    public AudioClip EndGameClip => _endGameClip;
    
    
    [Header("Music")]
    [SerializeField] private AudioClip _lowMusicClip;
    [SerializeField] private AudioClip _mediumMusicClip;
    [SerializeField] private AudioClip _highMusicClip;
    [Space] 
    [Header("Sounds")] 
    [SerializeField] private AudioClip _placeFigureClip;
    [SerializeField] private AudioClip _clearPlanesClip;
    [Space]
    [SerializeField] private AudioClip _useAbilityClip;
    [Space]
    [SerializeField] private AudioClip _bestScoreClip;
    [SerializeField] private AudioClip _endGameClip;
}