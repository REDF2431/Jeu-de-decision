using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Describes a single task within a day.
/// Attach to any GameObject. Register it with DayManager automatically or manually.
///
/// ── Inspector layout ─────────────────────────────────────────────────────────
///
///  [Task Info]
///    Task Name        ← shown in DayManager's HUD list
///    Description      ← optional longer description / tooltip
///
///  [Morality]
///    Morality Delta   ← -1, 0, or +1 awarded when task completes
///
///  [On Task Start (day begins)]
///    OnTaskStarted    ← UnityEvent fired at the start of the day
///
///  [On Task Complete]
///    OnTaskCompleted  ← UnityEvent fired when MarkComplete() is called
///
/// ─────────────────────────────────────────────────────────────────────────────
/// </summary>
public class DayTask : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Inspector: info
    // ─────────────────────────────────────────────

    [Header("Task Info")]
    [Tooltip("Short name displayed in the day UI task list.")]
    public string taskName = "New Task";

    [Tooltip("Optional description shown to the player.")]
    [TextArea(2, 4)]
    public string description = "";

    // ─────────────────────────────────────────────
    //  Inspector: morality
    // ─────────────────────────────────────────────

    [Header("Morality")]
    [Tooltip("Points added to MoralityTracker when this task is completed.\n-1 = bad  |  0 = neutral  |  +1 = good")]
    [Range(-1, 1)]
    public int moralityDelta = 0;

    // ─────────────────────────────────────────────
    //  Inspector: events
    // ─────────────────────────────────────────────

    [Header("On Task Start (day begins)")]
    [Tooltip("Fired once when DayManager starts the day this task belongs to.")]
    public UnityEvent OnTaskStarted;

    [Header("On Task Complete")]
    [Tooltip("Fired when MarkComplete() is called on this task.")]
    public UnityEvent OnTaskCompleted;

    // ─────────────────────────────────────────────
    //  Runtime state
    // ─────────────────────────────────────────────

    /// <summary>True after MarkComplete() has been called.</summary>
    public bool IsCompleted { get; private set; }

    // ─────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Called by DayManager at the start of the day.
    /// Also fires OnTaskStarted so you can hook up dialogue, objectives, etc.
    /// </summary>
    public void StartTask()
    {
        IsCompleted = false;
        OnTaskStarted?.Invoke();
        Debug.Log($"[DayTask] '{taskName}' started.");
    }

    /// <summary>
    /// Call this from any game logic (trigger zone, dialogue outcome, etc.)
    /// to mark the task done and award morality.
    /// </summary>
    public void MarkComplete()
    {
        if (IsCompleted)
        {
            Debug.LogWarning($"[DayTask] '{taskName}' was already completed.");
            return;
        }

        IsCompleted = true;

        // Award morality
        if (MoralityTracker.Instance != null)
            MoralityTracker.Instance.AddMorality(moralityDelta);
        else
            Debug.LogWarning("[DayTask] MoralityTracker.Instance is null. Make sure MoralityTracker exists in the scene.");

        OnTaskCompleted?.Invoke();
        Debug.Log($"[DayTask] '{taskName}' completed. Morality delta: {moralityDelta:+0;-0}.");
    }

    /// <summary>Reset for replaying the day (called by DayManager if needed).</summary>
    public void ResetTask()
    {
        IsCompleted = false;
    }
}
