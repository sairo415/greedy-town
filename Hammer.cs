using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public float damage;
    public int per;//관통

    Rigidbody rigid;
    Rigidbody target;//player위치 판별때문에

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void Init(float damage, int per)
    {
        this.damage = damage;
        this.per = per;     
        //해머 전용 회전
        GetComponent<Transform>().Rotate(new Vector3(90, 0, 0));
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;
        rigid.velocity = dir * 15; //15는 속력
        target = GameManager.instance.player.GetComponent<Rigidbody>();

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") || per == -1)
            return;

        per--;

        if(per < 0)
        {
            rigid.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (per == -1)
            return;

        Vector3 myPos = transform.position;
        Vector3 targetPos = target.transform.position;

        float curDiff = Vector3.Distance(myPos, targetPos);

        if(curDiff > 30)//너무 멀어지면
        {
            rigid.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}
