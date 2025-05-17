using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardView _view;
    [SerializeField] private Transform boardContainer; 
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 20;
    [SerializeField] private int figureLimitY = 14;
    [SerializeField] private int depth = 10;
    
    private Transform[,,] grid;
    
    private AbilityManager _abilityManager;
    
    public Vector3Int Size { get; private set; }
    public Action OnReachLimit; 
    
    private void Awake() 
    {
        if (figureLimitY > height)
        {
            Debug.LogError($"Board figure limit is bigger than height: {figureLimitY} > {height}");
        }
        
        Size = new Vector3Int(width, height, depth);
        grid = new Transform[width, height, depth];
        _view.GenerateGrid(width, figureLimitY, depth);
    }

    private void Start()
    {
        LevelController levelController = ServiceLocator.Instance.LevelController;
        levelController.EndGame += EndGame;
        
        _abilityManager = ServiceLocator.Instance.AbilityManager;
        _abilityManager.OnDeletePlaneAbility += DeleteFirstPlanes;
    }

    private void EndGame()
    {
        for (int y = 0; y < height; y++) 
        {
            DeleteLayer(y);
        }
    }

    public bool IsInside(Vector3Int pos) 
    {
        return pos.x >= 0 && pos.x < width && 
               pos.y >= 0 && pos.y < height && 
               pos.z >= 0 && pos.z < depth;
    }

    public bool IsOccupied(Vector3Int pos) 
    {
        return grid[pos.x, pos.y, pos.z] != null;
    }

    public void PlaceFigure(Figure figure) 
    {
        bool reachedLimit = false;
        foreach (Transform cube in figure.Parts) 
        {
            cube.SetParent(boardContainer);
            Vector3Int gridPos = Vector3Int.RoundToInt(cube.position);
            cube.position = gridPos;
            grid[gridPos.x, gridPos.y, gridPos.z] = cube;
            if (gridPos.y >= figureLimitY) reachedLimit = true;
        }
        Destroy(figure.gameObject);
        if (reachedLimit)
        {
            OnReachLimit?.Invoke();
        }
    }

    public void DeleteFirstPlanes(int planesCount)
    {
        planesCount = Mathf.Clamp(planesCount, 0, height);

        int nonEmptyLayers = 0;

        for (int i = 0; i < planesCount; i++)
        {
            if (IsLayerHaveCube(0))
                nonEmptyLayers++;

            DeleteLayers(new List<int> { 0 });

            ShiftLayersDown(new List<int> { 0 });
        }

        _abilityManager.NotifyLayersDeleted(nonEmptyLayers);
    }

    private bool IsLayerHaveCube(int y)
    {
        for (int x = 0; x < width; x++) 
        {
            for (int z = 0; z < depth; z++) 
            {
                if (grid[x, y, z] != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<int> ClearFullLayers() 
    {
        List<int> fullLayers = GetFullLayers();
        if (fullLayers.Count == 0)
        {
            return fullLayers;
        }

        DeleteLayers(fullLayers);
        ShiftLayersDown(fullLayers);

        return fullLayers;
    }
    
    private List<int> GetFullLayers() 
    {
        List<int> result = new();
        for (int y = 0; y < height; y++) 
        {
            if (IsLayerFull(y))
            {
                result.Add(y);
            }
        }
        return result;
    }

    private bool IsLayerFull(int y) 
    {
        for (int x = 0; x < width; x++) 
        {
            for (int z = 0; z < depth; z++) 
            {
                if (grid[x, y, z] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    private void DeleteLayers(List<int> layers) 
    {
        foreach (int y in layers) 
        {
            DeleteLayer(y);
        }
    }
    
    private void ShiftLayersDown(List<int> removedLayers)
    {
        int firstAffected = removedLayers.Min() + 1;
        for (int y = firstAffected; y < height; y++) 
        {
            int removedBelow = removedLayers.Count(layer => layer < y);
            if (removedBelow == 0)
            {
                continue;
            }

            for (int x = 0; x < width; x++) 
            {
                for (int z = 0; z < depth; z++) 
                {
                    Transform cell = grid[x, y, z];
                    if (cell == null)
                    {
                        continue;
                    }
                    grid[x, y - removedBelow, z] = cell;
                    grid[x, y, z] = null;

                    cell.DOKill(true);
                    Vector3 targetPos = cell.position + Vector3.down * removedBelow;
                    cell.DOMove(targetPos, 0.3f).SetLink(cell.gameObject).OnComplete(() => 
                    {
                        if (cell != null)
                        {
                            cell.position = targetPos;
                        }
                    });
                }
            }
        }
    }

    private void DeleteLayer(int y) 
    {
        for (int x = 0; x < width; x++) 
        {
            for (int z = 0; z < depth; z++) 
            {
                Transform cell = grid[x, y, z];
                if (cell == null)
                {
                    continue;
                }
                grid[x, y, z] = null;
                cell.DOKill(true);
                cell.DOScale(Vector3.zero, 0.3f)
                    .OnComplete(() => Destroy(cell.gameObject));
            }
        }
    }
}