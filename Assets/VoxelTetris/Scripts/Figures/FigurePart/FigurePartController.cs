using UnityEngine;

public class FigurePartController : MonoBehaviour
{
    public FigurePartModel Model { get; private set; }

    private void Awake()
    {
        FigureController figureController = GetComponentInParent<FigureController>();
        Model = new(this, figureController, Vector3Int.RoundToInt(transform.position));
    }

    public void Initialize(FigureCube cubePrefab, Material color)
    {
        FigureCube cube = Instantiate(cubePrefab, transform.position, Quaternion.identity, transform);
        cube.SetColor(color);
    }
}
