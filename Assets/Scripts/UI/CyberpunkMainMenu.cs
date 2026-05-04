using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Drives the cyberpunk Main Menu scene.
/// Attach to a "MenuController" GameObject in the MainMenu scene.
///
/// Expected Canvas hierarchy (built by SetupMainMenuScene editor tool or manually):
///
///  Canvas
///  └─ Background          (Image – deep purple gradient)
///     └─ GridOverlay      (Image – perspective grid, low alpha)
///  └─ CityLayer           (Image – neon city silhouette)
///  └─ GlowBeams           (Image – radial light beams behind title)
///  └─ TitleGroup
///     └─ TitleText        (TMP – "CODE RUNNER")
///     └─ SubtitleText     (TMP – "Fragmentos de Consciencia")
///  └─ ButtonBar           (HorizontalLayoutGroup)
///     └─ BtnLeaderboard   (CyberpunkIconButton)
///     └─ BtnAchievements  (CyberpunkIconButton)
///     └─ BtnPlay          (CyberpunkIconButton – LARGE / gold)
///     └─ BtnSettings      (CyberpunkIconButton)
///     └─ BtnCredits       (CyberpunkIconButton)
///  └─ VersionText         (TMP – bottom-left)
/// </summary>
[RequireComponent(typeof(MainMenuController))]
public class CyberpunkMainMenu : MonoBehaviour
{
    [Header("Title")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;

    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Background FX")]
    public Image backgroundImage;
    public Image cityLayer;
    public Image glowBeams;

    [Header("Flicker settings")]
    [Range(0f, 1f)] public float flickerChance = 0.03f;
    public float flickerMinAlpha = 0.7f;

    // ── Pulse animation state ────────────────────────────────────────────────
    private float _pulseTime;
    private Image _playButtonImage;

    void Start()
    {
        ApplyCyberpunkStyle();
        StartCoroutine(TitleGlitchRoutine());
    }

    void Update()
    {
        PulsePlayButton();
        FlickerCityLayer();
    }

    // ── Visual setup ─────────────────────────────────────────────────────────

    void ApplyCyberpunkStyle()
    {
        // Background
        if (backgroundImage)
            backgroundImage.color = CyberpunkTheme.BgDeep;

        // Title – golden gradient via vertex color trick
        if (titleText)
        {
            titleText.text = "CODE RUNNER";
            titleText.enableVertexGradient = true;
            titleText.colorGradient = new VertexGradient(
                CyberpunkTheme.NeonYellow,
                CyberpunkTheme.NeonOrange,
                CyberpunkTheme.NeonOrange,
                new Color(0.8f, 0.3f, 0f)
            );
            titleText.fontStyle = FontStyles.Bold;
            titleText.characterSpacing = 8f;
        }

        // Subtitle – white with slight blue tint
        if (subtitleText)
        {
            subtitleText.text = "FRAGMENTOS DE CONSCIENCIA";
            subtitleText.color = CyberpunkTheme.TextPrimary;
            subtitleText.fontStyle = FontStyles.Italic;
            subtitleText.characterSpacing = 4f;
        }

        // Play button – gold frame
        if (playButton)
        {
            _playButtonImage = playButton.GetComponent<Image>();
            if (_playButtonImage)
                _playButtonImage.color = CyberpunkTheme.ExecuteGold;

            var label = playButton.GetComponentInChildren<TextMeshProUGUI>();
            if (label)
            {
                label.text = "▶  JUGAR";
                label.color = CyberpunkTheme.BgDeep;
                label.fontStyle = FontStyles.Bold;
                label.fontSize = 28;
            }

            // Color block
            var cb = playButton.colors;
            cb.normalColor      = CyberpunkTheme.ExecuteGold;
            cb.highlightedColor = Color.white;
            cb.pressedColor     = CyberpunkTheme.NeonOrange;
            cb.disabledColor    = CyberpunkTheme.ExecuteGoldDim;
            playButton.colors   = cb;
        }

        // Secondary buttons – cyan neon
        StyleSecondaryButton(settingsButton, "⚙  OPCIONES");
        StyleSecondaryButton(creditsButton,  "✦  CRÉDITOS");
        StyleSecondaryButton(quitButton,     "✕  SALIR");
    }

    void StyleSecondaryButton(Button btn, string label)
    {
        if (btn == null) return;
        var img = btn.GetComponent<Image>();
        if (img) img.color = CyberpunkTheme.BgPanelDark;

        var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (txt)
        {
            txt.text  = label;
            txt.color = CyberpunkTheme.NeonCyan;
            txt.fontStyle = FontStyles.Bold;
        }

        var cb = btn.colors;
        cb.normalColor      = CyberpunkTheme.BgPanelDark;
        cb.highlightedColor = new Color(0.1f, 0.3f, 0.4f);
        cb.pressedColor     = CyberpunkTheme.NeonCyan;
        btn.colors          = cb;
    }

    // ── Animations ───────────────────────────────────────────────────────────

    void PulsePlayButton()
    {
        if (_playButtonImage == null) return;
        _pulseTime += Time.deltaTime * 2f;
        float t = (Mathf.Sin(_pulseTime) + 1f) * 0.5f;
        _playButtonImage.color = Color.Lerp(CyberpunkTheme.ExecuteGold, Color.white, t * 0.25f);
    }

    void FlickerCityLayer()
    {
        if (cityLayer == null) return;
        if (Random.value < flickerChance)
        {
            float a = Random.Range(flickerMinAlpha, 1f);
            var c = cityLayer.color;
            c.a = a;
            cityLayer.color = c;
        }
    }

    IEnumerator TitleGlitchRoutine()
    {
        if (titleText == null) yield break;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 8f));
            // Brief glitch: shift character spacing
            titleText.characterSpacing = Random.Range(-2f, 14f);
            yield return new WaitForSeconds(0.05f);
            titleText.characterSpacing = 8f;
        }
    }
}
