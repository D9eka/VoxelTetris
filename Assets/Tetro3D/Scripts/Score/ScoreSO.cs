using UnityEngine;

[CreateAssetMenu(fileName = "ScoreSO", menuName = "VoxelTetris/ScoreSO")]
public class ScoreSO : ScriptableObject
{
    [SerializeField] private int _lowScoreStateValue;
    [SerializeField] private int _mediumScoreStateValue;
    [SerializeField] private int _highScoreStateValue;
        
    public int LowScoreStateValue => _lowScoreStateValue;
    public int MediumScoreStateValue => _mediumScoreStateValue;
    public int HighScoreStateValue => _highScoreStateValue;
}