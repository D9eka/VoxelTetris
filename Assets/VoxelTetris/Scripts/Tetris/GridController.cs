using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private Vector3Int _grid;
    [SerializeField] private int _limitY;
    [SerializeField] private GridView _view;
    
    public GridView View => _view;
    public GridModel Model { get; private set; }

    public Action<FigureController, int> OnPlaceFigure;
    public Action<FigureController, int> OnClearPlane;
    public Action OnReachLimit;

    private void Awake()
    {
        Model = new GridModel(_grid.x, _grid.y, _grid.z, _limitY);
        _view.GenerateGrid(_grid.x, _limitY, _grid.z);
    }

    private void Start()
    {
        ServiceLocator.Instance.AbilityManager.OnDeletePlaneAbility += DeletePlaneAbility;
        ServiceLocator.Instance.LevelController.EndGame += OnEndGame;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.AbilityManager.OnDeletePlaneAbility -= DeletePlaneAbility;
        ServiceLocator.Instance.LevelController.EndGame -= OnEndGame;
    }

    [ContextMenu("PrintModel")]
    public void PrintModel() => Debug.Log(Model.ToString());

    public bool TryMoveFigure(FigureModel figureModel, Vector3Int directionInt)
    {
        List<FigurePartModel> figureParts = figureModel.Parts;
        if (figureParts.Count == 0)
        {
            return false;
        }

        bool shouldPlaceFigure = false;
        bool outOfBounds = false;
        int placementY = 0;

        foreach (var part in figureParts)
        {
            Vector3Int newPos = part.Position + directionInt;
        
            // Проверяем выход за вертикальные границы (только для движения вниз)
            if (directionInt.y < 0 && newPos.y < 0)
            {
                shouldPlaceFigure = true;
                placementY = part.Position.y;
                break;
            }
        
            // Проверяем горизонтальные/глубинные границы
            if (newPos.x < 0 || newPos.x >= Model.Width || 
                newPos.z < 0 || newPos.z >= Model.Depth)
            {
                outOfBounds = true;
                break;
            }

            FigurePartModel existingPart = Model.GetPart(newPos);
            if (existingPart != null && existingPart.Parent != figureModel.Controller)
            {
                shouldPlaceFigure = true;
                placementY = part.Position.y;
                break;
            }
        }

        if (outOfBounds)
        {
            return false;
        }

        if (shouldPlaceFigure)
        {
            OnPlaceFigure?.Invoke(figureModel.Controller, placementY);
            CheckForFullPlanes();
            CheckForFiguresInLimit();
            return false;
        }

        // Если все проверки пройдены - перемещаем фигуру
        figureParts.ForEach(part => MovePartInGrid(part, part.Position + directionInt));
        return true;
    }

    private void DeletePlaneAbility(int planesToDelete)
    {
        int deletedLayers = 0;
        int fullLayersCount = 0;
        var figuresController = ServiceLocator.Instance.FiguresController;

        for (int i = 0; i < planesToDelete; i++)
        {
            GridPlaneModel plane = Model.Grid[i];
            bool wasFull = plane.IsFull();
            figuresController.RemoveFiguresPartAtPlane(plane.Figures, i);
            plane.Clear();
            deletedLayers++;
            fullLayersCount += wasFull ? 1 : 0;
        }

        ServiceLocator.Instance.AbilityManager.NotifyLayersDeleted(deletedLayers, fullLayersCount);
    
        MoveFigures();
    }

    private void ClearPlanes()
    {
        foreach (GridPlaneModel plane in Model.Grid)
        {
            plane.Clear();
        }
        foreach (FigureController figure in GetComponentsInChildren<FigureController>())
        {
            Destroy(figure.gameObject);
        }
    }

    private bool IsInGrid(Vector3Int position) => 
        position.x >= 0 && position.x < Model.Width &&
        position.y >= 0 && position.y < Model.Height &&
        position.z >= 0 && position.z < Model.Depth;

    public bool CanPlaceFigureAtPositions(FigureModel figureModel, List<Vector3Int> newPositions) => 
        AreAllPositionsValid(newPositions, figureModel);

    public void UpdateFigurePositions(FigureModel figureModel, List<Vector3Int> oldPositions, List<Vector3Int> newPositions)
    {
        RemovePartsFromGrid(oldPositions);
        AddPartsToGrid(figureModel, newPositions);
    }

    private bool AreAllPositionsValid(List<Vector3Int> positions, FigureModel figureModel)
    {
        foreach (var pos in positions)
        {
            if (!IsInGrid(pos))
            {
                return false;
            }
            FigurePartModel part = Model.GetPart(pos);
            if (part != null && part.Parent != figureModel.Controller)
            {
                return false;
            }
        }
        return true;
    }

    private void MovePartInGrid(FigurePartModel part, Vector3Int newPosition)
    {
        RemovePartFromGrid(part.Position);
        AddPartToGrid(part, newPosition);
    }

    private void RemovePartsFromGrid(List<Vector3Int> positions)
    {
        foreach (var pos in positions)
        {
            Model.Grid[pos.y].Remove(new Vector2Int(pos.x, pos.z));
        }
    }

    private void AddPartsToGrid(FigureModel figureModel, List<Vector3Int> newPositions)
    {
        for (int i = 0; i < newPositions.Count; i++)
        {
            Vector3Int pos = newPositions[i];
            FigurePartModel part = figureModel.Parts[i];
            Model.Grid[pos.y].Add(part, new Vector2Int(pos.x, pos.z));
            part.SetPosition(pos);
        }
    }

    private void RemovePartFromGrid(Vector3Int position) => 
        Model.Grid[position.y].Remove(new Vector2Int(position.x, position.z));

    private void AddPartToGrid(FigurePartModel part, Vector3Int newPosition)
    {
        Model.Grid[newPosition.y].Add(part, new Vector2Int(newPosition.x, newPosition.z));
        part.SetPosition(newPosition);
    }

    private void CheckForFullPlanes()
    {
        List<int> planesToClear = new List<int>();

        for (int i = 0; i < Model.Grid.Length; i++)
        {
            if (Model.Grid[i].IsFull())
            {
                planesToClear.Add(i);
            }
        }

        foreach (int planeIndex in planesToClear)
        {
            ClearPlane(planeIndex, Model.Grid[planeIndex].LastFigure);
        }

        if (planesToClear.Count > 0)
        {
            MoveFigures();
        }
    }

    private void ClearPlane(int planeIndex, FigureController lastFigure = null)
    {
        if (planeIndex < 0 || planeIndex >= Model.Grid.Length)
        {
            return;
        }
        
        var plane = Model.Grid[planeIndex];
        FiguresController figuresController = ServiceLocator.Instance.FiguresController;
        
        figuresController.RemoveFiguresPartAtPlane(plane.Figures, planeIndex);
        plane.Clear();
        OnClearPlane?.Invoke(lastFigure, planeIndex);
    }

    private void MoveFigures()
    {
        FiguresController figuresController = ServiceLocator.Instance.FiguresController;
        HashSet<FigureController> figuresToMove = new HashSet<FigureController>();
        for (int i = 0; i < Model.Height; i++)
        {
            foreach (FigureController figure in Model.Grid[i].Figures)
            {
                figuresToMove.Add(figure);
            }
        }

        figuresController.MoveFiguresToBottom(figuresToMove);
    }
    
    private void CheckForFiguresInLimit()
    {
        StartCoroutine(CheckForFiguresInLimitCoroutine());
    }

    private IEnumerator CheckForFiguresInLimitCoroutine()
    {
        yield return null;

        for (int i = Model.LimitY; i < Model.Height; i++)
        {
            if (HasNonMovingFigures(Model.Grid[Model.LimitY]))
            {
                OnReachLimit?.Invoke();
                yield return null;
            }
        }
    }

    private bool HasNonMovingFigures(GridPlaneModel plane)
    {
        foreach (var figure in plane.Figures)
        {
            if (!figure.IsMoving)
            {
                return true;
            }
        }
        return false;
    }

    private void OnEndGame() => ClearPlanes();
}