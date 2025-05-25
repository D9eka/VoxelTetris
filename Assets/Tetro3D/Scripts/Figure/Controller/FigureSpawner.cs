using UnityEngine;
using Random = UnityEngine.Random;

public class FigureSpawner : MonoBehaviour {
    [SerializeField] private GameObject[] _figurePrefabs;
    [SerializeField] private Cube[] _cubes;
    [SerializeField] private Material[] _materials;
    
    private Vector3Int _spawnPosition;
    private Cube _cube;
    
    private void Start()
    {
        Vector3Int boardSize = ServiceLocator.Instance.Board.Size;
        _spawnPosition = new Vector3Int(boardSize.x / 2, boardSize.y, boardSize.z / 2);
        
        _cube = _cubes[Random.Range(0, _cubes.Length)];
    }

    public Figure SpawnFigure() {
        int index = Random.Range(0, _figurePrefabs.Length);
        GameObject prefab = _figurePrefabs[index];
        int figureHeight = prefab.GetComponent<Figure>().Height;
        Vector3 spawnPosition = new Vector3Int(_spawnPosition.x, _spawnPosition.y - figureHeight, _spawnPosition.z);
        
        GameObject figureObj = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        Figure figure = figureObj.GetComponent<Figure>();
        Material randomMat = _materials[Random.Range(0, _materials.Length)];
        figure.Initialize(_cube, randomMat);
        return figure;
    }
}