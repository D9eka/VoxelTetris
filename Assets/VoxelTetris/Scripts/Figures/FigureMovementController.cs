using System.Collections.Generic;
using UnityEngine;

public class FigureMovementController : MonoBehaviour
{
    [SerializeField] private float _timeToDropFigure;

    private List<FigureController> _figuresToMove;
    private FigureController _activeFigure;

    private float _timeFromLastMove;

    public static FigureMovementController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _figuresToMove = new();
    }

    private void Update()
    {
        if (_activeFigure == null)
        {
            _activeFigure = FigureSpawner.Instance.SpawnFigure();
            AddFigure(_activeFigure, true);
        }

        MoveFiguresToMove();
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
    }

    public void MoveToBottom()
    {
        Vector3Int directionInt = Vector3Int.down;
        while (TryMove(_activeFigure, directionInt))
        {
            continue;
        }
    }

    public void Move(Vector2 direction)
    {
        Vector3Int directionInt = new Vector3Int(Mathf.RoundToInt(direction.y), 0, -Mathf.RoundToInt(direction.x));

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

    private bool TryMove(FigureController figure, Vector3Int directionInt)
    {
        if (GridController.Instance.TryMoveFigure(figure.Model, directionInt))
        {
            for (int i = 0; i < figure.Model.Parts.Count; i++)
            {
                figure.Model.Parts[i].SetPosition(figure.Model.Parts[i].Position + directionInt);
            }

            figure.View.transform.position += directionInt;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MoveFiguresToMove()
    {
        if (_timeFromLastMove < _timeToDropFigure)
        {
            _timeFromLastMove += Time.deltaTime;
            return;
        }

        if (_figuresToMove == null || _figuresToMove.Count == 0)
        {
            return;
        }

        for (int i = _figuresToMove.Count - 1; i >= 0; i--)
        {
            if (!TryMove(_figuresToMove[i], Vector3Int.down))
            {
                if (_figuresToMove[i] == _activeFigure)
                {
                    _activeFigure = null;
                }
                _figuresToMove.RemoveAt(i);
            }
        }
        _timeFromLastMove = 0;
    }
}