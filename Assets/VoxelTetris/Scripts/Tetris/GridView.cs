using UnityEngine;

public class GridView : MonoBehaviour
{
    public Vector3 Center { get; private set; }

    [SerializeField] private GameObject _gridBlockPrefab;

    private GameObject _platform;

    private GameObject _upWall;
    private GameObject _leftWall;
    private GameObject _rightWall;
    private GameObject _downWall;

    private GameObject _leftUpCorner;
    private GameObject _rightUpCorner;
    private GameObject _leftDownCorner;
    private GameObject _rightDownCorner;

    public void GenerateGrid(int width, int height, int depth)
    {
        SpawnBlockParents();

        Center = new Vector3(width / 2f, height / 2f, depth / 2f);

        GeneratePlatform(-1, width, depth);

        GenerateXWall(-1, height, depth, _leftWall.transform);
        GenerateXWall(width, height, depth, _rightWall.transform);

        GenerateZWall(-1, width, height, _downWall.transform);
        GenerateZWall(depth, width, height, _upWall.transform);

        GenerateCorner(-1, height, -1, _leftDownCorner.transform);
        GenerateCorner(-1, height, depth, _leftUpCorner.transform);
        GenerateCorner(width, height, -1, _rightDownCorner.transform);
        GenerateCorner(width, height, depth, _rightUpCorner.transform);
    }

    private void SpawnBlockParents()
    {
        _platform = SpawnBlockParent(nameof(_platform));

        _upWall = SpawnBlockParent(nameof(_upWall));
        _leftWall = SpawnBlockParent(nameof(_leftWall));
        _rightWall = SpawnBlockParent(nameof(_rightWall));
        _downWall = SpawnBlockParent(nameof(_downWall));

        _leftUpCorner = SpawnBlockParent(nameof(_leftUpCorner));
        _rightUpCorner = SpawnBlockParent(nameof(_rightUpCorner));
        _leftDownCorner = SpawnBlockParent(nameof(_leftDownCorner));
        _rightDownCorner = SpawnBlockParent(nameof(_rightDownCorner));
    }

    private GameObject SpawnBlockParent(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = transform;
        return go;
    }

    private void GeneratePlatform(int y, int width, int depth)
    {
        for (int x = -1; x <= width; x++)
        {
            for (int z = -1; z <= depth; z++)
            {
                InstantiateBlock(new Vector3(x, y, z), _platform.transform);
            }
        }
    }

    private void GenerateXWall(int x, int height, int depth, Transform parent)
    {
        for (int y = height - 1; y >= 0; y--)
        {
            for (int z = 0; z < depth; z++)
            {
                InstantiateBlock(new Vector3(x, y, z), parent);
            }
        } 
    }

    private void GenerateZWall(int z, int width, int height, Transform parent)
    {
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                InstantiateBlock(new Vector3(x, y, z), parent);
            }
        }
    }

    private void GenerateCorner(int x, int height, int z, Transform parent)
    {
        for (int y = height - 1; y >= 0; y--)
        {
            InstantiateBlock(new Vector3(x, y, z), parent);
        }
    }

    private void InstantiateBlock(Vector3 position, Transform parent)
    {
        Instantiate(_gridBlockPrefab, position, Quaternion.identity, parent);
    }
}