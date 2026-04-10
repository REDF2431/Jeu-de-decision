using UnityEngine;
using TMPro;
using UnityEngine.Events; // Needed for UnityEvent

public class DecisionManager : MonoBehaviour
{
    public int totalScore = 0;
    public TextMeshProUGUI scoreText;
    public GameObject decisionPanel; // Drag your UI Container here

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


}