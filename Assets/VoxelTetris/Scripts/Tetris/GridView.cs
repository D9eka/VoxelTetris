using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GridView : MonoBehaviour
{
    public Vector3 Center { get; private set; }

    [SerializeField] private GameObject _gridXLinePrefab;
    [SerializeField] private GameObject _gridYLinePrefab;
    [SerializeField] private GameObject _gridZLinePrefab;
    [SerializeField] private GameObject _gridCornerPrefab;
    [SerializeField] private GameObject _gridPlatformBlock;
    [SerializeField] private GridWall _initialWallToHide;

    private GameObject _platform;

    private GameObject _frontWall;
    private GameObject _leftWall;
    private GameObject _rightWall;
    private GameObject _backWall;

    private GameObject _leftUpCorner;
    private GameObject _rightUpCorner;
    private GameObject _leftDownCorner;
    private GameObject _rightDownCorner;
    
    private GridWall _hiddenWall;

    private float _offset;

    private void Start()
    {
        _hiddenWall = _initialWallToHide;
        //HideWall(_hiddenWall);
        
        ServiceLocator.Instance.InputManager.OnRotateCamera += OnRotateCamera;
    }

    public void GenerateGrid(int width, int height, int depth)
    {
        SpawnBlockParents();

        Center = new Vector3(width / 2f, height / 2f, depth / 2f);
        int platformYPos = -1;

        _offset = 0.45f;
        
        GenerateXWall(platformYPos, -1, height, depth, _offset, _leftWall.transform);
        GenerateXWall(platformYPos, width, height, depth, -_offset, _rightWall.transform);
        
        GenerateZWall(platformYPos, -1, width, height, _offset, _backWall.transform);
        GenerateZWall(platformYPos, depth, width, height, -_offset, _frontWall.transform);

        GenerateCorner(platformYPos, -1, height, -1, 0, new Vector3(_offset, 0, _offset), _leftDownCorner.transform);
        GenerateCorner(platformYPos, -1, height, depth, 90, new Vector3(_offset, 0, -_offset), _leftUpCorner.transform);
        GenerateCorner(platformYPos, width, height, -1, 270, new Vector3(-_offset, 0, _offset), _rightDownCorner.transform);
        GenerateCorner(platformYPos, width, height, depth, 180, new Vector3(-_offset, 0, -_offset), _rightUpCorner.transform);
    }

    public void HideWall(GridWall wall)
    {
        //ChangeWallActiveState(wall, false);
    }

    private void SpawnBlockParents()
    {
        _platform = SpawnBlockParent(nameof(_platform));

        _frontWall = SpawnBlockParent(nameof(_frontWall));
        _leftWall = SpawnBlockParent(nameof(_leftWall));
        _rightWall = SpawnBlockParent(nameof(_rightWall));
        _backWall = SpawnBlockParent(nameof(_backWall));

        _leftUpCorner = SpawnBlockParent(nameof(_leftUpCorner));
        _rightUpCorner = SpawnBlockParent(nameof(_rightUpCorner));
        _leftDownCorner = SpawnBlockParent(nameof(_leftDownCorner));
        _rightDownCorner = SpawnBlockParent(nameof(_rightDownCorner));
    }

    private GameObject SpawnBlockParent(string blockName)
    {
        GameObject go = new GameObject(blockName);
        go.transform.parent = transform;
        return go;
    }

    private void GenerateXWall(int platformYPos, int x, int height, int depth, float offset, Transform parent)
    {
        for (int z = 0; z < depth; z++)
        {
            InstantiateBlock(_gridZLinePrefab, new Vector3(x + offset, height - 1 + Math.Abs(offset), z), Quaternion.identity, parent);
            InstantiateBlock(_gridZLinePrefab, new Vector3(x + offset, platformYPos + Math.Abs(offset), z), Quaternion.identity, parent);
        }
    }

    private void GenerateZWall(int platformYPos, int z, int width, int height, float offset, Transform parent)
    {
        for (int x = 0; x < width; x++)
        {
            InstantiateBlock(_gridXLinePrefab, new Vector3(x, height - 1 + Math.Abs(offset), z+offset), Quaternion.identity, parent);
            InstantiateBlock(_gridXLinePrefab, new Vector3(x, platformYPos + Math.Abs(offset), z+offset), Quaternion.identity, parent);
        }
    }

    private void GenerateCorner(int platformYPos, int x, int height, int z, float angleY, Vector3 offset, Transform parent)
    {
        for (int y = height; y >= platformYPos; y--)
        {
            if (y == height)
            {
                InstantiateBlock(_gridCornerPrefab, new Vector3(x, y - (1 - _offset), z) + offset, Quaternion.Euler(0, angleY, 0), parent);
            }
            else if (y == platformYPos)
            {
                InstantiateBlock(_gridCornerPrefab, new Vector3(x, y + (1 - _offset) - 0.1f, z) + offset, Quaternion.Euler(0, angleY, 0), parent);
            }
            else
            {
                InstantiateBlock(_gridYLinePrefab, new Vector3(x, y, z) + offset, Quaternion.identity, parent);
            }
        }
    }

    private void InstantiateBlock(
        GameObject block, Vector3 position, Quaternion rotation, Transform parent)
    {
        Instantiate(block, position, rotation, parent);
    }

    private void ChangeWallActiveState(GridWall wall, bool state)
    {
        switch (wall)
        {
            case GridWall.Left:
                _leftWall.SetActive(state);
                _leftUpCorner.SetActive(state);
                _leftDownCorner.SetActive(state);
                break;
            case GridWall.Front:
                _frontWall.SetActive(state);
                _leftUpCorner.SetActive(state);
                _rightUpCorner.SetActive(state);
                break;
            case GridWall.Right:
                _rightWall.SetActive(state);
                _rightUpCorner.SetActive(state);
                _rightDownCorner.SetActive(state);
                break;
            case GridWall.Back:
                _backWall.SetActive(state);
                _leftDownCorner.SetActive(state);
                _rightDownCorner.SetActive(state);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnRotateCamera(float direction)
    {
        /*
        ChangeWallActiveState(_hiddenWall, true);
        _hiddenWall = NextWall(_hiddenWall, Mathf.RoundToInt(direction));
        ChangeWallActiveState(_hiddenWall, false);
        */
    }
    
    private GridWall NextWall(GridWall currentWall, int direction)
    {
        int wallCount = Enum.GetValues(typeof(GridWall)).Length;
        int currentIndex = (int)currentWall;
        int nextIndex = (currentIndex + direction + wallCount) % wallCount;
        return (GridWall)nextIndex;
    }
}