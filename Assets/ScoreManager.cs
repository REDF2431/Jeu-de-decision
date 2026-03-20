using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    
    public static ScoreManager instance;

    [Header("Game Data")]
    public int score = 0;

    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
           
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Current Score: " + score); 
    }
}