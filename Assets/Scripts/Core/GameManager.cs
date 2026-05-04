using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias")]
    // CORRECCIÓN: Se cambia 'RobotController' por 'RobotController3D'
    public RobotController3D robot;

    private bool isRunning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ExecuteSequence()
    {
        if (isRunning) return;

        // CORRECCIÓN: Referencia al Singleton correcto del CommandSequenceManager
        var commands = CommandSequenceManager.Instance.GetCommands();
        if (commands.Count == 0) return;

        isRunning = true;

        // CORRECCIÓN: Se usa CyberpunkUIManager si ese es el nombre de tu clase actual
        CyberpunkUIManager.Instance?.SetExecuteButtonInteractable(false);
        StartCoroutine(robot.ExecuteSequence(commands));
    }

    public void OnSequenceComplete()
    {
        isRunning = false;
        CyberpunkUIManager.Instance?.ShowSuccessPanel();
    }

    public void OnFail()
    {
        isRunning = false;
        CyberpunkUIManager.Instance?.SetExecuteButtonInteractable(true);
    }

    public void ResetLevel()
    {
        isRunning = false;
        if (robot != null) robot.ResetToStart();

        CommandSequenceManager.Instance.ClearCommands();
        CyberpunkUIManager.Instance?.SetExecuteButtonInteractable(true);
        CyberpunkUIManager.Instance?.HideSuccessPanel();
    }

    public void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene("MainMenu");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}