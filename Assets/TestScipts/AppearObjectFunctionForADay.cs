using UnityEngine;

public class AppearObjectFunctionForADay : MonoBehaviour
{
    public int appearOnDay = 1; // Set this in the Inspector
    public bool disappearAfterDay = false; // Should it vanish on Day 2?

    private DayManager dayManager;

    void Start()
    {
        dayManager = FindFirstObjectByType<DayManager>();
        CheckDay();
    }

    // Call this from the DayManager's "OnDayStart" event
    public void CheckDay()
    {
        if (dayManager == null) return;

        bool isCorrectDay = (dayManager.currentDay == appearOnDay);

        // If it's the right day, show it. If not, hide it.
        gameObject.SetActive(isCorrectDay);

        // Optional: Keep it visible if it's past the appear day
        if (!disappearAfterDay && dayManager.currentDay > appearOnDay)
        {
            gameObject.SetActive(true);
        }
    }
}
