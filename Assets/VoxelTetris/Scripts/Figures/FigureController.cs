using System.Collections.Generic;
using UnityEngine;

public class FigureController : MonoBehaviour
{
    [SerializeField] private FigureView _view;

    public FigureView View => _view;
    public FigureModel Model { get; private set; }

    public FigurePartModel Center => Model.Center;

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
        _view.transform.position += directionInt;
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

