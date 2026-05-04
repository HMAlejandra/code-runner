using UnityEngine;
using UnityEngine.UI;

// Adjunta este script a cada boton del Banco de Funciones
public class CommandButton : MonoBehaviour
{
    public CommandType commandType;

    void Start()
    {
        // El Button puede estar en este GameObject o en el padre
        Button btn = GetComponent<Button>();
        if (btn == null)
            btn = GetComponentInParent<Button>();

        if (btn == null)
        {
            Debug.LogError($"[CommandButton] No Button component found on {gameObject.name} or its parents.");
            return;
        }

        btn.onClick.AddListener(() =>
        {
            if (CommandSequenceManager.Instance != null)
                CommandSequenceManager.Instance.AddCommand(commandType);
            else
                Debug.LogError("[CommandButton] CommandSequenceManager.Instance is null!");
        });
    }
}
