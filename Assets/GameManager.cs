using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int totalPoints = 0;

    void awake()
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

    public void AddPoints(int pointsToAdd)
    {
        totalPoints += pointsToAdd;
        Debug.Log("Total Points: " + totalPoints);
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
