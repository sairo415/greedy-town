using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider coll;

    void Awake()
    {
        coll = GetComponent<Collider>();
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Area"))
            return;


        Vector3 playerPos = VamsuGameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;


        Vector3 playerDir = VamsuGameManager.instance.player.inputVec;
        float dirX = playerPos.x - myPos.x;
        float dirZ = playerPos.z - myPos.z;

        float diffX = Mathf.Abs(dirX);
        float diffZ = Mathf.Abs(dirZ);

        dirX = dirX > 0 ? 1 : -1;
        dirZ = dirZ > 0 ? 1 : -1;

        switch (transform.tag)
        {
            case "Ground":
                if(diffX > diffZ)
                {
                    transform.Translate(Vector3.right * dirX * 120);
                }
                else if (diffX < diffZ)
                {
                    transform.Translate(Vector3.forward * dirZ * 120);
                }
                break;

            case "Enemy":
                if (coll.enabled)
                {
                    transform.position = playerPos + (playerDir * 45 + new Vector3(Random.Range(-3f,3f),0, Random.Range(-3f,3f)));
                }
                break; 
        }
    }
}
