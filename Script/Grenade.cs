using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // 각 자식 오브젝트와 리지드바디를 담을 변수 추가
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

        // 물리적 속도를 모두 Vector3.zero 로 초기화
        rigid.velocity = Vector3.zero;
        // 회전속도도 제로로 초기화
        rigid.angularVelocity = Vector3.zero;

		meshObj.SetActive(false);
        effectObj.SetActive(true);

        // SphereCastAll : 구체 모양의 레이캐스팅(모든 오브젝트) 닿는 모든 물체를 가져옴
        // 수류탄의 시작 위치, 반지름, 쏘는 방향(상관없음, 중심으로만 터짐), ray 를 쏘는 길이
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hitObj in rayHits)
        {
            // foreach 문으로 수류탄 범위 적들의 피격함수를 호출
            // HitByGrenade() : 수류탄을 맞았음, 수류탄의 위치를 매개변수로
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
