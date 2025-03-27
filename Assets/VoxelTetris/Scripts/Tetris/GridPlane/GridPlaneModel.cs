using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GridPlaneModel
{
    public FigurePartModel[,] Plane { get; private set; }
    public HashSet<FigureController> Figures { get; private set; }
    public FigureController LastFigure { get; private set; }

    public int Width { get; private set; }
    public int Depth { get; private set; }

    public GridPlaneModel(int width, int depth)
    {
        Width = width;
        Depth = depth;
        Plane = new FigurePartModel[width, depth];
        Figures = new();
    }

    public void Add(FigurePartModel figurePartModel, Vector2Int position)
    {
        Plane[position.x, position.y] = figurePartModel;
        FigureController figurePartParent = figurePartModel.Parent;
        Figures.Add(figurePartParent);
        LastFigure = figurePartParent;
    }

    public void Remove(Vector2Int position)
    {
        FigureController figurePartParent = Plane[position.x, position.y].Parent;
        Figures.Remove(figurePartParent);
        if (LastFigure == figurePartParent)
        {
            LastFigure = null;
        }
        Plane[position.x, position.y] = null;
    }

    public bool IsFull()
    {
        for (int i = 0; i < Plane.GetLength(0); i++)
        {
            for (int j = 0; j < Plane.GetLength(1); j++)
            {
                if (Plane[i, j] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Clear()
    {
        Plane = new FigurePartModel[Width, Depth];
        Figures = new();
        LastFigure = null;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                stringBuilder.Append(Plane[x, z].ToString());
                stringBuilder.Append(' ');
            }
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }
}