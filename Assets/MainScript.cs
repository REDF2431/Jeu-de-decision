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

    [Header("Score Settings")]
    public int pointsToGive = 1; // You can change this in the Inspector

    private bool hasTriggered = false;

    void Start()
    {
        if (uiObject != null) uiObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (!String.IsNullOrEmpty(tagFilter) && !other.CompareTag(tagFilter)) return;

        hasTriggered = true;

        // --- NEW POINT LOGIC ---
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(pointsToGive);
        }
        // -----------------------

        // 1. Run your Spawning (via the UnityEvent in Inspector)
        onTriggerEnter.Invoke();

        // 2. Run the UI Logic
        if (uiObject != null)
        {
            StartCoroutine(HandleUI());
        }
        else if (destroyOnTriggerEnter)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator HandleUI()
    {
        uiObject.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        uiObject.SetActive(false);

        if (destroyOnTriggerEnter)
        {
            Destroy(gameObject);
        }
    }
}