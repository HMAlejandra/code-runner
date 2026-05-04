using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Legacy UI manager kept for backward compatibility.
/// When CyberpunkUIManager is present in the scene it delegates to it automatically.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Cola de instrucciones")]
    public Transform queueContainer;
    public GameObject commandBlockPrefab;

    [Header("Botones")]
    public Button executeButton;
    public Button resetButton;

    [Header("Paneles")]
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("Log emocional")]
    public TextMeshProUGUI emotionalLogText;
    public float logDuration = 3f;

    [Header("Monitor de estado")]
    public Image stateIndicator;
    public Color colorEstadoA = Color.red;
    public Color colorEstadoB = Color.blue;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (queueContainer == null)
        {
            var panel = GameObject.Find("QueuePanel");
            if (panel != null) queueContainer = panel.transform;
        }

        if (commandBlockPrefab == null)
            commandBlockPrefab = Resources.Load<GameObject>("CommandBlockPrefab");
    }

    void Start()
    {
        if (executeButton) executeButton.onClick.AddListener(() => GameManager.Instance?.ExecuteSequence());
        if (resetButton) resetButton.onClick.AddListener(() => GameManager.Instance?.ResetLevel());
        HideSuccessPanel();
        if (failPanel) failPanel.SetActive(false);
        if (emotionalLogText) emotionalLogText.text = "";
    }

    // ── Queue ────────────────────────────────────────────────────────────────

    public void RefreshQueue(List<CommandType> commands)
    {
        // Delegate to cyberpunk manager when available
        if (CyberpunkUIManager.Instance != null)
        {
            CyberpunkUIManager.Instance.RefreshQueue(commands);
            return;
        }

        if (queueContainer == null) return;
        if (commandBlockPrefab == null)
        {
            Debug.LogWarning("[UIManager] commandBlockPrefab not assigned. Queue display skipped.");
            return;
        }

        foreach (Transform child in queueContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < commands.Count; i++)
        {
            int index = i;
            GameObject block = Instantiate(commandBlockPrefab, queueContainer);
            var label = block.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = commands[i].ToString();

            var btn = block.GetComponent<Button>();
            if (btn) btn.onClick.AddListener(() =>
                CommandSequenceManager.Instance.RemoveCommand(index));
        }
    }

    // ── Emotional log ────────────────────────────────────────────────────────

    public void ShowEmotionalLog(string message)
    {
        if (CyberpunkUIManager.Instance != null)
        {
            CyberpunkUIManager.Instance.ShowEmotionalLog(message);
            return;
        }

        if (emotionalLogText == null) return;
        emotionalLogText.text = message;
        CancelInvoke(nameof(ClearLog));
        Invoke(nameof(ClearLog), logDuration);

        if (failPanel) failPanel.SetActive(true);
        Invoke(nameof(HideFailPanel), logDuration);
    }

    private void ClearLog() { if (emotionalLogText) emotionalLogText.text = ""; }
    private void HideFailPanel() { if (failPanel) failPanel.SetActive(false); }

    // ── Panels ───────────────────────────────────────────────────────────────

    public void ShowSuccessPanel()
    {
        if (CyberpunkUIManager.Instance != null) { CyberpunkUIManager.Instance.ShowSuccessPanel(); return; }
        if (successPanel) successPanel.SetActive(true);
    }

    public void HideSuccessPanel()
    {
        if (CyberpunkUIManager.Instance != null) { CyberpunkUIManager.Instance.HideSuccessPanel(); return; }
        if (successPanel) successPanel.SetActive(false);
    }

    // ── Execute button ───────────────────────────────────────────────────────

    public void SetExecuteButtonInteractable(bool value)
    {
        if (CyberpunkUIManager.Instance != null) { CyberpunkUIManager.Instance.SetExecuteButtonInteractable(value); return; }
        if (executeButton) executeButton.interactable = value;
    }

    // ── State indicator ──────────────────────────────────────────────────────

    // CAMBIO AQUÍ: Se especifica que RobotState viene de RobotController3D
    public void UpdateStateIndicator(RobotController3D.RobotState state)
    {
        // También actualizamos la llamada al CyberpunkUIManager si existe
        if (CyberpunkUIManager.Instance != null) { CyberpunkUIManager.Instance.UpdateStateMonitor(state); return; }

        if (stateIndicator == null) return;

        // Comparamos con la ruta completa del enum
        stateIndicator.color = (state == RobotController3D.RobotState.ESTADO_A) ? colorEstadoA : colorEstadoB;
    }
}