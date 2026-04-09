using UnityEngine;
using UnityEngine.Events;

public class DayManagerOld : MonoBehaviour
{
    public int currentDay = 1;
    public TaskManager taskManager;

    [Header("Day Events")]
    public UnityEvent onDayStart;
    public UnityEvent onDayEnd;

    public void GoToBed()
    {
        if (taskManager != null && taskManager.AreAllTasksDone())
        {
            EndDay();
        }
        else
        {
            Debug.Log("You still have chores to do!");
        }
    }

    private void EndDay()
    {
        onDayEnd.Invoke();
        currentDay++;
        // No need to reset tasks here; LoadDay handles the switch
        StartNewDay();
    }

    private void StartNewDay()
    {
        // Tell the TaskManager to focus on the new day's list
        taskManager.LoadDay(currentDay);
        onDayStart.Invoke();
    }
}
