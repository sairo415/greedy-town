using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public ParticleSystem ps;

    List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    float damage;
    Weapon weapon;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        Transform parent = transform.parent;
        while(!parent.TryGetComponent(out Weapon wea))
        {
            parent = parent.parent;
        }
        weapon = parent.GetComponent<Weapon>();
    }


    void OnParticleCollision(GameObject other)
    {
        if(other.TryGetComponent(out Enemy enemy))
        {
            Debug.Log(transform.name+" "+weapon.damage);
            enemy.TakeDamage(weapon.damage);
        }
    }
}
