using System.Collections.Generic;
using UnityEngine;

public class FiguresController : MonoBehaviour
{
    [SerializeField] private FiguresControllerData _data;
    
    private FigureSpawner _figureSpawner;

    private List<FigureController> _figuresToMove;
    private FigureController _activeFigure;

    private float _timeFromLastMove;
    private bool _active;

    private void Awake()
    {
        _figuresToMove = new();
    }

    private void Start()
    {
        GridModel gridModel = ServiceLocator.Instance.GridController.Model;
        Vector3Int spawnPosition = Vector3Int.RoundToInt(
            new Vector3(gridModel.Width / 2f, gridModel.Height - _data.SpawnOffsetY, gridModel.Depth / 2f));
        _figureSpawner = new FigureSpawner(_data.FigurePrefabs, spawnPosition, transform, _data.FigureColors);
        
        ServiceLocator.Instance.GridController.OnReachLimit += OnReachLimit;
    }

    private void Update()
    {
        if (!_active)
        {
            return;
        }
        
        if (_activeFigure is null)
        {
            FigureController newActiveFigure = _figureSpawner.SpawnFigure();
            AddFigure(newActiveFigure, true);
        }

        MoveFigures();
    }

    [ContextMenu("StartSpawning")]
    public void StartSpawning()
    {
        _active = true;
    }

    public void AddFigures(IEnumerable<FigureController> figures)
    {
        foreach (FigureController figure in figures)
        {
            AddFigure(figure);
        }
    }

    public void AddFigure(FigureController figure, bool setActive = false)
    {
        _figuresToMove.Add(figure);
        if (setActive)
        {
            _activeFigure = figure;
        }
    }

    public void MoveToBottom()
    {
        Vector3Int directionInt = Vector3Int.down;
        while (TryMove(_activeFigure, directionInt))
        {
        }
        _activeFigure = null;
    }

    public void Move(Vector2 input)
    {
        float cameraAngle = CameraController.Instance.Angle;
        float angleRad = -cameraAngle * Mathf.Deg2Rad;
    
        float x = input.x;
        float z = input.y;
    
        float rotatedX = x * Mathf.Cos(angleRad) + z * Mathf.Sin(angleRad);
        float rotatedZ = -x * Mathf.Sin(angleRad) + z * Mathf.Cos(angleRad);
    
        Vector3Int directionInt = new Vector3Int(
            Mathf.RoundToInt(rotatedX),
            0,
            Mathf.RoundToInt(rotatedZ)
        );
    
        TryMove(_activeFigure, directionInt);
    }

    public void Rotate(Vector3 axis)
    {
        Quaternion rotation = Quaternion.Euler(axis * 90f);
        Vector3 centerPosition = _activeFigure.Model.Center.Position;

        for (int i = 0; i < _activeFigure.Model.Parts.Count; i++)
        {
            Vector3 relativePosition = _activeFigure.Model.Parts[i].Position - centerPosition;
            Vector3 rotatedPosition = rotation * relativePosition;
            Vector3Int newPosition = Vector3Int.RoundToInt(rotatedPosition + centerPosition);

            TryMove(_activeFigure, newPosition);
        }
    }

    public void RemoveFiguresPartAtPlane(IEnumerable<FigureController> figures, int planePosY)
    {
        foreach (FigureController figure in figures)
        {
            List<FigurePartModel> figureParts = figure.Model.Parts;
            for (int i = 0; i < figureParts.Count; i++)
            {
                if (figureParts[i].Position.y == planePosY)
                {
                    figure.DeleteFigurePart(figureParts[i].Controller);
                }
            }
        }
    }

    public void Reload()
    {
        foreach (FigureController figure in GetComponentsInChildren<FigureController>())
        {
            Destroy(figure.gameObject);
        }
        ServiceLocator.Instance.GridController.ClearPlanes();
        _figuresToMove = new List<FigureController>();
        _activeFigure = null;
        _active = true;
    }

    private void OnReachLimit()
    {
        _active = false;
        _activeFigure = null;
        Debug.Log("Reach Limit");
    }

    private void MoveFigures()
    {
        if (_timeFromLastMove < _data.TimeToDropFigure)
        {
            _timeFromLastMove += Time.deltaTime;
            return;
        }

        for (int i = _figuresToMove.Count - 1; i >= 0; i--)
        {
            if (!TryMove(_figuresToMove[i], Vector3Int.down))
            {
                //Debug.Log($"ФИГУРУ {_figuresToMove[i]} ({i}, {_figuresToMove[i] == _activeFigure}) БОЛЬШЕ НЕ ПОДВИНУТЬ");
                if (_figuresToMove[i] == _activeFigure)
                {
                    _activeFigure = null;
                }
                _figuresToMove.RemoveAt(i);
            }
            else
            {
                //Debug.Log($"ДВИНУЛ ФИГУРУ {_figuresToMove[i]} ({i}, {_figuresToMove[i] == _activeFigure}) НА {Vector3Int.down}");
            }
        }
        _timeFromLastMove = 0;
    }
    
    private bool TryMove(FigureController figure, Vector3Int directionInt)
    {
        if (ServiceLocator.Instance.GridController.TryMoveFigure(figure.Model, directionInt))
        {
            figure.Move(directionInt);
            return true;
        }
        return false;
    }
}