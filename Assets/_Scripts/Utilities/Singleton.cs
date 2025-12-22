using Unity.VisualScripting;
using UnityEngine;
// a StaticInstance overrides the current instance rather than destroying it.
public class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance {  get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

// This adds changes to StaticInstance: It now destroys any new instances, only saving the original.
public class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }
}

// This survives through scenes. Good for stateful, persisten data or audio.
public class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
