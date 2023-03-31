using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{

    public int damage;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("BossAttack");

        // Set layer to ignore collision with itself
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("BossAttack"), LayerMask.NameToLayer("BossAttack"));

        // Set layer to ignore collision with Boss layer
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("BossAttack"), LayerMask.NameToLayer("Boss"));
    }
}
