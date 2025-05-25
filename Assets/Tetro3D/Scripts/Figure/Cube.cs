using UnityEngine;

public class Cube : MonoBehaviour 
{
    [SerializeField] private MeshRenderer _meshToColor;
    
    public void SetMaterial(Material material) 
    {
        _meshToColor.material = material;
    }
}