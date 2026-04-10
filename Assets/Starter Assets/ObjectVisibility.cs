using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the visibility of a GameObject.
/// Can be triggered manually, from a UnityEvent (e.g. DayTask's OnTaskStarted),
/// or automatically on Start.
///
/// ── Common uses ──────────────────────────────────────────────────────────────
/// • Show an object at the start of a day  →  wire DayTask.OnTaskStarted to Show()
/// • Hide an object when a task completes  →  wire DayTask.OnTaskCompleted to Hide()
/// • Make something appear after a delay   →  call ShowAfterDelay(seconds)
/// • Toggle a door / prop from a button    →  wire a UI Button to Toggle()
/// ─────────────────────────────────────────────────────────────────────────────
/// </summary>
public class ObjectVisibility : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Inspector fields
    // ─────────────────────────────────────────────

    [Header("Target")]
    [Tooltip("The object to show or hide. Defaults to this GameObject if left empty.")]
    public GameObject target;

    [Header("Initial State")]
    [Tooltip("Whether the object should be visible when the scene starts.")]
    public bool visibleOnStart = false;

    [Header("Delay")]
    [Tooltip("Seconds to wait before showing the object when Show() or ShowAfterDelay() is called.")]
    [Min(0f)]
    public float showDelay = 0f;

    [Tooltip("Seconds to wait before hiding the object when Hide() is called.")]
    [Min(0f)]
    public float hideDelay = 0f;

    [Header("Events")]
    [Tooltip("Fired when the object becomes visible.")]
    public UnityEvent OnShown;

    [Tooltip("Fired when the object becomes hidden.")]
    public UnityEvent OnHidden;

    // ─────────────────────────────────────────────
    //  Runtime
    // ─────────────────────────────────────────────

    private Coroutine _pending;

    /// <summary>Whether the target is currently active/visible.</summary>
    public bool IsVisible => Target.activeSelf;

    private GameObject Target => target != null ? target : gameObject;

    // ─────────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        Target.SetActive(visibleOnStart);
    }

    // ─────────────────────────────────────────────
    //  Public API  (call from UnityEvents or code)
    // ─────────────────────────────────────────────

    /// <summary>Show the object, respecting showDelay.</summary>
    public void Show()
    {
        CancelPending();
        if (showDelay > 0f)
            _pending = StartCoroutine(DelayedSetActive(true, showDelay));
        else
            SetVisible(true);
    }

    /// <summary>Hide the object, respecting hideDelay.</summary>
    public void Hide()
    {
        CancelPending();
        if (hideDelay > 0f)
            _pending = StartCoroutine(DelayedSetActive(false, hideDelay));
        else
            SetVisible(false);
    }

    /// <summary>Toggle between shown and hidden.</summary>
    public void Toggle()
    {
        if (IsVisible) Hide(); else Show();
    }

    /// <summary>Show after a one-off custom delay, ignoring the showDelay field.</summary>
    public void ShowAfterDelay(float seconds)
    {
        CancelPending();
        _pending = StartCoroutine(DelayedSetActive(true, seconds));
    }

    /// <summary>Hide after a one-off custom delay, ignoring the hideDelay field.</summary>
    public void HideAfterDelay(float seconds)
    {
        CancelPending();
        _pending = StartCoroutine(DelayedSetActive(false, seconds));
    }

    // ─────────────────────────────────────────────
    //  Internal
    // ─────────────────────────────────────────────

    private void SetVisible(bool visible)
    {
        Target.SetActive(visible);
        if (visible) OnShown?.Invoke();
        else         OnHidden?.Invoke();
        Debug.Log($"[ObjectVisibility] '{Target.name}' → {(visible ? "shown" : "hidden")}");
    }

    private IEnumerator DelayedSetActive(bool visible, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetVisible(visible);
        _pending = null;
    }

    private void CancelPending()
    {
        if (_pending != null)
        {
            StopCoroutine(_pending);
            _pending = null;
        }
    }
}
