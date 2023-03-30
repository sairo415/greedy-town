using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public float damage;
    public int per;//����

    Rigidbody rigid;
    Rigidbody target;//player��ġ �Ǻ�������

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

        //�Ѿ˰� �÷��̾� �Ÿ� Ž���� -> �ʹ� �־����� �ȵǴϱ�
        target = VamsuGameManager.instance.player.GetComponent<Rigidbody>();

    }

    public void InitRotate(Vector3 vec3)
    {
        GetComponent<Transform>().Rotate(vec3);
    }

    //���Ÿ����� ����Ž��
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

    //���Ÿ����� �󸶳� ���ư����� Ž��
    void FixedUpdate()
    {
        if (per == -1)
            return;

        Vector3 myPos = transform.position;
        Vector3 targetPos = target.transform.position;

        float curDiff = Vector3.Distance(myPos, targetPos);

        if(curDiff > 30)//�ʹ� �־�����
        {
            rigid.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}
