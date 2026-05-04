using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum RobotState { ESTADO_A, ESTADO_B }

public class RobotController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveDistance = 2f;
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Estado Visual")]
    public Renderer robotRenderer;
    public Material materialEstadoA; // Rojo
    public Material materialEstadoB; // Azul

    [Header("Capas de colision")]
    public LayerMask groundLayer;
    public LayerMask estadoALayer;
    public LayerMask estadoBLayer;

    [Header("Eventos")]
    public UnityEvent onCommandStart;
    public UnityEvent onSequenceComplete;
    public UnityEvent onFail;
    public UnityEvent<string> onEmotionalLog;

    private Rigidbody rb;
    private RobotState currentState = RobotState.ESTADO_A;
    private bool isExecuting = false;
    private Vector3 startPosition;

    // Logs emocionales por tipo de fallo
    private string[] wallLogs = {
        "Siento que me desvanezco...",
        "Este muro... no puedo atravesarlo.",
        "Error. No encuentro el camino."
    };
    private string[] fallLogs = {
        "¿Por qué este comando duele tanto?",
        "Caigo... y no recuerdo cómo detenerme.",
        "El vacío me consume. Intenta de nuevo."
    };

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        ApplyStateMaterial();
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    public RobotState GetCurrentState() => currentState;
    public bool IsExecuting() => isExecuting;

    public void ResetToStart()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        currentState = RobotState.ESTADO_A;
        ApplyStateMaterial();
        isExecuting = false;
    }

    public IEnumerator ExecuteSequence(List<CommandType> commands)
    {
        isExecuting = true;
        onCommandStart?.Invoke();

        for (int i = 0; i < commands.Count; i++)
        {
            yield return StartCoroutine(ExecuteCommand(commands[i]));

            if (!isExecuting) yield break; // fallo detectado
        }

        isExecuting = false;
        onSequenceComplete?.Invoke();
    }

    private IEnumerator ExecuteCommand(CommandType cmd)
    {
        switch (cmd)
        {
            case CommandType.MOVER:
                yield return StartCoroutine(MoveForward());
                break;
            case CommandType.SALTAR:
                yield return StartCoroutine(Jump());
                break;
            case CommandType.ESPERAR:
                yield return new WaitForSeconds(1f);
                break;
            case CommandType.CAMBIAR_ESTADO:
                ToggleState();
                yield return new WaitForSeconds(0.3f);
                break;
        }
    }

    private IEnumerator MoveForward()
    {
        Vector3 target = transform.position + transform.forward * moveDistance;

        // Verificar colision frontal
        if (Physics.Raycast(transform.position, transform.forward, moveDistance, ~groundLayer))
        {
            TriggerFail(wallLogs[Random.Range(0, wallLogs.Length)]);
            yield break;
        }

        float elapsed = 0f;
        float duration = moveDistance / moveSpeed;
        Vector3 origin = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(origin, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }

    private IEnumerator Jump()
    {
        if (!IsGrounded())
        {
            TriggerFail(wallLogs[Random.Range(0, wallLogs.Length)]);
            yield break;
        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);

        // Esperar a aterrizar
        float timeout = 3f;
        while (!IsGrounded() && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
    }

    private void ToggleState()
    {
        currentState = currentState == RobotState.ESTADO_A
            ? RobotState.ESTADO_B
            : RobotState.ESTADO_A;
        ApplyStateMaterial();
    }

    private void ApplyStateMaterial()
    {
        if (robotRenderer == null) return;
        robotRenderer.sharedMaterial = currentState == RobotState.ESTADO_A
            ? materialEstadoA
            : materialEstadoB;
    }

    private void TriggerFail(string log)
    {
        isExecuting = false;
        onEmotionalLog?.Invoke(log);
        onFail?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Void"))
            TriggerFail(fallLogs[Random.Range(0, fallLogs.Length)]);

        if (other.CompareTag("Goal") && isExecuting)
        {
            isExecuting = false;
            onSequenceComplete?.Invoke();
        }

        // Barreras de estado
        if (other.CompareTag("BarreraA") && currentState == RobotState.ESTADO_B)
            TriggerFail("Esta barrera bloquea mi estado actual...");

        if (other.CompareTag("BarreraB") && currentState == RobotState.ESTADO_A)
            TriggerFail("Necesito cambiar mi estado para pasar.");
    }
}
