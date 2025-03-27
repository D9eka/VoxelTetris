using UnityEngine;

public class FigurePartModel
{
    public FigurePartController Controller { get; private set; }
    public FigureController Parent { get; private set; }
    public Vector3Int Position { get; private set; }

    public FigurePartModel(FigurePartController controller, FigureController parent, Vector3Int position)
    {
        Controller = controller;
        Parent = parent;
        Position = position;
    }

    public void SetPosition(Vector3Int position)
    {
        Position = position; 
    }

    public override string ToString()
    {
        return Position.ToString();
    }
}