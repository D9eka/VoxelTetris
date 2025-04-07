using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Angle => _targetAngle;
    public static CameraController Instance { get; private set; }
    
    [SerializeField] private Camera _camera;
    [SerializeField] private CameraData _data;
    
    private float _currentAngle;
    private float _targetAngle;

    private Vector3 _gridCenter;
    private float _gridHeight;
    private float _gridDepth;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _camera.fieldOfView = _data.FOV;
        _currentAngle = 0f;
        _targetAngle = 0f;
        UpdateCameraPosition();
        
        GridController gridController = ServiceLocator.Instance.GridController;
        _gridCenter = gridController.View.Center;
        
        GridModel gridModel = gridController.Model;
        _gridHeight = gridModel.Height;
        _gridDepth = gridModel.Depth;

        ServiceLocator.Instance.InputManager.PlayerRotateCamera += OnRotateCamera;
    }

    private void Update()
    {
        _currentAngle = Mathf.LerpAngle(_currentAngle, _targetAngle, Time.deltaTime * _data.RotationSpeed);
        UpdateCameraPosition();
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.InputManager.PlayerRotateCamera -= OnRotateCamera;
    }

    private void OnRotateCamera(float direction)
    {
        int directionInt = Mathf.RoundToInt(direction);
        _targetAngle = (_targetAngle + 90 * directionInt) % 360f;
    }

    private void UpdateCameraPosition()
    {
        float cameraHeight = _gridHeight * _data.HeightModifier;
        float cameraDistance = _gridDepth * _data.DepthModifier;

        float angleInRadians = _currentAngle * Mathf.Deg2Rad;
        float x = _gridCenter.x + cameraDistance * Mathf.Sin(angleInRadians);
        float z = _gridCenter.z - cameraDistance * Mathf.Cos(angleInRadians);

        Vector3 cameraPosition = new Vector3(x, cameraHeight, z);
        _camera.transform.position = cameraPosition;

        _camera.transform.LookAt(_gridCenter);
    }
}