using System.Text;

public class GridModel
{
    public GridPlaneModel[] Grid { get; private set; }
    public int LimitY { get; private set; }
    public int Width => Grid[0].Width;
    public int Height => Grid.Length;
    public int Depth => Grid[0].Depth;


    public GridModel(int width, int height, int depth, int limitY)
    {
        Grid = new GridPlaneModel[height];
        for (int i = 0; i < height; i++)
        {
            Grid[i] = new GridPlaneModel(width, depth);
        }
        if(limitY < height)
        {
            LimitY = limitY;
        }
        else
        {
            UnityEngine.Debug.LogError($"LIMIT БОЛЬШЕ HEIGHT: {limitY}vs{Height}");
        }
    }

    public FigurePartModel GetPart(UnityEngine.Vector3Int partPosition)
    {
        return Grid[partPosition.y].Plane[partPosition.x, partPosition.z];
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{Width}x{Height}({LimitY})x{Depth}");
        for (int y = Height - 1; y >= 0; y--)
        {
            stringBuilder.AppendLine(Grid[y].ToString());
        }
        return stringBuilder.ToString();
    }
}