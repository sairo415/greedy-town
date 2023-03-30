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
    }


    public void Init(float damage, int per, Vector3 dir, float speed)
    {
        this.damage = damage;
        this.per = per;
        rigid.velocity = dir * speed;

        //총알과 플레이어 거리 탐색용 -> 너무 멀어지면 안되니까
        target = VamsuGameManager.instance.player.GetComponent<Rigidbody>();

    }

    public void InitRotate(Vector3 vec3)
    {
        GetComponent<Transform>().Rotate(vec3);
    }

    //원거리무기 관통탐색
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

    //원거리무기 얼마나 날아갔는지 탐색
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
