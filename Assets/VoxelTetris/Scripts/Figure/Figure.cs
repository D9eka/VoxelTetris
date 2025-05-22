using System.Collections.Generic;
using UnityEngine;

public class Figure : MonoBehaviour
{
    [SerializeField] private FigureSO _data;
    [SerializeField] private Transform _center;
    [SerializeField] private int _height;
    
    public FigureSO Data => _data;
    public List<Transform> Parts { get; private set; }
    public Transform Center => _center;
    public int Height => _height;

    public void Initialize(Cube cube, Material mat) 
    {
        Vector3Int centerPos = Vector3Int.RoundToInt(_center.position);
        
        List<Vector3Int> partsPositions = new List<Vector3Int>();
        foreach (Transform child in transform) {
            partsPositions.Add(Vector3Int.RoundToInt(child.position));
            Destroy(child.gameObject);
        }
        
        Parts = new List<Transform>();
        foreach (Vector3Int partPositions in partsPositions) {
            GameObject cubeGo = Instantiate(cube.gameObject, partPositions, Quaternion.identity, transform);
            Parts.Add(cubeGo.transform);
            cubeGo.GetComponent<Cube>().SetMaterial(mat);
            if (partPositions == centerPos)
            {
                _center = cubeGo.transform;
            }
        }
    }
}