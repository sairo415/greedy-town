using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossParticle : MonoBehaviour
{
    public ParticleSystem ps;

    // List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    public int damage;
    // public Weapon weapon;

    void Start()
    {
       ps = GetComponent<ParticleSystem>();

        //effect maker에서 붙여주긴 하는데, 만들어지는게 아니라 기본 배치인 애들은 가져오긴 해야할듯
        // if (weapon == null)
       // {
         //   Transform parent = transform.parent;
           // while (!parent.TryGetComponent(out Weapon wea))
           // {
             //   parent = parent.parent;
           // }
           // weapon = parent.GetComponent<Weapon>();
       // }

    }


    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.layer ==  LayerMask.NameToLayer("Player"))
       {
            Warrior warrior = new Warrior();
            warrior.TakeDamage(damage);
       }
    }
}