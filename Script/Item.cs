using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Ammo,
        Coin,
        Grenade,
        Heart,
        Weapon
    };

    public Type type;
    public int value;

    // ���� �浹�� ����ϴ� �ݶ��̴��� �浹�Ͽ� ���� �߻�
    Rigidbody rigid;
    SphereCollider sphereCollider;

	void Awake()
	{
        rigid = GetComponent<Rigidbody>();
        // ���⼭ Collider �� �ΰ��ε� �̷��� �Ұ�� GetComponent �� ù��° ������Ʈ�� �����´�.
        sphereCollider = GetComponent<SphereCollider>();
	}

	void Update()
	{
        transform.Rotate(Vector3.up * 40 * Time.deltaTime);
	}

	void OnCollisionEnter(Collision collision)
	{
        // �������� �����ǰ� �ٴڿ� ����� ��
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
	}
}
