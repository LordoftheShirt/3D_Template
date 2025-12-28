using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Module")]
public class ScriptableModules : ScriptableObject
{
    public string moduleName;
    public GameObject prefab;
    public float weightPercentage = 1f;
}
