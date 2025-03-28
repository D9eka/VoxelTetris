using System.Collections.Generic;
using UnityEngine;

public static class FigureModelFactory
{
    public static FigureModel CreateFigureModel(FigureController figureController)
    {
        Vector3Int centerPartPosition = Vector3Int.RoundToInt(figureController.Center.transform.position);
        List<FigurePartModel> parts = new();
        FigurePartModel center = null;
        int minHeight = 999;
        int maxHeight = 0;
        foreach (FigurePartController part in figureController.Parts)
        {
            parts.Add(part.Model);
            if (part.Model.Position == centerPartPosition)
            {
                center = part.Model;
            }
            minHeight = Mathf.Min(part.Model.Position.y, minHeight);
            maxHeight = Mathf.Max(part.Model.Position.y, maxHeight);
        }
        int height = maxHeight - minHeight + 1;
        return new FigureModel(figureController, parts, center, height);
    }
}