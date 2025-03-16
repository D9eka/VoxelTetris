using UnityEngine;

public class FigureSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _figuresPrefabs;
    [SerializeField] private int _offsetY;

    private Vector3Int _spawnPosition;

    public static FigureSpawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GridModel gridModel = GridController.Instance.Model;
        _spawnPosition = Vector3Int.RoundToInt(
            new Vector3(gridModel.Width / 2f, gridModel.Height - _offsetY, gridModel.Depth / 2f));
    }

    public FigureController SpawnFigure()
    {
        return Instantiate(
            GetRandomFigurePrefab(),
            _spawnPosition, 
            Quaternion.identity, 
            transform).GetComponent<FigureController>();
    }

    private GameObject GetRandomFigurePrefab()
    {
        return _figuresPrefabs[Random.Range(0, _figuresPrefabs.Length - 1)];
    }
}