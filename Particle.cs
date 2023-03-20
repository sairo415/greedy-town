using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public ParticleSystem ps;

    List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    float damage;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        damage = transform.root.GetComponent<Weapon>().damage;
    }

    void OnParticleCollision(GameObject other)
    {
        if(other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
        }
    }
}
