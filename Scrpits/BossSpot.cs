using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpot : MonoBehaviour
{
    public BoxCollider boxCollider;
    public bool isBoss;

    public bool[] isInExplodes;

    public GameObject[] fallingSpots;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Boss")
        {
            isBoss = true;
        }
    }
}
