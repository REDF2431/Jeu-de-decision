using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Completes a linked DayTask when the player (or any tagged object) enters
/// the trigger collider on this GameObject.
///
/// ── Setup ────────────────────────────────────────────────────────────────────
/// 1. Add this component to a GameObject that has a Collider.
/// 2. Tick "Is Trigger" on that Collider.
/// 3. Assign the DayTask to complete in the Task field.
/// 4. Set Trigger Tag to the tag on your player (default: "Player").
/// 5. Optionally tick Hide On Trigger to make this object disappear on contact.
/// ─────────────────────────────────────────────────────────────────────────────
/// </summary>
public class TaskTrigger : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Inspector fields
    // ─────────────────────────────────────────────

    [Header("Task")]
    [Tooltip("The DayTask that will be marked complete when the trigger fires.")]
    public DayTask task;

    [Header("Trigger Filter")]
    [Tooltip("Only objects with this tag will activate the trigger. Must match your player's tag.")]
    public string triggerTag = "Player";

    [Tooltip("If true, the trigger can only fire once per day. Prevents double-completion.")]
    public bool triggerOnce = true;

    [Header("On Trigger")]
    [Tooltip("Hide this GameObject after the trigger fires (useful for pickup-style objects).")]
    public bool hideOnTrigger = false;

    [Tooltip("Optional: a different object to hide instead (e.g. a visual mesh separate from the collider).")]
    public GameObject hideTarget;

    [Tooltip("Extra UnityEvents to fire when the trigger activates.")]
    public UnityEvent OnTriggered;

    // ─────────────────────────────────────────────
    //  Runtime
    // ─────────────────────────────────────────────

    private bool _hasFired;

    private void OnEnable()
    {
        // Reset when the object is re-enabled (e.g. new day starts)
        _hasFired = false;
    }

    // ─────────────────────────────────────────────
    //  Collision detection
    // ─────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        if (!CanFire(other.tag)) return;
        Fire();
    }

    // Also support 2D colliders
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!CanFire(other.tag)) return;
        Fire();
    }

    // ─────────────────────────────────────────────
    //  Core logic
    // ─────────────────────────────────────────────

    private bool CanFire(string collidingTag)
    {
        if (triggerOnce && _hasFired) return false;
        if (!string.IsNullOrEmpty(triggerTag) && collidingTag != triggerTag) return false;
        return true;
    }

    private void Fire()
    {
        if (triggerOnce) _hasFired = true;

        // Complete the task
        if (task != null)
            task.MarkComplete();
        else
            Debug.LogWarning($"[TaskTrigger] '{name}' fired but no DayTask is assigned.", this);

        // Fire extra events
        OnTriggered?.Invoke();

        // Hide
        if (hideOnTrigger)
        {
            var obj = hideTarget != null ? hideTarget : gameObject;
            obj.SetActive(false);
        }

        Debug.Log($"[TaskTrigger] '{name}' triggered by player.");
    }

    // ─────────────────────────────────────────────
    //  Gizmos — show trigger zone in Scene view
    // ─────────────────────────────────────────────
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var col = GetComponent<Collider>();
        if (col == null) return;

        bool done = task != null && task.IsCompleted;
        Gizmos.color = done
            ? new Color(0.3f, 1f, 0.4f, 0.25f)
            : new Color(1f, 0.85f, 0.1f, 0.25f);

        Gizmos.matrix = transform.localToWorldMatrix;

        if (col is BoxCollider box)
            Gizmos.DrawCube(box.center, box.size);
        else if (col is SphereCollider sphere)
            Gizmos.DrawSphere(sphere.center, sphere.radius);
        else if (col is CapsuleCollider capsule)
            Gizmos.DrawSphere(capsule.center, capsule.radius);

        // Outline
        Gizmos.color = done
            ? new Color(0.3f, 1f, 0.4f, 0.8f)
            : new Color(1f, 0.85f, 0.1f, 0.8f);

        if (col is BoxCollider box2)
            Gizmos.DrawWireCube(box2.center, box2.size);
        else if (col is SphereCollider sphere2)
            Gizmos.DrawWireSphere(sphere2.center, sphere2.radius);
    }
#endif
}
