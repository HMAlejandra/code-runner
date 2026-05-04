using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Cyberpunk-styled command button for the Banco de Funciones panel.
/// Shows a large Unicode icon + label underneath, with neon glow on hover.
///
/// Hierarchy expected under this GameObject:
///   [Button]
///   ├─ FrameImage   (Image – dark panel background)
///   ├─ IconText     (TMP – large Unicode symbol)
///   └─ LabelText    (TMP – command name, small)
/// </summary>
[RequireComponent(typeof(Button))]
public class CyberpunkCommandButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Command")]
    public CommandType commandType;

    [Header("References (auto-found if blank)")]
    public Image frameImage;
    public TextMeshProUGUI iconText;
    public TextMeshProUGUI labelText;

    private Button _btn;
    private Color _baseColor;
    private Color _accentColor;

    void Awake()
    {
        _btn = GetComponent<Button>();

        // Auto-find children
        if (frameImage == null) frameImage = GetComponent<Image>();
        if (iconText  == null) iconText  = transform.Find("IconText")?.GetComponent<TextMeshProUGUI>();
        if (labelText == null) labelText = transform.Find("LabelText")?.GetComponent<TextMeshProUGUI>();

        _accentColor = CyberpunkTheme.BlockColor(commandType);
        _baseColor   = CyberpunkTheme.BgPanelDark;

        ApplyStyle(false);
    }

    void Start()
    {
        // Click is handled via IPointerClickHandler to allow flash feedback
    }

    // ── Style ────────────────────────────────────────────────────────────────

    void ApplyStyle(bool hovered)
    {
        if (frameImage)
            frameImage.color = hovered
                ? new Color(_accentColor.r * 0.25f, _accentColor.g * 0.25f, _accentColor.b * 0.25f, 0.95f)
                : _baseColor;

        if (iconText)
        {
            iconText.text  = CyberpunkTheme.Icon(commandType);
            iconText.color = hovered ? Color.white : _accentColor;
            iconText.fontSize = 36;
            iconText.fontStyle = FontStyles.Bold;
        }

        if (labelText)
        {
            labelText.text  = CyberpunkTheme.Label(commandType);
            labelText.color = hovered ? _accentColor : CyberpunkTheme.TextSecondary;
            labelText.fontSize = 11;
            labelText.fontStyle = FontStyles.Bold;
            labelText.characterSpacing = 2f;
        }
    }

    // ── Events ───────────────────────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData _) => ApplyStyle(true);
    public void OnPointerExit(PointerEventData _)  => ApplyStyle(false);

    public void OnPointerClick(PointerEventData _)
    {
        if (CommandSequenceManager.Instance != null)
            CommandSequenceManager.Instance.AddCommand(commandType);
        else
            Debug.LogError("[CyberpunkCommandButton] CommandSequenceManager.Instance is null!");

        // Brief flash feedback
        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        if (frameImage)
        {
            frameImage.color = _accentColor;
            yield return new WaitForSeconds(0.08f);
            ApplyStyle(false);
        }
    }
}
