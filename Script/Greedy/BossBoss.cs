using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBoss : MonoBehaviour
{
    // 체력과 컴포넌트를 담을 변수 선언
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;

    Material mat;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

    // 플레이어가 휘두르는 망치 혹은 날아오는 총알
    // 트리거로 처리
    // OnTriggerEnter() 함수에 태그 비교 조건을 작성
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
        {
            Debug.Log("Monster AAAYA");

            //Player player = other.GetComponent<Player>();
            //Debug.Log(player.wSkillDamage);
            //curHealth -= player.wSkillDamage;
            curHealth -= 20;

            // 적과 닿았을 때 삭제되도록 Destroy() 호출
            if(other.tag == "PlayerAttack")
            {
                Destroy(other.gameObject);
                other.gameObject.SetActive(false);
            }

            //StartCoroutine(OnDamage(reactVec, false));
        }

        /*if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            // 현재 위치에 피격 위치를 빼서 반작용 구하기
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));

            //Debug.Log("Melee : " + curHealth);
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            // 현재 위치에 피격 위치를 빼서 반작용 구하기
            Vector3 reactVec = transform.position - other.transform.position;

            // 총알의 경우, 적과 닿았을 때 삭제되도록 Destroy() 호출
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
            //Debug.Log("Range : " + curHealth);
        }*/
    }
}
