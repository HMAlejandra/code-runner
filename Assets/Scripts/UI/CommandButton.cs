using UnityEngine;
using UnityEngine.UI;

// Adjunta este script a cada boton del Banco de Funciones
public class CommandButton : MonoBehaviour
{
    public CommandType commandType;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
            CommandSequenceManager.Instance.AddCommand(commandType));
    }
}
