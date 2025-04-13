using UnityEngine;
using UnityEngine.Serialization;

public class FigureController : MonoBehaviour
{
    public FigurePartController[] Parts => GetComponentsInChildren<FigurePartController>();

    public FigureType Type => _type;
    public FigurePartController Center => _center;
    public FigureModel Model { get; private set; } 
    
    [SerializeField] private FigureType _type;
    [SerializeField] private FigurePartController _center;
    
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
        Destroy(figurePart.gameObject);
    }

    private void CreateModel()
    {
        Model = FigureModelFactory.CreateFigureModel(this);
    }
}

