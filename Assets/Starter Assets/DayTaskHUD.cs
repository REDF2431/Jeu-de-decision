using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Drives a Unity UI panel that shows the current day label and
/// a live list of pending / completed tasks.
///
/// ── Setup ────────────────────────────────────────────────────────────────────
/// 1. Create a Canvas (if you don't have one).
/// 2. Inside it, build this hierarchy:
///
///      [Panel] TaskHUD
///        [TextMeshProUGUI]  DayLabel          ← "Day 1 — Arrival"
///        [ScrollRect]       TaskScroll        ← optional, for many tasks
///          [Content]        TaskListContent   ← assign to taskListParent
///        [TextMeshProUGUI]  AllDoneMessage    ← "All tasks complete!"
///
/// 3. Create a Prefab for a single task row:
///
///      [GameObject] TaskRow  (assign to taskRowPrefab)
///        [TextMeshProUGUI]  TaskText   ← needs a tag OR be the first TMP child
///
/// 4. Attach this script to TaskHUD and assign the fields below.
/// ─────────────────────────────────────────────────────────────────────────────
/// </summary>
public class DayTaskHUD : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Inspector fields
    // ─────────────────────────────────────────────

    [Header("References")]
    [Tooltip("TextMeshPro text that shows the current day label.")]
    public TMP_Text dayLabelText;

    [Tooltip("Parent transform where task row prefabs are spawned (e.g. the Content of a ScrollRect).")]
    public Transform taskListParent;

    [Tooltip("Prefab for a single task row. Must have a TMP_Text child for the task name.")]
    public GameObject taskRowPrefab;

    [Tooltip("Optional: a message shown when all tasks are complete.")]
    public GameObject allDoneMessage;

    [Header("Appearance")]
    [Tooltip("Color for a task that is still pending.")]
    public Color pendingColor = Color.white;

    [Tooltip("Color for a task that has been completed.")]
    public Color completedColor = new Color(0.4f, 0.85f, 0.4f);

    [Tooltip("Prefix added to completed task names (e.g. a checkmark).")]
    public string completedPrefix = "✓  ";

    [Tooltip("Prefix added to pending task names.")]
    public string pendingPrefix = "•  ";

    [Header("Refresh")]
    [Tooltip("How often (in seconds) the list is refreshed. 0.1 is smooth without being wasteful.")]
    [Range(0.05f, 1f)]
    public float refreshInterval = 0.1f;

    // ─────────────────────────────────────────────
    //  Runtime
    // ─────────────────────────────────────────────

    private float _timer;
    private readonly List<TMP_Text> _rowTexts = new();

    // ─────────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        if (allDoneMessage != null)
            allDoneMessage.SetActive(false);

        RefreshUI();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= refreshInterval)
        {
            _timer = 0f;
            RefreshUI();
        }
    }

    // ─────────────────────────────────────────────
    //  Core refresh
    // ─────────────────────────────────────────────

    private void RefreshUI()
    {
        var dm = DayManager.Instance;
        if (dm == null || dm.CurrentDay == null) return;

        // Day label
        if (dayLabelText != null)
            dayLabelText.text = dm.CurrentDay.dayLabel;

        // Rebuild task rows to match current day's task count
        var tasks = dm.CurrentDay.tasks;
        EnsureRowCount(tasks.Count);

        bool allDone = true;

        for (int i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];
            if (task == null) continue;

            bool done = task.IsCompleted;
            if (!done) allDone = false;

            var txt = _rowTexts[i];
            txt.text = (done ? completedPrefix : pendingPrefix) + task.taskName;
            txt.color = done ? completedColor : pendingColor;
        }

        // All-done message
        if (allDoneMessage != null)
            allDoneMessage.SetActive(allDone && tasks.Count > 0);
    }

    // ─────────────────────────────────────────────
    //  Row pool — spawn or reuse rows
    // ─────────────────────────────────────────────

    private void EnsureRowCount(int needed)
    {
        // Spawn missing rows
        while (_rowTexts.Count < needed)
        {
            var row = Instantiate(taskRowPrefab, taskListParent);
            var txt = row.GetComponentInChildren<TMP_Text>();
            if (txt == null)
            {
                Debug.LogError("[DayTaskHUD] taskRowPrefab has no TMP_Text child.");
                Destroy(row);
                return;
            }
            _rowTexts.Add(txt);
        }

        // Show/hide rows based on how many tasks today has
        for (int i = 0; i < _rowTexts.Count; i++)
            _rowTexts[i].transform.parent.gameObject.SetActive(i < needed);
    }

    // ─────────────────────────────────────────────
    //  Public helpers
    // ─────────────────────────────────────────────

    /// <summary>Toggle the whole HUD panel on/off (e.g. from a pause menu).</summary>
    public void SetVisible(bool visible) => gameObject.SetActive(visible);
}
