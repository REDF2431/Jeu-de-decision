using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    [System.Serializable]
    public class Task
    {
        public string taskName;
        public bool isCompleted;
    }

    [System.Serializable]
    public class DayData
    {
        public string dayLabel; // e.g., "Monday"
        public List<Task> tasksForThisDay = new List<Task>();
    }

    // This creates the "List of Days" you see in the Inspector
    public List<DayData> allDays = new List<DayData>();

    // We'll use this to keep track of which day's tasks to look at
    private int currentDayIndex = 0;

    public void LoadDay(int dayNumber)
    {
        // Day 1 is index 0, Day 2 is index 1, etc.
        currentDayIndex = Mathf.Clamp(dayNumber - 1, 0, allDays.Count - 1);
    }

    public void CompleteTask(string name)
    {
        foreach (var task in allDays[currentDayIndex].tasksForThisDay)
        {
            if (task.taskName == name)
            {
                task.isCompleted = true;
                return;
            }
        }
    }

    public bool AreAllTasksDone()
    {
        // Only checks the tasks for the CURRENT active day
        var currentTasks = allDays[currentDayIndex].tasksForThisDay;
        return currentTasks.FindAll(t => t.isCompleted == false).Count == 0;
    }

    public void ResetAllTasks()
    {
        foreach (var day in allDays)
        {
            foreach (var task in day.tasksForThisDay) task.isCompleted = false;
        }
    }
}
