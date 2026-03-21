using UnityEngine;

public class EndingActivator : MonoBehaviour
{
    [Header("Score Thresholds")]
    public int goodScore = 20;
    public int neutralScore = 10;

    [Header("Ending Objects (Already in Scene)")]
    public GameObject goodEndingObject;
    public GameObject neutralEndingObject;
    public GameObject badEndingObject;

    [SerializeField] string tagFilter = "Player";
    private bool hasTriggered = false;

    void Start()
    {
        // Ensure all endings are OFF when the game starts
        if (goodEndingObject != null) goodEndingObject.SetActive(false);
        if (neutralEndingObject != null) neutralEndingObject.SetActive(false);
        if (badEndingObject != null) badEndingObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag(tagFilter)) return;

        hasTriggered = true;
        ActivateEnding();

        // Destroy this "First" trigger so it doesn't check again
        Destroy(gameObject);
    }

    void ActivateEnding()
    {
        int score = ScoreManager.instance.score;

        if (score >= goodScore)
        {
            if (goodEndingObject != null) goodEndingObject.SetActive(true);
            Debug.Log("Activated GOOD ending");
        }
        else if (score >= neutralScore)
        {
            if (neutralEndingObject != null) neutralEndingObject.SetActive(true);
            Debug.Log("Activated NEUTRAL ending");
        }
        else
        {
            if (badEndingObject != null) badEndingObject.SetActive(true);
            Debug.Log("Activated BAD ending");
        }
    }
}