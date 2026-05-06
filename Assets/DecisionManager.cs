using UnityEngine;
using TMPro;
using UnityEngine.Events; // Needed for UnityEvent

public class DecisionManager : MonoBehaviour
{
    public int totalScore = 0;
    public TextMeshProUGUI scoreText;
    public GameObject decisionPanel; // Drag your UI Container here
    public GameObject targetPositiveEnding;
    public GameObject targetNeutralEnding;
    public GameObject targetNegativeEnding;

    public void MakeDecision(int pointValue)
    {
        // 1. Update Score
        totalScore += pointValue;
        if (scoreText != null)
            scoreText.text = "Score: " + totalScore;

        // 2. Hide the UI
        if (decisionPanel != null)
            decisionPanel.SetActive(false);

        Debug.Log("Choice made. Points: " + pointValue);
    }

    public void ResetDecisions()
    {
        // Reset score
        totalScore = 0;
        if (scoreText != null)
            scoreText.text = "Score: " + totalScore;

        // Hide all ending screens
        if (targetPositiveEnding != null) targetPositiveEnding.SetActive(false);
        if (targetNeutralEnding != null) targetNeutralEnding.SetActive(false);
        if (targetNegativeEnding != null) targetNegativeEnding.SetActive(false);

        // Re-show the decision panel
        if (decisionPanel != null)
            decisionPanel.SetActive(true);

        Debug.Log("[DecisionManager] ↺ Reset. Score cleared, decision panel restored.");
    }

    public void startRightEnding()
    {
        if (totalScore == 0)
        {
            if (targetNeutralEnding  != null)
                targetNeutralEnding.SetActive(true);
        }
        else if (totalScore < 0)
        {
            if (targetNegativeEnding != null)
                targetNegativeEnding.SetActive(true);
        }
        else if (totalScore > 0)
        {
            if (targetPositiveEnding != null)
                targetPositiveEnding.SetActive(true);
        }
    }
}