using UnityEngine;

/// <summary>
/// Energy barrier for "Dicotomía de Datos" (Level 3+).
///
/// Physics layer approach (recommended):
///   - Kyle's layer:       "Player"
///   - Red barriers layer: "RedEnergy"
///   - Blue barriers layer:"BlueEnergy"
///   Configure the Physics Matrix so:
///     Player ↔ RedEnergy  → collide only when Kyle is ESTADO_A
///     Player ↔ BlueEnergy → collide only when Kyle is ESTADO_B
///
/// This script handles the VISUAL color and the TRIGGER-based log feedback.
/// The actual pass-through is controlled by the Physics Collision Matrix
/// (see Manual Checklist) combined with the layer-swap in RobotController3D.
/// </summary>
[RequireComponent(typeof(Collider))]
public class StateBarrier : MonoBehaviour
{
    [Header("Which state is BLOCKED by this barrier?")]
    [Tooltip("ESTADO_A = Red barrier blocks Logic mode.\nESTADO_B = Blue barrier blocks Emotional mode.")]
    public RobotController3D.RobotState blocksState;

    [Header("Renderer")]
    public Renderer barrierRenderer;

    [Header("Colors")]
    public Color colorA = new Color(1f, 0.15f, 0.10f, 0.75f);   // Red
    public Color colorB = new Color(0.15f, 0.50f, 1.00f, 0.75f); // Blue

    [Header("Emission intensity")]
    public float emissionIntensity = 2.5f;

    // ── Private ──────────────────────────────────────────────────────────────

    private Collider           _col;
    private MaterialPropertyBlock _mpb;

    // ════════════════════════════════════════════════════════════════════════
    // LIFECYCLE
    // ════════════════════════════════════════════════════════════════════════

    void Start()
    {
        _col = GetComponent<Collider>();
        _mpb = new MaterialPropertyBlock();

        ApplyVisual();

        // Tag so RobotController3D can identify barrier type on collision
        gameObject.tag = (blocksState == RobotController3D.RobotState.ESTADO_A)
            ? "BarreraA"
            : "BarreraB";
    }

    // ════════════════════════════════════════════════════════════════════════
    // VISUAL
    // ════════════════════════════════════════════════════════════════════════

    void ApplyVisual()
    {
        if (barrierRenderer == null) return;

        bool isA  = (blocksState == RobotController3D.RobotState.ESTADO_A);
        Color col = isA ? colorA : colorB;

        barrierRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_BaseColor", col);
        _mpb.SetColor("_Color",     col);
        _mpb.SetColor("_EmissionColor", col * emissionIntensity);
        barrierRenderer.SetPropertyBlock(_mpb);
    }

    // ════════════════════════════════════════════════════════════════════════
    // COLLISION / TRIGGER
    // ════════════════════════════════════════════════════════════════════════

    void OnCollisionEnter(Collision col)
    {
        HandleContact(col.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleContact(other.gameObject);
    }

    void HandleContact(GameObject other)
    {
        if (!other.CompareTag("Player")) return;

        var robot = other.GetComponent<RobotController3D>();
        if (robot == null) return;

        if (robot.currentState == blocksState)
        {
            // Robot is in the blocked state – trigger emotional log
            robot.TriggerCollisionLog();
        }
    }
}
