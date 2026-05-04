using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Controls Robot Kyle (URP) – movement, jump, state toggle, and aura.
/// Handles batch command sequence execution for the programming puzzle mechanic.
/// </summary>
public class RobotController3D : MonoBehaviour
{
    // ── State ────────────────────────────────────────────────────────────────

    public enum RobotState { ESTADO_A, ESTADO_B }

    [Header("Current State")]
    public RobotState currentState = RobotState.ESTADO_A;

    // ── Visual ───────────────────────────────────────────────────────────────

    [Header("Aura Light (Point Light inside chest)")]
    public Light auraLight;

    [Header("Aura Colors")]
    public Color auraColorA = new Color(1f, 0.15f, 0.10f);   // RED  – Logic
    public Color auraColorB = new Color(0.15f, 0.50f, 1.00f); // BLUE – Emotional

    [Header("Aura Intensity")]
    public float auraIntensity = 3.5f;

    // ── Animator ─────────────────────────────────────────────────────────────

    [Header("Animator (Robot Kyle)")]
    public Animator animator;

    // ── Movement ─────────────────────────────────────────────────────────────

    [Header("Movement")]
    public float moveDistance = 2f;
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 6f;
    private Rigidbody _rb;
    private bool _isGrounded = true;

    // ── Events ───────────────────────────────────────────────────────────────

    [Header("Emotional Log Event")]
    public UnityEvent<string> onEmotionalLog;

    // ── Private ──────────────────────────────────────────────────────────────

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private static readonly string[] CollisionMessages =
    {
        "Este comando duele...",
        "Memoria corrompida...",
        "ERROR: fragmento perdido",
        "No puedo procesar esto...",
        "Protocolo rechazado",
        "Datos incoherentes detectados",
        "Fallo en el núcleo emocional",
        "Acceso denegado por la barrera",
        "Consciencia fragmentada...",
        "Ciclo roto. Reiniciando..."
    };

    // ════════════════════════════════════════════════════════════════════════
    // LIFECYCLE
    // ════════════════════════════════════════════════════════════════════════

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        ApplyAura();
    }

    // ════════════════════════════════════════════════════════════════════════
    // STATE & AURA
    // ════════════════════════════════════════════════════════════════════════

    public void ToggleState()
    {
        currentState = (currentState == RobotState.ESTADO_A) ? RobotState.ESTADO_B : RobotState.ESTADO_A;
        ApplyAura();

        string msg = (currentState == RobotState.ESTADO_A) ? "PROTOCOLO LÓGICO ACTIVADO" : "PROTOCOLO EMOCIONAL ACTIVADO";
        onEmotionalLog?.Invoke(msg);
        CyberpunkUIManager.Instance?.UpdateStateMonitor(currentState);
    }

    void ApplyAura()
    {
        if (auraLight == null) return;
        auraLight.color = (currentState == RobotState.ESTADO_A) ? auraColorA : auraColorB;
        auraLight.intensity = auraIntensity;
    }

    // ════════════════════════════════════════════════════════════════════════
    // COMMAND EXECUTION
    // ════════════════════════════════════════════════════════════════════════

    public void ExecuteSequence(List<CommandType> commands)
    {
        StartCoroutine(ProcessSequence(commands));
    }

    private IEnumerator ProcessSequence(List<CommandType> commands)
    {
        foreach (var cmd in commands)
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
                    yield return new WaitForSeconds(1.0f);
                    break;

                case CommandType.CAMBIAR_ESTADO:
                    ToggleState();
                    break;
            }

            yield return new WaitForSeconds(0.35f);
        }

        GameManager.Instance?.OnSequenceComplete();
    }

    // ════════════════════════════════════════════════════════════════════════
    // MOVEMENT COROUTINES
    // ════════════════════════════════════════════════════════════════════════

    IEnumerator MoveForward()
    {
        SetWalking(true);
        Vector3 targetPos = transform.position + transform.forward * moveDistance;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        SetWalking(false);
    }

    IEnumerator Jump()
    {
        if (!_isGrounded) yield break;
        _isGrounded = false;

        if (animator != null) animator.SetTrigger("Jump");

        if (_rb != null)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        float elapsed = 0f;
        while (!_isGrounded && elapsed < 2f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void SetWalking(bool value)
    {
        if (animator != null) animator.SetBool("IsWalking", value);
    }

    // ════════════════════════════════════════════════════════════════════════
    // COLLISION & LOGGING
    // ════════════════════════════════════════════════════════════════════════

    // Este método es el que arregla el error CS1061 en StateBarrier.cs
    public void TriggerCollisionLog()
    {
        if (CollisionMessages.Length > 0)
        {
            string msg = CollisionMessages[Random.Range(0, CollisionMessages.Length)];
            onEmotionalLog?.Invoke(msg);
            CyberpunkUIManager.Instance?.ShowEmotionalLog(msg);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // Detectar suelo
        if (col.gameObject.CompareTag("Ground") || col.gameObject.layer == LayerMask.NameToLayer("Default"))
            _isGrounded = true;
    }

    // ════════════════════════════════════════════════════════════════════════
    // RESET
    // ════════════════════════════════════════════════════════════════════════

    public void ResetToStart()
    {
        StopAllCoroutines();
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        if (_rb != null) _rb.linearVelocity = Vector3.zero;
        SetWalking(false);
        if (animator != null) animator.Play("Idle");
        currentState = RobotState.ESTADO_A;
        ApplyAura();
        onEmotionalLog?.Invoke("SISTEMA REINICIADO");
        CyberpunkUIManager.Instance?.UpdateStateMonitor(currentState);
    }
}