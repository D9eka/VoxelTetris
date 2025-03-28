using System.Collections.Generic;
using UnityEngine;

public class FigureController : MonoBehaviour
{
    public FigurePartController[] Parts => GetComponentsInChildren<FigurePartController>();

    public FigurePartController Center => _center;
    
    [SerializeField] private FigurePartController _center;

    public FigureModel Model { get; private set; }

    private void Start()
    {
        CreateModel();
    }

    [ContextMenu("PrintModel")]
    public void PrintModel()
    {
        Debug.Log(Model.ToString());
    }

    public void Move(Vector3Int directionInt)
    {
        transform.position += directionInt;
    }

    public void DeleteFigurePart(FigurePartController figurePart)
    {
        Model.Parts.Remove(figurePart.Model);
        Destroy(figurePart);
    }

    private void CreateModel()
    {
        Vector3Int centerPartPosition = Vector3Int.RoundToInt(_view.Center.transform.position);
        List<FigurePartModel> parts = new();
        FigurePartModel center = null;
        int minHeight = 999;
        int maxHeight = 0;
        foreach (FigurePartController part in _view.Parts)
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
        Model = new FigureModel(this, parts, center, height);
        //Debug.Log(Model.ToString());
    }
}

