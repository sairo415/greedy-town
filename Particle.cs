using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public ParticleSystem ps;

    List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    float damage;
    public Weapon weapon;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        if(weapon == null)
        {
            Transform parent = transform.parent;
            while (!parent.TryGetComponent(out Weapon wea))
            {
                parent = parent.parent;
            }
            weapon = parent.GetComponent<Weapon>();
        }
        
    }


    void OnParticleCollision(GameObject other)
    {
        if(other.TryGetComponent(out Enemy enemy))
        {
            Debug.Log(transform.name+" "+weapon.damage + " enemy: " + enemy.name);
            enemy.TakeDamage(weapon.damage);
        }
    }
}
