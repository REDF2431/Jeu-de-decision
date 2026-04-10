using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Shows or hides GameObjects (tasks, props, NPCs, UI) based on the player's
/// current morality score from MoralityTracker.
///
/// ── How it works ─────────────────────────────────────────────────────────────
/// You define one or more MoralityCondition entries. Each entry has:
///   • A min/max score range
///   • A list of objects to SHOW when the score is in that range
///   • A list of objects to HIDE when the score is in that range
///   • Optional UnityEvents to fire
///
/// Call Evaluate() manually, or set Auto Evaluate On Day Start to true and
/// wire it to DayManager's OnAnyDayStart event so it runs every morning.
///
/// ── Example setup ────────────────────────────────────────────────────────────
/// Score ≤ -2  → show "dark path" tasks, hide "good path" tasks
/// Score = 0   → show neutral tasks
/// Score ≥ 2   → show "good path" tasks, hide "dark path" tasks
/// ─────────────────────────────────────────────────────────────────────────────
/// </summary>
public class MoralityVisibility : MonoBehaviour
{
    // =========================================================================
    //  Condition entry
    // =========================================================================
    [System.Serializable]
    public class MoralityCondition
    {
        [Tooltip("Label for this condition (Inspector only, not shown in game).")]
        public string label = "New Condition";

        [Header("Score Range")]
        [Tooltip("Minimum morality score (inclusive) for this condition to activate.")]
        public int minScore = -1;

        [Tooltip("Maximum morality score (inclusive) for this condition to activate.")]
        public int maxScore = 1;

        [Header("Objects to Show")]
        [Tooltip("These GameObjects will be SET ACTIVE when the condition is met.")]
        public GameObject[] showObjects;

        [Header("Objects to Hide")]
        [Tooltip("These GameObjects will be SET INACTIVE when the condition is met.")]
        public GameObject[] hideObjects;

        [Header("Events")]
        [Tooltip("Extra events to fire when this condition is met.")]
        public UnityEvent OnConditionMet;

        /// <summary>Returns true if <paramref name="score"/> falls within the range.</summary>
        public bool Matches(int score) => score >= minScore && score <= maxScore;
    }

    // ─────────────────────────────────────────────
    //  Inspector fields
    // ─────────────────────────────────────────────

    [Header("Conditions")]
    [Tooltip("Each entry defines what to show/hide for a given morality score range.\n" +
             "Conditions are checked top to bottom. ALL matching ones are applied.")]
    public MoralityCondition[] conditions;

    [Header("Fallback")]
    [Tooltip("Objects to show when NO condition matches the current score.")]
    public GameObject[] fallbackShowObjects;

    [Tooltip("Objects to hide when NO condition matches the current score.")]
    public GameObject[] fallbackHideObjects;

    [Header("Behaviour")]
    [Tooltip("Evaluate automatically when the scene starts.")]
    public bool evaluateOnStart = true;

    [Tooltip("If true, ALL matching conditions are applied.\n" +
             "If false, only the FIRST matching condition is applied.")]
    public bool applyAllMatching = false;

    // ─────────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        if (evaluateOnStart)
            Evaluate();
    }

    // ─────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Read the current morality score and apply all matching conditions.
    /// Call this from DayManager's OnAnyDayStart UnityEvent, or from any script.
    /// </summary>
    public void Evaluate()
    {
        if (MoralityTracker.Instance == null)
        {
            Debug.LogWarning("[MoralityVisibility] MoralityTracker.Instance is null. Make sure it exists in the scene.");
            return;
        }

        int score = MoralityTracker.Instance.Score;
        bool anyMatched = false;

        foreach (var condition in conditions)
        {
            if (!condition.Matches(score)) continue;

            anyMatched = true;
            ApplyCondition(condition, score);

            if (!applyAllMatching) break; // stop after first match
        }

        // Fallback
        if (!anyMatched)
        {
            Debug.Log($"[MoralityVisibility] No condition matched score {score}. Applying fallback.");
            SetActiveAll(fallbackShowObjects, true);
            SetActiveAll(fallbackHideObjects, false);
        }
    }

    // ─────────────────────────────────────────────
    //  Internal
    // ─────────────────────────────────────────────

    private void ApplyCondition(MoralityCondition condition, int score)
    {
        SetActiveAll(condition.showObjects, true);
        SetActiveAll(condition.hideObjects, false);
        condition.OnConditionMet?.Invoke();

        Debug.Log($"[MoralityVisibility] Condition '{condition.label}' matched (score={score}). " +
                  $"Showed {condition.showObjects?.Length ?? 0}, " +
                  $"hid {condition.hideObjects?.Length ?? 0} objects.");
    }

    private static void SetActiveAll(GameObject[] objects, bool active)
    {
        if (objects == null) return;
        foreach (var obj in objects)
            if (obj != null) obj.SetActive(active);
    }
}
