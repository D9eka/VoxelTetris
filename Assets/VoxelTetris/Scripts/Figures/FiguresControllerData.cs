using UnityEngine;

[CreateAssetMenu(fileName = "FiguresControllerData", menuName = "VoxelTetris/FiguresControllerData")]
public class FiguresControllerData : ScriptableObject
{
    public GameObject[] FigurePrefabs => _figurePrefabs;
    public FigureCube[] FigureCubes => _figureCubes;
    public Material[] FigureColors => _figureColors;
    public float SpawnOffsetY => _spawnOffsetY;
    public float TimeToDropFigure => _timeToDropFigure;
        
    [SerializeField] private GameObject[] _figurePrefabs;
    [SerializeField] private FigureCube[] _figureCubes;
    [SerializeField] private Material[] _figureColors;
    [SerializeField] private float _spawnOffsetY;
    [Space]
    [SerializeField] private float _timeToDropFigure;
}