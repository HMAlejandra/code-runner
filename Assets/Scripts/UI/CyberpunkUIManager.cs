using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CyberpunkUIManager : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────────────────
    public static CyberpunkUIManager Instance { get; private set; }

    // ── Inspector refs ───────────────────────────────────────────────────────
    [Header("Queue")]
    public Transform queueContent;
    public GameObject commandBlockPrefab;
    public TextMeshProUGUI queueCountLabel;

    [Header("Action Buttons")]
    public Button executeButton;
    public Button resetButton;

    [Header("State Monitor")]
    public Image stateIcon;
    public TextMeshProUGUI stateLabel;
    public TextMeshProUGUI fragmentCountText;

    [Header("Overlay Panels")]
    public GameObject emotionalLogPanel;
    public TextMeshProUGUI emotionalLogText;
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("Settings")]
    public float logDuration = 3.5f;
    public int maxCommands = 10;

    // ── Private ──────────────────────────────────────────────────────────────
    private int _fragmentsCollected = 0;
    private int _fragmentsTotal = 5;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (queueContent == null)
        {
            var go = GameObject.Find("QueueContent");
            if (go) queueContent = go.transform;
        }

        if (commandBlockPrefab == null)
            commandBlockPrefab = Resources.Load<GameObject>("CommandBlockPrefab");
    }

    void Start()
    {
        if (executeButton) executeButton.onClick.AddListener(() => GameManager.Instance?.ExecuteSequence());
        if (resetButton) resetButton.onClick.AddListener(() => GameManager.Instance?.ResetLevel());

        HideSuccessPanel();
        HideFailPanel();
        if (emotionalLogPanel) emotionalLogPanel.SetActive(false);
        if (emotionalLogText) emotionalLogText.text = "";

        // CORRECCIÓN: Referencia completa al estado inicial
        UpdateStateMonitor(RobotController3D.RobotState.ESTADO_A);
        StyleActionButtons();
    }

    // ── Queue ────────────────────────────────────────────────────────────────

    public void RefreshQueue(List<CommandType> commands)
    {
        if (queueContent == null) return;

        foreach (Transform child in queueContent)
            Destroy(child.gameObject);

        for (int i = 0; i < commands.Count; i++)
        {
            int idx = i;
            GameObject block = commandBlockPrefab != null
                ? Instantiate(commandBlockPrefab, queueContent)
                : CreateFallbackBlock(commands[i]);

            StyleQueueBlock(block, commands[i]);

            var btn = block.GetComponent<Button>();
            if (btn) btn.onClick.AddListener(() =>
                CommandSequenceManager.Instance?.RemoveCommand(idx));
        }

        if (queueCountLabel)
            queueCountLabel.text = $"COLA DE INSTRUCCIONES  [{commands.Count}/{maxCommands}]";
    }

    void StyleQueueBlock(GameObject block, CommandType cmd)
    {
        if (block == null) return;

        Color accent = CyberpunkTheme.BlockColor(cmd);
        var img = block.GetComponent<Image>();
        if (img) img.color = new Color(accent.r * 0.2f, accent.g * 0.2f, accent.b * 0.2f, 0.9f);

        var texts = block.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = CyberpunkTheme.Icon(cmd);
            texts[0].color = accent;
            texts[0].fontSize = 18;
            texts[0].fontStyle = FontStyles.Bold;
        }
        if (texts.Length > 1)
        {
            texts[1].text = CyberpunkTheme.Label(cmd);
            texts[1].color = CyberpunkTheme.TextSecondary;
            texts[1].fontSize = 9;
        }
    }

    GameObject CreateFallbackBlock(CommandType cmd)
    {
        var go = new GameObject($"Block_{cmd}");
        go.transform.SetParent(queueContent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60, 60);
        go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();
        img.color = CyberpunkTheme.BlockColor(cmd);
        go.AddComponent<Button>();
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        var lrt = labelGo.AddComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.sizeDelta = Vector2.zero;
        labelGo.AddComponent<CanvasRenderer>();
        var tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = CyberpunkTheme.Icon(cmd);
        tmp.color = Color.white;
        tmp.fontSize = 22;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return go;
    }

    // ── State Monitor ────────────────────────────────────────────────────────

    // CORRECCIÓN: Se cambia RobotState por RobotController3D.RobotState
    public void UpdateStateMonitor(RobotController3D.RobotState state)
    {
        bool isA = state == RobotController3D.RobotState.ESTADO_A;

        if (stateIcon)
        {
            stateIcon.color = isA ? CyberpunkTheme.StateA : CyberpunkTheme.StateB;
            StartCoroutine(PulseIcon());
        }

        if (stateLabel)
        {
            stateLabel.text = isA ? "MODO LÓGICO" : "MODO EMOCIONAL";
            stateLabel.color = isA ? CyberpunkTheme.StateA : CyberpunkTheme.StateB;
        }
    }

    IEnumerator PulseIcon()
    {
        if (stateIcon == null) yield break;
        Color original = stateIcon.color;
        stateIcon.color = Color.white;
        yield return new WaitForSeconds(0.12f);
        stateIcon.color = original;
    }

    public void AddFragment()
    {
        _fragmentsCollected = Mathf.Min(_fragmentsCollected + 1, _fragmentsTotal);
        if (fragmentCountText)
            fragmentCountText.text = $"[*] {_fragmentsCollected} / {_fragmentsTotal}";
    }

    // ── Emotional Log ────────────────────────────────────────────────────────

    public void ShowEmotionalLog(string message)
    {
        if (emotionalLogText) emotionalLogText.text = message;
        if (emotionalLogPanel) emotionalLogPanel.SetActive(true);
        if (failPanel) failPanel.SetActive(true);

        CancelInvoke(nameof(HideLog));
        Invoke(nameof(HideLog), logDuration);
    }

    void HideLog()
    {
        if (emotionalLogText) emotionalLogText.text = "";
        if (emotionalLogPanel) emotionalLogPanel.SetActive(false);
        HideFailPanel();
    }

    // ── Panels ───────────────────────────────────────────────────────────────

    public void ShowSuccessPanel() { if (successPanel) successPanel.SetActive(true); }
    public void HideSuccessPanel() { if (successPanel) successPanel.SetActive(false); }
    public void HideFailPanel() { if (failPanel) failPanel.SetActive(false); }

    // ── Execute button ───────────────────────────────────────────────────────

    public void SetExecuteButtonInteractable(bool value)
    {
        if (executeButton == null) return;
        executeButton.interactable = value;
        var img = executeButton.GetComponent<Image>();
        if (img) img.color = value ? CyberpunkTheme.ExecuteGold : CyberpunkTheme.ExecuteGoldDim;
    }

    // ── Button styling ───────────────────────────────────────────────────────

    void StyleActionButtons()
    {
        if (executeButton)
        {
            var img = executeButton.GetComponent<Image>();
            if (img) img.color = CyberpunkTheme.ExecuteGold;
            var txt = executeButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt)
            {
                txt.text = ">>  EJECUTAR";
                txt.color = CyberpunkTheme.BgDeep;
                txt.fontStyle = FontStyles.Bold;
                txt.fontSize = 20;
            }
            var cb = executeButton.colors;
            cb.normalColor = CyberpunkTheme.ExecuteGold;
            cb.highlightedColor = Color.white;
            cb.pressedColor = CyberpunkTheme.NeonOrange;
            cb.disabledColor = CyberpunkTheme.ExecuteGoldDim;
            executeButton.colors = cb;
        }

        if (resetButton)
        {
            var img = resetButton.GetComponent<Image>();
            if (img) img.color = CyberpunkTheme.BgPanelDark;
            var txt = resetButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt)
            {
                txt.text = "<>  REINICIAR";
                txt.color = CyberpunkTheme.NeonCyan;
                txt.fontStyle = FontStyles.Bold;
                txt.fontSize = 16;
            }
            var cb = resetButton.colors;
            cb.normalColor = CyberpunkTheme.BgPanelDark;
            cb.highlightedColor = new Color(0.05f, 0.25f, 0.35f);
            cb.pressedColor = CyberpunkTheme.NeonCyan;
            resetButton.colors = cb;
        }
    }
}