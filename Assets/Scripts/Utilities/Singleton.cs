using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour // Syntax for declaring generic type class.
{
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    instance = singleton.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    [SerializeField] private bool dontDestroyOnLoad = false;
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (dontDestroyOnLoad && Application.isPlaying)
            {
                transform.parent = null;
                DontDestroyOnLoad(gameObject); // this method only works if the gameObject is a root object (doesn't have any parent)
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
