using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Controls the day-based progression system.
///
/// ── Quick setup ──────────────────────────────────────────────────────────────
/// 1. Add this component to a manager GameObject.
/// 2. Add one or more DayData entries in the Inspector.
/// 3. For each day, assign DayTask components and (optionally) a TeleportPoint.
/// 4. Wire the "Player Transform" to your player.
/// 5. Plug UnityEvents into "On Day Start" / "On Day End" per day, or use the
///    global hooks at the bottom of the Inspector.
/// 6. Call DayManager.Instance.CompleteCurrentDay() or let AllTasksDone() auto-advance.
///
/// ── Flow ─────────────────────────────────────────────────────────────────────
///  StartDay(n)
///    → Teleport player (if TeleportPoint assigned)
///    → Fire global OnAnyDayStart
///    → Fire per-day OnDayStart event
///    → Call StartTask() on every DayTask
///
///  When all tasks are complete:
///    → Fire per-day OnDayEnd event
///    → Fire global OnAnyDayEnd
///    → Advance to next day  (or fire OnAllDaysCompleted)
///
/// ─────────────────────────────────────────────────────────────────────────────
/// </summary>
public class DayManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Singleton
    // ─────────────────────────────────────────────
    public static DayManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // =========================================================================
    //  DayData — one entry per day
    // =========================================================================
    [System.Serializable]
    public class DayData
    {
        [Tooltip("Label shown in the Inspector and UI (e.g. 'Day 1 — Arrival').")]
        public string dayLabel = "Day 1";

        [Header("Player Spawn")]
        [Tooltip("Where the player is teleported at the start of this day. Leave empty to skip.")]
        public TeleportPoint spawnPoint;

        [Header("Tasks")]
        [Tooltip("All DayTask components that must be completed before this day ends.")]
        public List<DayTask> tasks = new();

        [Header("Day Events")]
        [Tooltip("Code / objects to activate at the START of this day.")]
        public UnityEvent OnDayStart;

        [Tooltip("Code / objects to activate when all tasks are done and the day ENDS.")]
        public UnityEvent OnDayEnd;
    }

    // ─────────────────────────────────────────────
    //  Inspector fields
    // ─────────────────────────────────────────────

    [Header("Days")]
    [Tooltip("Add one DayData entry per day. Order = Day 0, Day 1, Day 2 …")]
    public List<DayData> days = new();

    [Header("Player")]
    [Tooltip("The Transform that will be teleported. Usually the player root.")]
    public Transform playerTransform;

    [Header("Auto-advance")]
    [Tooltip("When true, the manager automatically moves to the next day as soon as all tasks are complete.")]
    public bool autoAdvance = true;

    [Header("Global Day Hooks  (run for every day)")]
    [Tooltip("Fired at the start of every day, before the per-day OnDayStart.")]
    public UnityEvent OnAnyDayStart;

    [Tooltip("Fired at the end of every day, after the per-day OnDayEnd.")]
    public UnityEvent OnAnyDayEnd;

    [Tooltip("Fired after the last day ends.")]
    public UnityEvent OnAllDaysCompleted;

    // ─────────────────────────────────────────────
    //  Runtime state
    // ─────────────────────────────────────────────

    /// <summary>Zero-based index of the currently active day.</summary>
    public int CurrentDayIndex { get; private set; } = -1;

    /// <summary>The currently active DayData, or null if no day is running.</summary>
    public DayData CurrentDay => (CurrentDayIndex >= 0 && CurrentDayIndex < days.Count)
        ? days[CurrentDayIndex] : null;

    // ─────────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        if (days.Count > 0)
            StartDay(0);
        else
            Debug.LogWarning("[DayManager] No days configured. Add DayData entries in the Inspector.");
    }

    private void Update()
    {
        if (autoAdvance && CurrentDay != null && AllTasksDone())
            CompleteCurrentDay();
    }

    // ─────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────

    /// <summary>Start the day at <paramref name="index"/>.</summary>
    public void StartDay(int index)
    {
        if (index < 0 || index >= days.Count)
        {
            Debug.LogWarning($"[DayManager] Day index {index} is out of range (0–{days.Count - 1}).");
            return;
        }

        CurrentDayIndex = index;
        var day = days[index];

        // Teleport player
        if (day.spawnPoint != null && playerTransform != null)
            day.spawnPoint.Teleport(playerTransform);
        else if (day.spawnPoint != null && playerTransform == null)
            Debug.LogWarning("[DayManager] SpawnPoint set but Player Transform is unassigned.");

        // Events
        OnAnyDayStart?.Invoke();
        day.OnDayStart?.Invoke();

        // Start every task
        foreach (var task in day.tasks)
            if (task != null) task.StartTask();

        Debug.Log($"[DayManager] ▶ {day.dayLabel} started. Tasks: {day.tasks.Count}");
    }

    /// <summary>
    /// Manually end the current day. If autoAdvance is on this is called automatically;
    /// call it yourself if you want a cutscene gate or confirmation dialog.
    /// </summary>
    public void CompleteCurrentDay()
    {
        if (CurrentDay == null) return;

        if (!AllTasksDone())
        {
            Debug.Log("[DayManager] Cannot end day — unfinished tasks remain.");
            LogPendingTasks();
            return;
        }

        var day = CurrentDay;
        day.OnDayEnd?.Invoke();
        OnAnyDayEnd?.Invoke();
        Debug.Log($"[DayManager] ✓ {day.dayLabel} completed.");

        int next = CurrentDayIndex + 1;
        if (next < days.Count)
            StartDay(next);
        else
        {
            OnAllDaysCompleted?.Invoke();
            Debug.Log("[DayManager] All days completed.");
        }
    }

    /// <summary>Returns true when every task in the current day is complete.</summary>
    public bool AllTasksDone()
    {
        if (CurrentDay == null) return false;
        foreach (var task in CurrentDay.tasks)
            if (task != null && !task.IsCompleted) return false;
        return true;
    }

    /// <summary>Returns a list of task names that are not yet done (for UI display).</summary>
    public List<string> GetPendingTaskNames()
    {
        var result = new List<string>();
        if (CurrentDay == null) return result;
        foreach (var task in CurrentDay.tasks)
            if (task != null && !task.IsCompleted)
                result.Add(task.taskName);
        return result;
    }

    // ─────────────────────────────────────────────
    //  Internal helpers
    // ─────────────────────────────────────────────

    private void LogPendingTasks()
    {
        foreach (var name in GetPendingTaskNames())
            Debug.Log($"  [DayManager]   • Pending: {name}");
    }

    // ─────────────────────────────────────────────
    //  Simple built-in GUI (visible in Game view)
    // ─────────────────────────────────────────────

    [Header("Debug HUD")]
    [Tooltip("Show the pending task list as an on-screen GUI overlay (useful during development).")]
    public bool showDebugHUD = true;

    private void OnGUI()
    {
        if (!showDebugHUD || CurrentDay == null) return;

        var pending = GetPendingTaskNames();
        int lineH = 22;
        int w = 260, h = 30 + pending.Count * lineH + 10;
        GUI.Box(new Rect(10, 10, w, h), "");

        var style = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold };
        GUI.Label(new Rect(18, 14, w - 20, 22), CurrentDay.dayLabel + " — Pending tasks", style);

        style.fontStyle = FontStyle.Normal;
        for (int i = 0; i < pending.Count; i++)
            GUI.Label(new Rect(24, 34 + i * lineH, w - 30, lineH), "• " + pending[i], style);

        if (pending.Count == 0)
        {
            style.normal.textColor = Color.green;
            GUI.Label(new Rect(24, 34, w - 30, lineH), "✓ All tasks complete!", style);
        }
    }
}

// =============================================================================
//  Custom Editor — makes the Inspector much more readable
// =============================================================================
#if UNITY_EDITOR
[CustomEditor(typeof(DayManager))]
public class DayManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var dm = (DayManager)target;

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("── Runtime Controls ──", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play mode to use runtime controls.", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField($"Current day: {dm.CurrentDayIndex}  " +
            $"({(dm.CurrentDay != null ? dm.CurrentDay.dayLabel : "—")})",
            EditorStyles.helpBox);

        var pending = dm.GetPendingTaskNames();
        if (pending.Count > 0)
        {
            EditorGUILayout.LabelField("Pending tasks:", EditorStyles.miniLabel);
            foreach (var t in pending)
                EditorGUILayout.LabelField("  • " + t, EditorStyles.miniLabel);
        }
        else if (dm.CurrentDay != null)
            EditorGUILayout.LabelField("✓ All tasks complete!", EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("◀ Previous Day") && dm.CurrentDayIndex > 0)
            dm.StartDay(dm.CurrentDayIndex - 1);
        if (GUILayout.Button("Complete Day"))
            dm.CompleteCurrentDay();
        if (GUILayout.Button("Next Day ▶"))
            dm.StartDay(dm.CurrentDayIndex + 1);
        EditorGUILayout.EndHorizontal();
    }
}
#endif
