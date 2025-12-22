using JetBrains.Annotations;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new 2D WFC table")]
public class WFC_2D_Scriptable : ScriptableObject
{
    public string MeshName;
    public GameObject gameObject;
    public WFC2Drotations[] rotation90;
}

[Serializable]
public struct WFC2Drotations
{
    public string rotation;
    public string up;
    public string down;
    public string left;
    public string right;
}