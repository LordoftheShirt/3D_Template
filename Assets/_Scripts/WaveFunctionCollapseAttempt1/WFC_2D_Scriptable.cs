using JetBrains.Annotations;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new 2D WFC table")]
public class WFC_2D_Scriptable : ScriptableObject
{
    public WFCType WFCType;
    public GameObject gameObject;
    public WFC2Drotations[] rotation90;
}

[Serializable]
public struct WFC2Drotations
{
    public int rotation;
    public int up;
    public int down;
    public int left;
    public int right;
}

public enum WFCType
{
    Roadless = 0,
    Straight = 1,
    DeadEnd = 2,
    Cross = 3,
    TCross = 4,
    Turn90 = 5,
}