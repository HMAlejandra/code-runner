using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias")]
    public RobotController robot;

    private bool isRunning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ExecuteSequence()
    {
        if (isRunning) return;
        var commands = CommandSequenceManager.Instance.GetCommands();
        if (commands.Count == 0) return;

        isRunning = true;
        UIManager.Instance?.SetExecuteButtonInteractable(false);
        StartCoroutine(robot.ExecuteSequence(commands));
    }

    public void OnSequenceComplete()
    {
        isRunning = false;
        UIManager.Instance?.ShowSuccessPanel();
    }

    public void OnFail()
    {
        isRunning = false;
        UIManager.Instance?.SetExecuteButtonInteractable(true);
    }

    public void ResetLevel()
    {
        isRunning = false;
        robot.ResetToStart();
        CommandSequenceManager.Instance.ClearCommands();
        UIManager.Instance?.SetExecuteButtonInteractable(true);
        UIManager.Instance?.HideSuccessPanel();
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
