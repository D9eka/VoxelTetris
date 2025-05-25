using System.Collections.Generic;
using UnityEngine;

public class FigureLight : MonoBehaviour
{
    [SerializeField] private Light _landingLightPrefab;
    private List<Light> _landingLights = new List<Light>();
    
    private Board _board;

    private void Start()
    {
        _board = ServiceLocator.Instance.Board;
    }

    public void UpdateLandingLights(Figure ActiveFigure)
        {
            ClearLandingLightsIfNoActiveFigure(ActiveFigure);
            if (ActiveFigure == null) return;
    
            var parts = ActiveFigure.Parts;
            int partCount = parts.Count;
    
            AdjustLandingLightsCount(partCount);
            UpdateLandingLightPositions(parts);
        }
    
        private void ClearLandingLightsIfNoActiveFigure(Figure ActiveFigure)
        {
            if (ActiveFigure == null)
            {
                for (int i = 0; i < _landingLights.Count; i++)
                    Destroy(_landingLights[i].gameObject);
                _landingLights.Clear();
            }
        }
    
        private void AdjustLandingLightsCount(int partCount)
        {
            while (_landingLights.Count < partCount)
            {
                var inst = Instantiate(_landingLightPrefab);
                _landingLights.Add(inst.GetComponent<Light>());
            }
            while (_landingLights.Count > partCount)
            {
                Destroy(_landingLights[_landingLights.Count - 1].gameObject);
                _landingLights.RemoveAt(_landingLights.Count - 1);
            }
        }
    
        private void UpdateLandingLightPositions(List<Transform> parts)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                Vector3 target = GetLandingPositionForCube(parts[i]);
                _landingLights[i].transform.position = target;
            }
        }
    
        private Vector3 GetLandingPositionForCube(Transform cube)
        {
            Vector3 cubePos = cube.position;
            int cubeX = Mathf.RoundToInt(cubePos.x);
            int cubeY = Mathf.RoundToInt(cubePos.y);
            int cubeZ = Mathf.RoundToInt(cubePos.z);
    
            int dropDist = CalculateDropDistanceForCube(cubeX, cubeY, cubeZ);
            return new Vector3(cubeX, cubeY - dropDist, cubeZ);
        }
    
        private int CalculateDropDistanceForCube(int x, int y, int z)
        {
            int dist = 0;
            while (_board.IsInside(new Vector3Int(x, y - (dist + 1), z)) &&
                   !_board.IsOccupied(new Vector3Int(x, y - (dist + 1), z)))
            {
                dist++;
            }
            return dist;
        }
}