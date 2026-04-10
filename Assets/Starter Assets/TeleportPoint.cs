using UnityEngine;

/// <summary>
/// Defines a world-space teleport destination (position + facing direction).
/// Attach to any empty GameObject and set the fields in the Inspector.
/// Call Teleport(target) or Teleport(target, useRotation) from DayManager or other scripts.
///
/// Compatible with DayManager: assign this component in the "Spawn Point" field
/// of each DayData entry to teleport the player at the start of that day.
/// </summary>
public class TeleportPoint : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Inspector fields
    // ─────────────────────────────────────────────

    [Header("Destination")]
    [Tooltip("World-space position the target will be moved to.")]
    public Vector3 position;

    [Tooltip("If true, also apply the facing direction below.")]
    public bool applyRotation = true;

    [Header("Facing Direction")]
    [Tooltip("Horizontal angle (Y-axis, degrees). 0 = world forward (+Z). 90 = right (+X).")]
    [Range(0f, 360f)]
    public float yawDegrees = 0f;

    [Tooltip("Vertical tilt (X-axis, degrees). Positive = look down.")]
    [Range(-89f, 89f)]
    public float pitchDegrees = 0f;

    // ─────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────

    /// <summary>Teleport <paramref name="target"/> to this point's position and optional rotation.</summary>
    public void Teleport(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("[TeleportPoint] Teleport called with a null target.", this);
            return;
        }

        // Move a CharacterController-based player safely (disable → move → re-enable)
        var cc = target.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        target.position = position;

        if (applyRotation)
            target.rotation = Quaternion.Euler(pitchDegrees, yawDegrees, 0f);

        if (cc != null) cc.enabled = true;

        Debug.Log($"[TeleportPoint] Teleported '{target.name}' to {position}, yaw={yawDegrees}°, pitch={pitchDegrees}°.");
    }

    /// <summary>Teleport and optionally override the rotation setting.</summary>
    public void Teleport(Transform target, bool forceApplyRotation)
    {
        bool saved = applyRotation;
        applyRotation = forceApplyRotation;
        Teleport(target);
        applyRotation = saved;
    }

    // ─────────────────────────────────────────────
    //  Gizmos — visible in Scene view
    // ─────────────────────────────────────────────
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.6f);
        Gizmos.DrawSphere(position, 0.25f);

        if (applyRotation)
        {
            Gizmos.color = new Color(1f, 0.6f, 0.1f, 0.9f);
            var dir = Quaternion.Euler(pitchDegrees, yawDegrees, 0f) * Vector3.forward;
            Gizmos.DrawRay(position, dir * 1.5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        UnityEditor.Handles.DrawWireDisc(position, Vector3.up, 0.5f);
        UnityEditor.Handles.Label(position + Vector3.up * 0.6f,
            $"  ↑ TeleportPoint\n  Yaw {yawDegrees}°  Pitch {pitchDegrees}°");
    }
#endif
}
