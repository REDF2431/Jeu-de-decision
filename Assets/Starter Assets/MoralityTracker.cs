using UnityEngine;

/// <summary>
/// Singleton that persists across scenes and stores the player's morality score.
/// Access from anywhere via MoralityTracker.Instance.Score
/// Range is unclamped — clamp at the ending as needed.
/// </summary>
public class MoralityTracker : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Singleton
    // ─────────────────────────────────────────────
    public static MoralityTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _score = 0;
    }

    // ─────────────────────────────────────────────
    //  Score
    // ─────────────────────────────────────────────
    private int _score;

    /// <summary>Current cumulative morality score.</summary>
    public int Score => _score;

    /// <summary>Add a morality delta (typically -1, 0, or +1).</summary>
    public void AddMorality(int delta)
    {
        _score += delta;
        Debug.Log($"[MoralityTracker] Score changed by {delta:+0;-0}. Total: {_score}");
    }

    /// <summary>Hard-set the score (useful for save/load).</summary>
    public void SetScore(int value) => _score = value;

    /// <summary>Reset to zero (e.g. new game).</summary>
    public void ResetScore() => _score = 0;
}
