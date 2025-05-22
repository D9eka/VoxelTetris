using DG.Tweening;
using UnityEngine;

public class FigureRotator
{
    private FigureController _figureController;
    private Board _board;

    public FigureRotator(FigureController figureController, Board board)
    {
        _figureController = figureController;
        _board = board;
    }
    
    public void Rotate(Vector3 axis)
    {
        Figure activeFigure = _figureController.ActiveFigure;
        
        if (activeFigure == null || _figureController.IsLocking || !CanRotate(axis))
        {
            return;
        }

        Quaternion deltaRot = Quaternion.Euler(axis * 90f);
        Vector3 pivotPos = activeFigure.Center.position;
        foreach (Transform cube in activeFigure.Parts) 
        {
            Vector3 relativePosition = cube.position - pivotPos;
            Vector3 rotatedPosition = deltaRot * relativePosition;
            Vector3Int newPosition = Vector3Int.RoundToInt(rotatedPosition + pivotPos);

            DOTween.Complete(cube);
            cube.DOMove(newPosition, 0.3f).SetLink(activeFigure.gameObject);
        }
    }

    public bool CanRotate(Vector3 axis) 
    {
        Figure activeFigure = _figureController.ActiveFigure;
        
        if (activeFigure == null)
        {
            return false;
        }

        Quaternion deltaRot = Quaternion.Euler(axis * 90f);
        Vector3 pivotPos = activeFigure.Center.position;

        foreach (Transform cube in activeFigure.Parts) 
        {
            Vector3 relativePosition = cube.position - pivotPos;
            Vector3 rotatedPosition = deltaRot * relativePosition;
            Vector3Int newPosition = Vector3Int.RoundToInt(rotatedPosition + pivotPos);

            if (!_board.IsInside(newPosition) || _board.IsOccupied(newPosition))
            {
                return false;
            }
        }
        return true;
    }
}