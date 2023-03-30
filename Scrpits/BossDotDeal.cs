using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDotDeal : MonoBehaviour
{
    public int damageAmount;
    public float damageInterval;
    bool inLava = false;

    float damageTimer = 0.0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inLava = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inLava = false;
            damageTimer = 0.0f;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && inLava)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                other.GetComponent<Warrior>().health -= damageAmount;
                print("µµÆ®µô!!");
                damageTimer = 0.0f;
            }
        }
    }
}
