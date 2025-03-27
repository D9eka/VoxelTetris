using System;
using UnityEngine;

public class GridView : MonoBehaviour
{
    public Vector3 Center { get; private set; }

    [SerializeField] private GameObject _gridBlockPrefab;

    private GameObject _platform;

    private GameObject _forwardWall;
    private GameObject _leftWall;
    private GameObject _rightWall;
    private GameObject _backwardWall;

    private GameObject _leftUpCorner;
    private GameObject _rightUpCorner;
    private GameObject _leftDownCorner;
    private GameObject _rightDownCorner;

    private void OnDrawGizmos()
    {
        if (Center != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Center, 2f);
        }
    }

    public void GenerateGrid(int width, int height, int depth)
    {
        SpawnBlockParents();

        Center = new Vector3(width / 2f, height / 2f, depth / 2f);
        Debug.Log(Center);
        int platformYPos = -1;

        GeneratePlatform(platformYPos, width, depth);

        GenerateXWall(platformYPos, -1, height, depth, _leftWall.transform);
        GenerateXWall(platformYPos, width, height, depth, _rightWall.transform);

        GenerateZWall(platformYPos, -1, width, height, _backwardWall.transform);
        GenerateZWall(platformYPos, depth, width, height, _forwardWall.transform);

        GenerateCorner(platformYPos, -1, height, -1, _leftDownCorner.transform);
        GenerateCorner(platformYPos, -1, height, depth, _leftUpCorner.transform);
        GenerateCorner(platformYPos, width, height, -1, _rightDownCorner.transform);
        GenerateCorner(platformYPos, width, height, depth, _rightUpCorner.transform);
    }

    public void HideWall(GameObject wall)
    {
        ChangeWallActiveState(wall, false);
    }

    private void SpawnBlockParents()
    {
        _platform = SpawnBlockParent(nameof(_platform));

        _forwardWall = SpawnBlockParent(nameof(_forwardWall));
        _leftWall = SpawnBlockParent(nameof(_leftWall));
        _rightWall = SpawnBlockParent(nameof(_rightWall));
        _backwardWall = SpawnBlockParent(nameof(_backwardWall));

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

    private void GeneratePlatform(int y, int width, int depth)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                InstantiateBlock(new Vector3(x, y, z), _platform.transform);
            }
        }
    }

    private void GenerateXWall(int platformYPos, int x, int height, int depth, Transform parent)
    {
        for (int y = height - 1; y >= platformYPos; y--)
        {
            for (int z = 0; z < depth; z++)
            {
                InstantiateBlock(new Vector3(x, y, z), parent);
            }
        } 
    }

    private void GenerateZWall(int platformYPos, int z, int width, int height, Transform parent)
    {
        for (int y = height - 1; y >= platformYPos; y--)
        {
            for (int x = 0; x < width; x++)
            {
                InstantiateBlock(new Vector3(x, y, z), parent);
            }
        }
    }

    private void GenerateCorner(int platformYPos, int x, int height, int z, Transform parent)
    {
        for (int y = height - 1; y >= platformYPos; y--)
        {
            InstantiateBlock(new Vector3(x, y, z), parent);
        }
    }

    private void InstantiateBlock(Vector3 position, Transform parent)
    {
        Instantiate(_gridBlockPrefab, position, Quaternion.identity, parent);
    }

    private void ChangeWallActiveState(GameObject wall, bool state)
    {
        bool previousState = wall.activeSelf;
        wall.SetActive(state);
        if (wall == _forwardWall)
        {
            _rightUpCorner.SetActive(state);
            _leftDownCorner.SetActive(state);
        }
        else if (wall == _leftWall)
        {
            _leftUpCorner.SetActive(state);
            _leftDownCorner.SetActive(state);
        }
        else if (wall == _rightWall)
        {
            _rightUpCorner.SetActive(state);
            _rightDownCorner.SetActive(state);
        }
        else if (wall == _backwardWall)
        {
            _leftUpCorner.SetActive(state);
            _rightDownCorner.SetActive(state);
        }
        else
        {
            wall.SetActive(previousState);
            Debug.LogError($"{wall} НЕ СТЕНА");
        }
    }
}