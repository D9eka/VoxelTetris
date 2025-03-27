using UnityEngine;

public class FigurePartController : MonoBehaviour
{
    [SerializeField] private FigurePartView _view;

    public FigurePartModel Model { get; private set; }

    private void Awake()
    {
        FigureController figureModel = _view.GetComponentInParent<FigureController>();
        Model = new(figureModel, Vector3Int.RoundToInt(_view.transform.position));
    }
}
