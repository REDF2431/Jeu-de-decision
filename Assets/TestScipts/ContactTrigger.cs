using UnityEngine;

public class ContactTrigger : MonoBehaviour
{
    public string taskNameToComplete;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your Player has the "Player" tag
        {
            // Find the TaskManager and tell it we're done!
            FindFirstObjectByType<TaskManager>().CompleteTask(taskNameToComplete);

            // Optional: Hide the cube after it's approached
            gameObject.SetActive(false);
        }
    }
}
