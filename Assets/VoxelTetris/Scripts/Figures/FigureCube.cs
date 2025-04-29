using UnityEngine;

public class FigureCube : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshToColor;

    public void SetColor(Material color)
    {
        _meshToColor.material = color;
    }
}
