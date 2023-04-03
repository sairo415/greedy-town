using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAlbinoJumpTarget : MonoBehaviour
{
    public bool isTargetOn;

    void OnTriggerEnter(Collider other)
    {
        if(other != null && other.tag == "Player")
        {
            Debug.Log("PLayer Tartget on");
            isTargetOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other != null && other.tag == "Player")
        {
            isTargetOn = false;
        }
    }
}
