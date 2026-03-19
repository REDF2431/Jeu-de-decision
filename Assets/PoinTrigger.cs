using UnityEngine;

public class PoinTrigger : MonoBehaviour
{
    public int pointsToAdd = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming you have a GameManager script to handle the score
            GameManager.instance.AddPoints(pointsToAdd);
            Destroy(gameObject); // Destroy the point object after collecting
        }
    }


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
