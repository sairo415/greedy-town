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

        //effect maker���� �ٿ��ֱ� �ϴµ�, ��������°� �ƴ϶� �⺻ ��ġ�� �ֵ��� �������� �ؾ��ҵ�
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