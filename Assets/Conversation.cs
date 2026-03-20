using UnityEngine;
using TMPro;
using System.Collections;

public class Conversation : MonoBehaviour
{
    public TextMeshProUGUI textOne;
    public TextMeshProUGUI textTwo;

    void Start()
    {
        //Hide text at the beginning
        textOne.gameObject.SetActive(false);
        textTwo.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(PlayConversation());
        }
        
    }

    IEnumerator PlayConversation()
    {
        // Sphere speaks first
        textOne.text = "Freak!";
        textOne.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        textOne.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        // Cube responds
        textTwo.text = "Screw you man.";
        textTwo.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        textTwo.gameObject.SetActive(false);
    }
}
