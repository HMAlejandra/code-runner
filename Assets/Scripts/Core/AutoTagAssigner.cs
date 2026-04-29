using UnityEngine;

// Asigna tags automaticamente basado en el nombre del GameObject
// Esto es necesario porque los tags custom a veces no se asignan desde el editor
public class AutoTagAssigner : MonoBehaviour
{
    public string targetTag;

    void Awake()
    {
        if (!string.IsNullOrEmpty(targetTag))
            gameObject.tag = targetTag;
    }
}
