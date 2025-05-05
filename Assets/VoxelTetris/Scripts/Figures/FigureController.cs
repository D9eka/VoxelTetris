using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class FigureController : MonoBehaviour
{
    public FigurePartController[] Parts => GetComponentsInChildren<FigurePartController>();

    public FigureType Type => _type;
    public FigurePartController Center => _center;
    public FigureModel Model { get; private set; } 
    public bool IsMoving { get; set; }
    
    [SerializeField] private FigureType _type;
    [SerializeField] private FigurePartController _center;

    private TweenerCore<Vector3, Vector3, VectorOptions> _currentAnim;
    
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
        Vector3 newPosition = transform.position + directionInt;
        if (_currentAnim != null && _currentAnim.IsActive())
        {
            newPosition = _currentAnim.endValue + directionInt;
            _currentAnim.Kill();
        }
        
        _currentAnim = transform.DOMove(newPosition, 0.3f);
    }

    public void DeleteFigurePart(FigurePartController partController)
    {
        if (partController == null)
        {
            return;
        }

        var partModel = partController.Model;
        if (partModel != null)
        {
            Model.Parts.Remove(partModel);
        }

        if (partController.gameObject != null)
        {
            Destroy(partController.gameObject);
        }

        if (Model.Parts.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    private void CreateModel()
    {
        Model = FigureModelFactory.CreateFigureModel(this);
    }
}

