using System;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private Vector3Int _grid;
    [SerializeField] private int _limitY;
    [SerializeField] private GridView _view;
    
    public GridView View => _view;
    public GridModel Model { get; private set; }

    public Action<FigureController> OnPlaceFigure;
    public Action<FigureController> OnClearPlane;
    public Action OnReachLimit;

    public static GridController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        Model = new GridModel(_grid.x, _grid.y, _grid.z, _limitY);
        _view.GenerateGrid(_grid.x, _limitY, _grid.z);
    }

    [ContextMenu("PrintModel")]
    public void PrintModel()
    {
        Debug.Log(Model.ToString());
    }

    private bool IsInGrid(Vector3Int figurePart)
    {
        return figurePart.x >= 0 && figurePart.x < Model.Width &&
               figurePart.y >= 0 && figurePart.y < Model.Height &&
               figurePart.z >= 0 && figurePart.z < Model.Depth;
    }

    public bool TryMoveFigure(FigureModel figureModel, Vector3Int directionInt)
    {
        List<FigurePartModel> figureParts = figureModel.Parts;
    
        for (int i = 0; i < figureParts.Count; i++)
        {
            Vector3Int newPlacePosition = figureParts[i].Position + directionInt;
            if (!IsInGrid(newPlacePosition))
            {
                Debug.Log($"НЕ В ГРИДЕ: {figureParts[i].Position} -> {newPlacePosition}");
                return false;
            }
            FigurePartModel newPlace = Model.GetPart(newPlacePosition);
            if (newPlace != null && newPlace.Parent != figureModel.Parts[0].Parent)
            {
                Debug.Log($"НОВАЯ ПОЗИЦИЯ ЗАНЯТА: {newPlacePosition}");
                return false;
            }
        }
        
        for (int i = 0; i < figureParts.Count; i++)
        {
            TryMoveFigurePart(figureParts[i], directionInt);
        }
        OnPlaceFigure?.Invoke(figureModel.Parts[0].Parent);
        Check();
        return true;
    }

    private bool TryMoveFigurePart(FigurePartModel figurePartModel, Vector3Int directionInt)
    {
        FigurePartModel oldPlace = Model.GetPart(figurePartModel.Position);
        if (oldPlace != null)
        {
            Model.Grid[figurePartModel.Position.y].Remove(
                new Vector2Int(figurePartModel.Position.x, figurePartModel.Position.z));
        }
        
        Vector3Int newPlacePosition = figurePartModel.Position + directionInt;
        Model.Grid[newPlacePosition.y].Add(figurePartModel, new Vector2Int(newPlacePosition.x, newPlacePosition.z));
        figurePartModel.SetPosition(newPlacePosition);
        return true;
    }

    private void Check()
    {
        bool clearPlane = false;
        foreach(GridPlaneModel planeModel in Model.Grid)
        {
            if (planeModel.IsFull())
            {
                OnClearPlane?.Invoke(planeModel.LastFigure);
                planeModel.Clear();
                clearPlane = true;
            }
            else
            { 
                if (clearPlane)
                {
                    FiguresController.Instance.AddFigures(planeModel.Figures);
                }
            }
        }
        if (HaveFiguresInLimit())
        {
            OnReachLimit?.Invoke();
        }
    }    

    private bool HaveFiguresInLimit()
    {
        return Model.Grid[Model.LimitY].Figures.Count > 0;
    }
}