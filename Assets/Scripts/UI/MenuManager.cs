using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button injectProtocolButton;
    public Button settingsButton;
    public Button achievementsButton;
    public int laboratorySceneIndex = 1;

    void Start()
    {
        if (injectProtocolButton != null)
            injectProtocolButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(laboratorySceneIndex);
    }
}