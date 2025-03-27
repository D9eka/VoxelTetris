using System.Collections.Generic;
using UnityEngine;

public class FigureView : MonoBehaviour
{
    [SerializeField] private FigurePartView _center;
    public FigurePartController[] Parts => GetComponentsInChildren<FigurePartController>();

    public FigurePartView Center => _center;
}