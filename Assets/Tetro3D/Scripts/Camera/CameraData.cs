using UnityEngine;

[CreateAssetMenu(fileName = "CameraData", menuName = "VoxelTetris/CameraData")]
public class CameraData : ScriptableObject
{
    public float RotationSpeed => _rotationSpeed;
    public float HeightModifier => _heightModifier;
    public float DepthModifier => _depthModifier;
    public float FOV => _fov;

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _heightModifier;
    [SerializeField] private float _depthModifier;
    [SerializeField] private float _fov;
}