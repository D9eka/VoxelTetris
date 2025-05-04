using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FiguresControllerData", menuName = "VoxelTetris/FiguresControllerData")]
public class FiguresControllerData : ScriptableObject
{
    public GameObject[] FigurePrefabs => _figurePrefabs;
    public FigureCube[] FigureCubes => _figureCubes;
    public Material[] FigureColors => _figureColors;
    public float SpawnOffsetY => _spawnOffsetY;
    public AudioClip FigureDropClip => _figureDropClip;
    public float LowTimeToDropFigure => _lowTimeToDropFigure;
    public float MediumTimeToDropFigure => _mediumTimeToDropFigure;
    public int ScoreToMediumTimeToDropFigure => _ScoreToMediumTimeToDropFigure;
    public float HighTimeToDropFigure => _highTimeToDropFigure;
    public int ScoreToHighTimeToDropFigure => _ScoreToHighTimeToDropFigure;
        
    [SerializeField] private GameObject[] _figurePrefabs;
    [SerializeField] private FigureCube[] _figureCubes;
    [SerializeField] private Material[] _figureColors;
    [SerializeField] private float _spawnOffsetY;
    [SerializeField] private AudioClip _figureDropClip;
    [Space]
    [SerializeField] private float _lowTimeToDropFigure;
    [Space]
    [SerializeField] private float _mediumTimeToDropFigure;
    [SerializeField] private int _ScoreToMediumTimeToDropFigure;
    [Space]
    [SerializeField] private float _highTimeToDropFigure;
    [SerializeField] private int _ScoreToHighTimeToDropFigure;
}