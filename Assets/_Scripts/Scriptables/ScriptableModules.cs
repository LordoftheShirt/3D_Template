using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Module")]
public class ScriptableModules : ScriptableObject
{
    public WFCModuleEnum2D module;
    public GameObject prefab;
    public float weightPercentage = 1f;
}
