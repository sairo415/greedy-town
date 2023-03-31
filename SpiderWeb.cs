using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    void Awake()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            VamsuGameManager.instance.player.speed = VamsuGameManager.instance.player.baseSpeed * 0.7f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            VamsuGameManager.instance.player.speed = VamsuGameManager.instance.player.baseSpeed;
        }
    }

    public void WebRemove()
    {
        VamsuGameManager.instance.player.speed = VamsuGameManager.instance.player.baseSpeed;
        transform.gameObject.SetActive(false);
    }
}
