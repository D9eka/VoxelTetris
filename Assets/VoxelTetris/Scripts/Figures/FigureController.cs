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
        Model = FigureModelFactory.CreateFigureModel(this);
    }
}

