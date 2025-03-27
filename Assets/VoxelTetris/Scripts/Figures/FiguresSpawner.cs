using UnityEngine;

public class FigureSpawner
{
    private GameObject[] _figuresPrefabs;
    private Vector3Int _spawnPosition;
    private Transform _parentTransform;

    public FigureSpawner(GameObject[] figuresPrefabs, Vector3Int spawnPosition, Transform parentTransform)
    {
        _figuresPrefabs = figuresPrefabs;
        _spawnPosition = spawnPosition;
        _parentTransform = parentTransform;
    }

    public FigureController SpawnFigure()
    {
        Debug.Log(_spawnPosition.ToString());
        return GameObject.Instantiate(
            GetRandomFigurePrefab(),
            _spawnPosition, 
            Quaternion.identity, 
            _parentTransform).GetComponent<FigureController>();
    }

    private GameObject GetRandomFigurePrefab()
    {
        return _figuresPrefabs[Random.Range(0, _figuresPrefabs.Length - 1)];
    }
}