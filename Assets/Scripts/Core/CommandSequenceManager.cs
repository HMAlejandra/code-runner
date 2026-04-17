using System.Collections.Generic;
using UnityEngine;

public class CommandSequenceManager : MonoBehaviour
{
    public static CommandSequenceManager Instance { get; private set; }

    private List<CommandType> commands = new List<CommandType>();
    public int MaxCommands = 10;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddCommand(CommandType cmd)
    {
        if (commands.Count >= MaxCommands) return;
        commands.Add(cmd);
        UIManager.Instance?.RefreshQueue(commands);
    }

    public void RemoveCommand(int index)
    {
        if (index < 0 || index >= commands.Count) return;
        commands.RemoveAt(index);
        UIManager.Instance?.RefreshQueue(commands);
    }

    public void ClearCommands()
    {
        commands.Clear();
        UIManager.Instance?.RefreshQueue(commands);
    }

    public List<CommandType> GetCommands() => new List<CommandType>(commands);
}
