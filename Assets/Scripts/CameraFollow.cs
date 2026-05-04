using UnityEngine;

/// <summary>
/// Simple camera follow script that smoothly follows a target GameObject
/// Attach this to the Main Camera and assign the Robot as the target
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The GameObject the camera should follow (assign the Robot here)")]
    public Transform target;

    [Header("Camera Offset")]
    [Tooltip("Offset from the target position")]
    public Vector3 offset = new Vector3(0f, 2f, -8f);

    [Header("Smoothing")]
    [Tooltip("How smoothly the camera follows (lower = smoother, higher = more responsive)")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    [Header("Look At Target")]
    [Tooltip("Should the camera always look at the target?")]
    public bool lookAtTarget = true;

    [Tooltip("Offset for the look-at point (useful to look slightly above the target)")]
    public Vector3 lookAtOffset = new Vector3(0f, 1f, 0f);

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned! Please assign the Robot in the Inspector.");
            return;
        }

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between current position and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update camera position
        transform.position = smoothedPosition;

        // Optionally look at the target
        if (lookAtTarget)
        {
            Vector3 lookAtPoint = target.position + lookAtOffset;
            transform.LookAt(lookAtPoint);
        }
    }

    // Draw gizmos in the Scene view to visualize the camera setup
    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Draw line from camera to target
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, target.position);

        // Draw sphere at target position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position, 0.5f);

        // Draw sphere at desired camera position
        Gizmos.color = Color.cyan;
        Vector3 desiredPos = target.position + offset;
        Gizmos.DrawWireSphere(desiredPos, 0.3f);
    }
}
