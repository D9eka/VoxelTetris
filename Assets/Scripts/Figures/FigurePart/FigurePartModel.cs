using UnityEngine;

public class FigurePartModel
{
    public FigureController Parent { get; private set; }
    public Vector3Int Position { get; private set; }

    public FigurePartModel(FigureController parent, Vector3Int position)
    {
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