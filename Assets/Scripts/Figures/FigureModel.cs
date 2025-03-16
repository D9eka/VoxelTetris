using System.Collections.Generic;
using System.Text;

public class FigureModel
{
    public FigureController Controller { get; set; }
    public List<FigurePartModel> Parts { get; private set; } 
    public FigurePartModel Center { get; private set; }
    public int Height { get; private set; }

    public FigureModel(FigureController controller, List<FigurePartModel> parts, FigurePartModel center, int height)
    {
        Controller = controller;
        Parts = parts;
        Center = center;
        Height = height;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        foreach (FigurePartModel part in Parts)
        {
            stringBuilder.Append(part.ToString());
            if (part == Center)
            {
                stringBuilder.Append(" CENTER");
            }
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }
}