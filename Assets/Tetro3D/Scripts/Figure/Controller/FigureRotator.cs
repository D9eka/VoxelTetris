using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FigureRotator
{
    private FigureController _figureController;
    private Board _board;

    private float _timeFromLastRotation;
    private const float ROTATION_DELAY = 0.3f;

    public FigureRotator(FigureController figureController, Board board)
    {
        _figureController = figureController;
        _board = board;
    }
    
    public void Rotate(Vector3 axis)
    {
        Figure activeFigure = _figureController.ActiveFigure;
        
        if (!CanRotate(axis, out Vector3 pivotPos))
            return;

        bool needToUnlock = !_figureController.IsLocking;
        _figureController.IsLocking = true;

        Quaternion deltaRot = Quaternion.Euler(axis * 90f);
        Sequence rotationSequence = DOTween.Sequence();
        
        foreach (Transform cube in activeFigure.Parts) 
        {
            Vector3 relativePosition = cube.position - pivotPos;
            Vector3 rotatedPosition = deltaRot * relativePosition;
            Vector3Int newPosition = Vector3Int.RoundToInt(rotatedPosition + pivotPos);

            DOTween.Complete(cube);
            rotationSequence.Join(
                cube.DOMove(newPosition, 0.3f).SetLink(activeFigure.gameObject)
            );
        }

        rotationSequence.OnComplete(() => 
        {
            if (needToUnlock)
                _figureController.IsLocking = false;
            
            _timeFromLastRotation = Time.time;
        });
    }

    public bool CanRotate(Vector3 axis, out Vector3 pivotPos) 
    {
        Figure activeFigure = _figureController.ActiveFigure;
        pivotPos = Vector3.zero;

        if (activeFigure == null || _figureController.IsLocking ||
            Time.time - _timeFromLastRotation < ROTATION_DELAY)
        {
            return false;
        }

        Quaternion deltaRot = Quaternion.Euler(axis * 90f);
        List<Transform> parts = activeFigure.Parts;

        if (activeFigure.Center != null && 
            CheckPivot(activeFigure.Center.position, parts, deltaRot))
        {
            pivotPos = activeFigure.Center.position;
            return true;
        }

        foreach (Transform pivotTransform in parts) 
        {
            Vector3 checkPos = pivotTransform.position;
            if (CheckPivot(checkPos, parts, deltaRot))
            {
                pivotPos = checkPos;
                return true;
            }
        }
        return false;
    }

    private bool CheckPivot(Vector3 pivotPos, List<Transform> parts, Quaternion deltaRot)
    {
        foreach (Transform cube in parts) 
        {
            Vector3 relativePos = cube.position - pivotPos;
            Vector3Int newPos = Vector3Int.RoundToInt(deltaRot * relativePos + pivotPos);

            if (!_board.IsInside(newPos) || _board.IsOccupied(newPos))
            {
                return false;
            }
        }
        return true;
    }
}