using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages the Main Menu scene (Scene 0).
/// Handles button wiring for "Inject Protocol" (Play), Settings, and Achievements.
/// Attach to the MenuController GameObject in the MainMenu scene.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Buttons – wire in Inspector")]
    public Button injectProtocolButton;   // "Inject Protocol" = Play
    public Button settingsButton;
    public Button achievementsButton;

    [Header("Scene Index")]
    [Tooltip("Build index of the first gameplay scene (Nivel 1).")]
    public int laboratorySceneIndex = 1;

    [Header("Optional – Settings / Achievements Panels")]
    public GameObject settingsPanel;
    public GameObject achievementsPanel;

    [Header("Transition")]
    public float transitionDelay = 0.4f;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    void Start()
    {
        WireButtons();
        ApplyButtonLabels();
    }

    // ── Wiring ───────────────────────────────────────────────────────────────

    void WireButtons()
    {
        if (injectProtocolButton != null)
            injectProtocolButton.onClick.AddListener(OnInjectProtocol);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);

        if (achievementsButton != null)
            achievementsButton.onClick.AddListener(OnAchievements);
    }

    /// <summary>
    /// Renames button labels to match the cyberpunk narrative.
    /// Safe to call even if TMP components are missing.
    /// </summary>
    void ApplyButtonLabels()
    {
        SetButtonLabel(injectProtocolButton, "▶  INJECT PROTOCOL");
        SetButtonLabel(settingsButton,       "⚙  SETTINGS");
        SetButtonLabel(achievementsButton,   "★  ACHIEVEMENTS");
    }

    static void SetButtonLabel(Button btn, string text)
    {
        if (btn == null) return;
        var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;
    }

    // ── Button handlers ──────────────────────────────────────────────────────

    public void OnInjectProtocol()
    {
        StartCoroutine(LoadSceneDelayed(laboratorySceneIndex));
    }

    public void OnSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void OnAchievements()
    {
        if (achievementsPanel != null)
            achievementsPanel.SetActive(!achievementsPanel.activeSelf);
    }

    IEnumerator LoadSceneDelayed(int index)
    {
        // Disable button to prevent double-click
        if (injectProtocolButton != null)
            injectProtocolButton.interactable = false;

        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(index);
    }

    // ── Public helpers (callable from Inspector events) ──────────────────────

    public void LoadScene(int index) => SceneManager.LoadScene(index);
    public void QuitGame()           => Application.Quit();
}
