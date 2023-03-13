using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

	// 근접공격 범위가 파괴되지 않도록 조건 추가
	public bool isMelee; // 근접 공격 여부

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Floor")
		{
			Destroy(gameObject, 3);
		}
	}

	// 총알을 위해 OnTriggerEnter() 함수 로직 생성
	void OnTriggerEnter(Collider other)
	{
		// 투사체만 파괴
		if(!isMelee && other.gameObject.tag == "Wall")
		{
			Destroy(gameObject);
		}
	}

}
