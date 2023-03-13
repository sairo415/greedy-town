using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // 적 타입
    public enum Type{ A, B, C};
    public Type enemyType;
    // 체력과 컴포넌트를 담을 변수 선언
    public int maxHealth;
    public int curHealth;
    // 목표물
    public Transform target;
    // 콜라이더를 담을 변수 추가
    public BoxCollider meleeArea;
    // 미사일 프리팹을 담아둘 변수
    public GameObject bullet;
    // 추적을 결정하는 bool 변수 추가
    public bool isChase;
    // 지금 공격을 하고 있는지 플래그 변수
    public bool isAttack;
    

    Rigidbody rigid;
    BoxCollider boxCollider;

    Material mat;

    // Nav 관련 클래스는 UnityEngine.AI 네임스페이스 사용 -> 직접 추가
    // NavMesh : NavAgent 가 경로를 그리기 위한 바탕.
    // AI 가 목표물을 따라다니기 위한 길을 만들어야되는데 그 길을 만들 바탕.
    // Windoew - AI - Navigation

    NavMeshAgent nav;

    // 애니메이션
    Animator anim;

    void Awake()
	{
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        // MeshRender 가 하위 컴포넌트에 있으므로 변경
        // mat = GetComponent<MeshRenderer>().material;
        mat = GetComponentInChildren<MeshRenderer>().material;
        // 목표물 추적 nav 초기화
        nav = GetComponent<NavMeshAgent>();
        // 애니메이션 자식오브젝트에 있음
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    // 추적 개시
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void FreezeVelocity()
    {
        // 피격 시에 따로 움직임 적용하기 위해서
        // 추적 중일 때만 제한 걸기
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        // SphereCast() 의 반지름, 길이를 조정할 변수 선언
        float targetRadius = 0;
        float targetRange = 0;

        // switch 문으로 각 타겟팅 수치를 정하기
        switch(enemyType)
        {
        case Type.A:
            targetRadius = 1.5f;
            targetRange = 3f;
            break;
        case Type.B:
            targetRadius = 1f; // 돌격을 정확하게 해야됨으로 작게
            targetRange = 12f; // 공격 타겟팅 범위 늘리기
            break;
        case Type.C:
            // 원거리이기 때문에 다겟팅 범위 늘리고 정확해야됨
            targetRadius = 0.5f;
            targetRange = 25f;
            break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        // rayHit 변수에 데이터가 들어오면 공격 코루틴 실행
        // 이미 공격 중이면 공격하면 안됨
        if(rayHits.Length > 0 && !isAttack) 
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        // 정지를 하고 공격을 하고 추적을 개시
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch(enemyType)
        {
        case Type.A:
            // 공격 모션의 딜레이와 실제 공격됨의 싱크를 맞춤
            yield return new WaitForSeconds(0.2f);
            meleeArea.enabled = true; // 공격 범위 활성화

            // 1초 뒤 비활성화
            yield return new WaitForSeconds(1f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(1f);
            break;
        case Type.B:
            // 먼저 돌격을 해야됨. 0.1 초 선딜레이
            yield return new WaitForSeconds(0.1f);
            rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
            meleeArea.enabled = true; // 공격 범위 활성화

            // 돌격 했기 때문에 빠르게 정지
            yield return new WaitForSeconds(0.5f);
            rigid.velocity = Vector3.zero; // velocity 를 Vector3.zero 로 속도 제어
            meleeArea.enabled = false; // 공격 범위 비활성화

            // 돌격한 이후 잠시 딜레이
            yield return new WaitForSeconds(2f);


            break;
        case Type.C:
            // 발사 준비 동작 시간
            yield return new WaitForSeconds(0.5f);

            // Instantiate() 함수로 미사일 인스턴스화
            GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
            // 미사일을 만들자마자 충돌하는 것은 자기 자신이므로 태그와 레이어를 Enemy 로 지정

            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
            rigidBullet.velocity = transform.forward * 20;

            // 공격한 이후 잠시 딜레이
            yield return new WaitForSeconds(2f);

            break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void Update()
	{
        //if(isChase)
        // 도착할 목표 위치 지정 함수
        //    nav.SetDestination(target.position); // 추적 개시

        // 기존 로직은 목표만 잃어버리는거라 이동이 유지됨
        // 목표물 활성화 되어있을 때만 목표물 추적
        if(nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase; //isStopped 를 사용하여 완벽하게 멈추도록 작성
        }
            
    }
    
    // 플레이어와 물리 충돌이 나면 따라다니질 못하는 문제 해결
    void FixedUpdate()
    {
        // 가까이 있음을 인지하고 공격 시작함
        // 타겟팅을 위한 함수 생성

        Targeting();
        FreezeVelocity();
    }

    // 플레이어가 휘두르는 망치 혹은 날아오는 총알
    // 트리거로 처리
    // OnTriggerEnter() 함수에 태그 비교 조건을 작성
    void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            // 현재 위치에 피격 위치를 빼서 반작용 구하기
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));

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

            StartCoroutine(OnDamage(reactVec, false));
            //Debug.Log("Range : " + curHealth);
        }
	}

    // 피격함수 로직은 이전 시간에 구현한 것과 동일
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    // 피격 처리
    // 수류탄만의 리액션을 위해 bool 매개변수 추가
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        // 맞으면 빨간색
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12; // 12 : Layer Dead
            isChase = false;
            // 사망 리액션, 위로 날라가기 를 위해 NavAgent 비활성
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if(isGrenade)
            {
                // 수류탄에 의한 사망 리액션은 큰 힘과 회전을 추가
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                //Freeze rotation 가 걸려있으므로
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            

            Destroy(gameObject, 4); // 4초 후에 사라짐
        }
    }
}
