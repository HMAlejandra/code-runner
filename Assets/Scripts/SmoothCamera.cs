using UnityEngine;

/// <summary>
/// Cinematic smooth-follow camera for Robot Kyle.
/// Uses Vector3.Lerp for position and Quaternion.Slerp for rotation
/// to eliminate jitter during movement and jumps.
///
/// ── Recommended Inspector values ──────────────────────────────────────────
///   Offset      X = 0   Y = 4.5   Z = -7
///   SmoothSpeed = 6
///   LookOffset  Y = 1.2  (looks slightly above Kyle's feet)
/// ──────────────────────────────────────────────────────────────────────────
/// </summary>
public class SmoothCamera : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Drag Robot Kyle's Transform here.")]
    public Transform target;

    [Header("Offset (world-space from target)")]
    [Tooltip("Recommended: X=0  Y=4.5  Z=-7")]
    public Vector3 offset = new Vector3(0f, 4.5f, -7f);

    [Header("Smoothing")]
    [Tooltip("Higher = snappier. Recommended: 6")]
    [Range(1f, 20f)]
    public float smoothSpeed = 6f;

    [Header("Look-at offset")]
    [Tooltip("Shifts the look-at point upward so Kyle is framed nicely.")]
    public float lookHeightOffset = 1.2f;

    [Header("Rotation smoothing")]
    [Range(1f, 20f)]
    public float rotationSmoothSpeed = 5f;

    // ── Private ──────────────────────────────────────────────────────────────

    private Vector3    _currentVelocity; // used by SmoothDamp fallback
    private bool       _initialized;

    // ════════════════════════════════════════════════════════════════════════
    // LIFECYCLE
    // ════════════════════════════════════════════════════════════════════════

    void Start()
    {
        if (target == null)
        {
            // Auto-find Kyle if not assigned
            var go = GameObject.FindWithTag("Player");
            if (go != null) target = go.transform;
        }

        if (target != null)
        {
            // Snap to desired position on first frame (no lerp lag at start)
            transform.position = target.position + offset;
            LookAtTarget();
            _initialized = true;
        }
    }

    // LateUpdate runs after all movement, so the camera never lags a frame behind
    void LateUpdate()
    {
        if (target == null) return;

        // ── Position ─────────────────────────────────────────────────────────
        Vector3 desiredPosition = target.position + offset;

        transform.position = _initialized
            ? Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime)
            : desiredPosition;

        // ── Rotation ─────────────────────────────────────────────────────────
        LookAtTarget();

        _initialized = true;
    }

    void LookAtTarget()
    {
        if (target == null) return;

        Vector3 lookPoint  = target.position + Vector3.up * lookHeightOffset;
        Vector3 direction  = lookPoint - transform.position;

        if (direction == Vector3.zero) return;

        Quaternion desiredRot = Quaternion.LookRotation(direction);
        transform.rotation    = Quaternion.Slerp(
            transform.rotation, desiredRot, rotationSmoothSpeed * Time.deltaTime);
    }

    // ── Gizmos ───────────────────────────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.position + Vector3.up * lookHeightOffset, 0.3f);
    }
}
