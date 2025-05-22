using UnityEngine;

[CreateAssetMenu(fileName = "FigureSO", menuName = "VoxelTetris/FigureSO")]
public class FigureSO : ScriptableObject
{
    [SerializeField] private int _placeScore;
    [SerializeField] private int _clearPlaneScore;
    [SerializeField] private int _comboBonusScore;
    
    public int PlaceScore => _placeScore;
    public int ClearPlaneScore => _clearPlaneScore;
    public int ComboBonusScore => _comboBonusScore;
}