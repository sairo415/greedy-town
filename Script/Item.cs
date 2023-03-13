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

    // 물리 충돌을 담당하는 콜라이더와 충돌하여 문제 발생
    Rigidbody rigid;
    SphereCollider sphereCollider;

	void Awake()
	{
        rigid = GetComponent<Rigidbody>();
        // 여기서 Collider 는 두개인데 이렇게 할경우 GetComponent 는 첫번째 컴포넌트를 가져온다.
        sphereCollider = GetComponent<SphereCollider>();
	}

	void Update()
	{
        transform.Rotate(Vector3.up * 40 * Time.deltaTime);
	}

	void OnCollisionEnter(Collision collision)
	{
        // 아이템이 생성되고 바닥에 닿았을 때
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
	}
}
