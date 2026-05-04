using System.Collections.Generic;
using UnityEngine;

public class CommandSequenceManager : MonoBehaviour
{
    public static CommandSequenceManager Instance { get; private set; }

    [Header("Referencias")]
    public RobotController3D robot;

    [Header("Configuración")]
    private List<CommandType> commands = new List<CommandType>();
    public int MaxCommands = 10;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Métodos para botones manuales
    public void AddMoveCommand() => AddCommand(CommandType.MOVER);
    public void AddJumpCommand() => AddCommand(CommandType.SALTAR);
    public void AddStateCommand() => AddCommand(CommandType.CAMBIAR_ESTADO);

    // Método público para que otros scripts añadan comandos
    public void AddCommand(CommandType cmd)
    {
        if (commands.Count >= MaxCommands) return;
        commands.Add(cmd);
        CyberpunkUIManager.Instance?.RefreshQueue(commands);
    }

    // Proporciona la lista de comandos al GameManager (Arregla el error CS1061)
    public List<CommandType> GetCommands() => new List<CommandType>(commands);

    public void ExecuteSequence()
    {
        if (robot != null && commands.Count > 0)
        {
            robot.ExecuteSequence(new List<CommandType>(commands));
            ClearCommands();
        }
        else
        {
            Debug.LogWarning("No hay robot asignado o la lista de comandos está vacía.");
        }
    }

    public void RemoveCommand(int index)
    {
        if (index < 0 || index >= commands.Count) return;
        commands.RemoveAt(index);
        CyberpunkUIManager.Instance?.RefreshQueue(commands);
    }

    public void ClearCommands()
    {
        commands.Clear();
        CyberpunkUIManager.Instance?.RefreshQueue(commands);
    }
}