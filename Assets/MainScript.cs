using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MasterTrigger : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] bool destroyOnTriggerEnter;
    [SerializeField] string tagFilter = "Player";
    [SerializeField] UnityEvent onTriggerEnter;

    [Header("UI Settings")]
    public GameObject uiObject;
    public float displayTime = 5f;

    private bool hasTriggered = false;

    void Start()
    {
        if (uiObject != null) uiObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // Prevent double-triggering while the Coroutine is running
        if (hasTriggered) return;

        if (!String.IsNullOrEmpty(tagFilter) && !other.CompareTag(tagFilter)) return;

        hasTriggered = true;

        // 1. Run your Spawning (via the UnityEvent in Inspector)
        onTriggerEnter.Invoke();

        // 2. Run the UI Logic
        if (uiObject != null)
        {
            StartCoroutine(HandleUI());
        }
        else if (destroyOnTriggerEnter)
        {
            // If there's no UI to wait for, destroy immediately
            Destroy(gameObject);
        }
    }

    IEnumerator HandleUI()
    {
        uiObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        uiObject.SetActive(false);

        // 3. NOW destroy the trigger, after the UI is done!
        if (destroyOnTriggerEnter)
        {
            Destroy(gameObject);
        }
    }
}