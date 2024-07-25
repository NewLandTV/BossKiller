using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected void Setup(T instance)
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            
            return;
        }

        Instance = instance;

        DontDestroyOnLoad(gameObject);
    }
}
