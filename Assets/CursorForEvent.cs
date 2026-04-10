using UnityEngine;

using UnityEngine;
using UnityEngine.UI; // Required if using a UI Button

public class CursorForEvent : MonoBehaviour
{
        // This runs when the Player enters the Trigger Zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UnlockCursor();
        }
    }

    // Call this function from your UI Button "OnClick" event
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor Locked");
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor Freed");
    }
}