using DG.Tweening;
using UnityEngine;

public class FigureMover
{
    private FigureController _figureController;
    private Board _board;

    public FigureMover(FigureController figureController, Board board)
    {
        _figureController = figureController;
        _board = board;
    }
    
    public void Move(Vector3Int direction) 
    {
        Figure activeFigure = _figureController.ActiveFigure;

        if (activeFigure == null || _figureController.IsLocking || !CanMove(direction))
        {
            return;
        }
        
        Vector3 endValue = activeFigure.transform.position + direction;
        DOTween.Complete(activeFigure.transform);
        activeFigure.transform.DOMove(endValue, 0.1f).SetLink(activeFigure.gameObject);
    }

    public bool CanMove(Vector3Int direction)
    {
        Figure activeFigure = _figureController.ActiveFigure;
        
        foreach (Transform cube in activeFigure.Parts) 
        {
            Vector3Int newPos = Vector3Int.RoundToInt(cube.position + direction);
            if (!_board.IsInside(newPos) || _board.IsOccupied(newPos))
            {
                return false;
            }
        }
        return true;
    }
}