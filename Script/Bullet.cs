using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

	// �������� ������ �ı����� �ʵ��� ���� �߰�
	public bool isMelee; // ���� ���� ����

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Floor")
		{
			Destroy(gameObject, 3);
		}
	}

	// �Ѿ��� ���� OnTriggerEnter() �Լ� ���� ����
	void OnTriggerEnter(Collider other)
	{
		// ����ü�� �ı�
		if(!isMelee && other.gameObject.tag == "Wall")
		{
			Destroy(gameObject);
		}
	}

}
