using UnityEngine;

public class FigureSpawner
{
    private GameObject[] _figuresPrefabs;
    private Vector3Int _spawnPosition;
    private Transform _parentTransform;
    private FigureCube _cube;
    private Material[] _colors;

    private int _previousColorIndex = 0;
    
    public FigureSpawner(GameObject[] figuresPrefabs, Vector3Int spawnPosition, 
        Transform parentTransform, FigureCube cube, Material[] colors)
    {
        _figuresPrefabs = figuresPrefabs;
        _spawnPosition = spawnPosition;
        _parentTransform = parentTransform;
        _cube = cube;
        _colors = colors;
    }

    public FigureController SpawnFigure()
    {
        GameObject figure = GameObject.Instantiate(
            GetRandomFigurePrefab(),
            _spawnPosition,
            Quaternion.identity,
            _parentTransform);
        
        _previousColorIndex = (_previousColorIndex + 1) % _colors.Length;
        Material color = _colors[_previousColorIndex];
        foreach (FigurePartController figurePartController in figure.GetComponentsInChildren<FigurePartController>())
        {
            figurePartController.Initialize(_cube, color);
        }
        
        return figure.GetComponent<FigureController>();
    }

    private GameObject GetRandomFigurePrefab()
    {
        return _figuresPrefabs[Random.Range(0, _figuresPrefabs.Length)];
    }
}