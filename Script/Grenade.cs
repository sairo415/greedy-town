using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // �� �ڽ� ������Ʈ�� ������ٵ� ���� ���� �߰�
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
	{
		yield return new WaitForSeconds(3f);

        // ������ �ӵ��� ��� Vector3.zero �� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        // ȸ���ӵ��� ���η� �ʱ�ȭ
        rigid.angularVelocity = Vector3.zero;

		meshObj.SetActive(false);
        effectObj.SetActive(true);

        // SphereCastAll : ��ü ����� ����ĳ����(��� ������Ʈ) ��� ��� ��ü�� ������
        // ����ź�� ���� ��ġ, ������, ��� ����(�������, �߽����θ� ����), ray �� ��� ����
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hitObj in rayHits)
        {
            // foreach ������ ����ź ���� ������ �ǰ��Լ��� ȣ��
            // HitByGrenade() : ����ź�� �¾���, ����ź�� ��ġ�� �Ű�������
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
