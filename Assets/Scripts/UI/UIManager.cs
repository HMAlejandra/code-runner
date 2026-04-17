using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    }

    void Start()
    {
        executeButton.onClick.AddListener(() => GameManager.Instance.ExecuteSequence());
        resetButton.onClick.AddListener(() => GameManager.Instance.ResetLevel());
        HideSuccessPanel();
        if (failPanel) failPanel.SetActive(false);
        if (emotionalLogText) emotionalLogText.text = "";
    }

    public void RefreshQueue(List<CommandType> commands)
    {
        // Limpiar bloques anteriores
        foreach (Transform child in queueContainer)
            Destroy(child.gameObject);

        // Crear un bloque por comando
        for (int i = 0; i < commands.Count; i++)
        {
            int index = i;
            GameObject block = Instantiate(commandBlockPrefab, queueContainer);
            var label = block.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = commands[i].ToString();

            // Click para eliminar
            var btn = block.GetComponent<Button>();
            if (btn) btn.onClick.AddListener(() =>
                CommandSequenceManager.Instance.RemoveCommand(index));
        }
    }

    public void ShowEmotionalLog(string message)
    {
        if (emotionalLogText == null) return;
        emotionalLogText.text = message;
        CancelInvoke(nameof(ClearLog));
        Invoke(nameof(ClearLog), logDuration);

        if (failPanel) failPanel.SetActive(true);
        Invoke(nameof(HideFailPanel), logDuration);
    }

    private void ClearLog() { if (emotionalLogText) emotionalLogText.text = ""; }
    private void HideFailPanel() { if (failPanel) failPanel.SetActive(false); }

    public void ShowSuccessPanel() { if (successPanel) successPanel.SetActive(true); }
    public void HideSuccessPanel() { if (successPanel) successPanel.SetActive(false); }

    public void SetExecuteButtonInteractable(bool value)
    {
        if (executeButton) executeButton.interactable = value;
    }

    public void UpdateStateIndicator(RobotState state)
    {
        if (stateIndicator == null) return;
        stateIndicator.color = state == RobotState.ESTADO_A ? colorEstadoA : colorEstadoB;
    }
}
