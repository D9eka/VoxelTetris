using DG.Tweening;
using UnityEngine;

public class FigureDropper {
    private float slowDelay = 1.5f;
    private float mediumDelay = 1f;
    private float fastDelay = 0.5f;
    
    private float defaultDelay;
    private float currentDelay;

    private float _dropTimer;
    private bool isSlowFallActive;
    private float timeModifier;
    
    private FigureController _figureController;
    private FigureMover _figureMover;
    private Board _board;
    

    public FigureDropper(FigureController figureController, FigureMover figureMover, Board board, ScoreManager scoreManager) 
    {
        _figureController = figureController;
        _figureMover = figureMover;
        _board = board;
        
        scoreManager.OnScoreStateChanged += OnScoreStateChanged;
        OnScoreStateChanged(scoreManager.ScoreState);
        UpdateDropSpeed();
    }

    public void Update(float deltaTime)
    {
        _dropTimer += deltaTime;
        if (_dropTimer >= currentDelay) 
        {
            Drop();
            _dropTimer = 0f;
        }
    }

    private void OnScoreStateChanged(ScoreState scoreState)
    {
        defaultDelay = scoreState switch
        {
            ScoreState.Low => slowDelay,
            ScoreState.Medium => mediumDelay,
            ScoreState.High => fastDelay,
            _ => defaultDelay
        };
        UpdateDropSpeed();
    }

    public void UpdateDropSpeed() 
    {
        currentDelay = defaultDelay;
        if (isSlowFallActive) {
            currentDelay *= timeModifier;
        }
    }

    public void ActivateSlowFall(float timeModifier) 
    {
        if (isSlowFallActive)
        {
            return;
        }
        this.timeModifier = timeModifier;
        isSlowFallActive = true;
        UpdateDropSpeed();
    }

    public void DeactivateSlowFall()
    {
        isSlowFallActive = false;
        UpdateDropSpeed();
    }
    
    public void Drop() 
    {
        Figure activeFigure = _figureController.ActiveFigure;
        if (!CanDrop(activeFigure))
        {
            return;
        }

        if (_figureMover.CanMove(Vector3Int.down)) 
        {
            _figureMover.Move(Vector3Int.down);
        } 
        else 
        {
            _figureController.LockFigure(activeFigure);
        }
    }

    public void HardDrop() 
    {
        Figure activeFigure = _figureController.ActiveFigure;
        if (!CanDrop(activeFigure))
        {
            return;
        }

        Figure figure = activeFigure;
        _figureController.IsLocking = true;

        int maxDist = int.MaxValue;
        foreach (Transform cube in figure.Parts) 
        {
            int dist = 0;
            Vector3Int pos = Vector3Int.RoundToInt(cube.position);
            while (_board.IsInside(pos + Vector3Int.down * (dist + 1)) &&
                   !_board.IsOccupied(pos + Vector3Int.down * (dist + 1))) 
            {
                dist++;
            }
            maxDist = Mathf.Min(maxDist, dist);
        }

        Vector3 target = figure.transform.position + Vector3.down * maxDist;
        figure.transform.DOMove(target, 0.1f).SetLink(figure.gameObject).OnComplete(() => 
        {
            _figureController.LockFigure(figure);
        });
    }

    private bool CanDrop(Figure activeFigure)
    {
        return _figureController.Active && activeFigure != null && !_figureController.IsLocking;
    }
}