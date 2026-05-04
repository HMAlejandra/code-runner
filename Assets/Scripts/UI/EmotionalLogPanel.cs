using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Displays narrative emotional log messages with a fade-in / hold / fade-out animation.
/// Wire ShowMessage() to RobotController3D.onEmotionalLog in the Inspector,
/// or call it directly from CyberpunkUIManager.ShowEmotionalLog().
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class EmotionalLogPanel : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI messageText;

    [Header("Timing")]
    public float fadeInDuration  = 0.25f;
    public float holdDuration    = 2.5f;
    public float fadeOutDuration = 0.5f;

    // ── Private ──────────────────────────────────────────────────────────────

    private CanvasGroup _group;
    private Coroutine   _routine;

    void Awake()
    {
        _group       = GetComponent<CanvasGroup>();
        _group.alpha = 0f;
        gameObject.SetActive(false);
    }

    // ════════════════════════════════════════════════════════════════════════
    // PUBLIC API
    // ════════════════════════════════════════════════════════════════════════

    public void ShowMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;

        if (_routine != null)
            StopCoroutine(_routine);

        gameObject.SetActive(true);
        _routine = StartCoroutine(FadeSequence());
    }

    // ════════════════════════════════════════════════════════════════════════
    // COROUTINE
    // ════════════════════════════════════════════════════════════════════════

    IEnumerator FadeSequence()
    {
        // Fade in
        yield return Fade(0f, 1f, fadeInDuration);

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        yield return Fade(1f, 0f, fadeOutDuration);

        gameObject.SetActive(false);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed      += Time.deltaTime;
            _group.alpha  = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _group.alpha = to;
    }
}
