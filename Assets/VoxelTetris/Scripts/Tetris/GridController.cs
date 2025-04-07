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

    public Action<FigureController> OnPlaceFigure;
    public Action<FigureController> OnClearPlane;
    public Action OnReachLimit;

    private void Awake()
    {
        Model = new GridModel(_grid.x, _grid.y, _grid.z, _limitY);
        _view.GenerateGrid(_grid.x, _limitY, _grid.z);
    }

    private void Start()
    {
        ServiceLocator.Instance.LevelController.EndGame += OnEndGame;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.LevelController.EndGame -= OnEndGame;
    }

    [ContextMenu("PrintModel")]
    public void PrintModel()
    {
        Debug.Log(Model.ToString());
    }

    public bool TryMoveFigure(FigureModel figureModel, Vector3Int directionInt)
    {
        List<FigurePartModel> figureParts = figureModel.Parts;
        if (figureParts.Count == 0)
        {
            return false;
        }
    
        for (int i = 0; i < figureParts.Count; i++)
        {
            Vector3Int newPlacePosition = figureParts[i].Position + directionInt;
            if (!IsInGrid(newPlacePosition))
            {
                //Debug.Log($"НЕ В ГРИДЕ: {figureParts[i].Position} -> {newPlacePosition}");
                OnPlaceFigure?.Invoke(figureModel.Parts[0].Parent);
                CheckForFullPlanes();
                return false;
            }
            FigurePartModel newPlace = Model.GetPart(newPlacePosition);
            if (newPlace != null && newPlace.Parent != figureModel.Parts[0].Parent)
            {
                //Debug.Log($"НОВАЯ ПОЗИЦИЯ ЗАНЯТА: {newPlacePosition}");
                OnPlaceFigure?.Invoke(figureModel.Parts[0].Parent);
                CheckForFullPlanes();
                CheckForFiguresInLimit();
                return false;
            }
        }
        
        for (int i = 0; i < figureParts.Count; i++)
        {
            TryMoveFigurePart(figureParts[i], directionInt);
        }
        return true;
    }

    private void ClearPlanes()
    {
        foreach (GridPlaneModel gridPlaneModel in Model.Grid)
        {
            gridPlaneModel.Clear();
        }
        foreach (FigureController figure in GetComponentsInChildren<FigureController>())
        {
            Destroy(figure.gameObject);
        }
    }

    private bool IsInGrid(Vector3Int figurePart)
    {
        return figurePart.x >= 0 && figurePart.x < Model.Width &&
               figurePart.y >= 0 && figurePart.y < Model.Height &&
               figurePart.z >= 0 && figurePart.z < Model.Depth;
    }
    
    public bool CanPlaceFigureAtPositions(FigureModel figureModel, List<Vector3Int> newPositions)
    {
        for (int i = 0; i < newPositions.Count; i++)
        {
            Vector3Int newPos = newPositions[i];
            // Проверяем, находится ли позиция в пределах сетки
            if (!IsInGrid(newPos))
            {
                return false;
            }
            // Проверяем, свободна ли ячейка или занята частью другой фигуры
            FigurePartModel partAtNewPos = Model.GetPart(newPos);
            if (partAtNewPos != null && partAtNewPos.Parent != figureModel.Controller)
            {
                return false;
            }
        }
        return true;
    }
    
    public void UpdateFigurePositions(FigureModel figureModel, List<Vector3Int> oldPositions, List<Vector3Int> newPositions)
    {
        // Удаляем старые позиции из сетки
        for (int i = 0; i < oldPositions.Count; i++)
        {
            Vector3Int oldPos = oldPositions[i];
            Model.Grid[oldPos.y].Remove(new Vector2Int(oldPos.x, oldPos.z));
        }
        // Добавляем новые позиции в сетку
        for (int i = 0; i < newPositions.Count; i++)
        {
            Vector3Int newPos = newPositions[i];
            FigurePartModel part = figureModel.Parts[i];
            Model.Grid[newPos.y].Add(part, new Vector2Int(newPos.x, newPos.z));
            part.SetPosition(newPos);
        }
    }
    
    private void OnEndGame()
    {
        ClearPlanes();
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

    private void CheckForFullPlanes()
    {
        FiguresController figuresController = ServiceLocator.Instance.FiguresController;
        
        bool clearPlane = false;
        for (int i = 0; i < Model.Grid.Length; i++)
        {
            if (Model.Grid[i].IsFull())
            {
                Debug.Log($"CLEAR PLANE: {i}");
                OnClearPlane?.Invoke(Model.Grid[i].LastFigure);
                figuresController.RemoveFiguresPartAtPlane(Model.Grid[i].Figures, i);
                Model.Grid[i].Clear();
                clearPlane = true;
            }
            else
            { 
                if (clearPlane)
                {
                    figuresController.AddFigures(Model.Grid[i].Figures);
                }
            }
        }
    }

    private void CheckForFiguresInLimit()
    {
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