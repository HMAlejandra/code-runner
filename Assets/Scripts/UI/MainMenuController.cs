using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button playButton;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1); // Carga el Nivel 1
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
