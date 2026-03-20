using System;
using System.Collections;
using UnityEngine;


public class ShowUI : MonoBehaviour
{
    public GameObject uiObject;

    void Start()
    {
              
            uiObject.SetActive(false);
        
    }

    void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            if (uiObject != null)
            {
                uiObject.SetActive(true);
                
                StartCoroutine(WaitforSec());
            }
            ; 
        }
    }


    IEnumerator WaitforSec()
    {
        yield return new WaitForSeconds(5);

        if (uiObject != null)
        {
            
            uiObject.SetActive(false);
        }

        
        Destroy(gameObject);
    }
}
