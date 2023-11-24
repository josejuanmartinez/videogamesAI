using UnityEngine;

public class Singleton : MonoBehaviour
{
    void Awake()
    {
        if (GameObject.Find(gameObject.name) != null)
            DestroyImmediate(this);
    }
}
