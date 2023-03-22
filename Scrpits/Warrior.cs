using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : MonoBehaviour
{
    // 플레이어의 스탯 관련 변수들
    public float speed;
    public int maxHealth;
    public int health;

    float horizontalAxis;
    float verticalAxis;

    // 공격 버튼 관련 변수
    bool attackDown;

    // 공속 관련 변수들
    float attackDelay;
    bool isReadyToAttack;

    // 무기를 선언해놓자
    public Sword sword;

    // 회전격 버튼
    bool spinDown;

    // 구르기 버튼
    bool dodgeDown;

    // 구르기 중에는 다른거 막자
    bool isDodge;

    // 궁극기 버튼
    bool ultiDown;

    // 궁극기 시전중
    public bool isUlti;

    // 궁쿨
    float ultiCoolTime = 10f;

    // 궁 시전후 대기 시간
    float ultiDelay;

    // 궁 시전 가능
    bool isReadyToShotUlti;

    // 구르기 방향 고정
    Vector3 dodgeVector;

    // 물리 바디 선언
    Rigidbody rigid;
    // 애니메이션 선언
    Animator anim;
    // 메인 카메라 변수
    public Camera followCamera;
    // 벽에 닿았는지 알려주는 변수
    bool isBorder;


    // 이동하는 벡터 선언
    Vector3 moveVector;

    // 시선 벡터 선언
    Vector3 lookVector;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 버튼 누르는것 인식하는 친구
        GetInput();
        // 이동 관련 로직
        Move();
        // 보는 방향 관련 로직
        Turn();
        // 공격 관련 로직
        Attack();
        // 회전격 로직
        SpinAttack();
        // 구르기 관련 로직
        Dodge();
        // 궁극기
        Ultimate();
    }

    void GetInput()
    {
        // 수직, 수평으로 이동하는 축값 따로 저장
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        // 일반 공격
        attackDown = Input.GetButtonDown("Fire1");
        // 회전격
        spinDown = Input.GetButtonDown("Fire2");
        // 구르기
        dodgeDown = Input.GetButtonDown("Jump");
        // 궁극기
        ultiDown = Input.GetButtonDown("Ultimate");
    }


    void Move()
    {

        if (!isUlti && !isBorder)
        {
            // 이동하는 벡터값을 키보드 값을 바탕으로 정해주자
            // y축으로는 이동하지 않으니 0으로 고정하고 모든 값을 평균치로 만들어주자
            moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

            transform.position += moveVector * speed * Time.deltaTime;
            anim.SetBool("isWalk", moveVector != Vector3.zero);
        }

    }

    void StopToWall()
    {
        isBorder = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        StopToWall();
    }

    void Turn()
    {
        if (!isUlti)
            transform.LookAt(transform.position + moveVector);

        if (!isUlti && !isDodge)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            // RaycastHit 정보를 받아주자
            RaycastHit rayHit;
            // out: return과 같이 반환값을 주어진 변수에 저장하는 키워드
            // 아래의 경우는 반환값이 rayHit에 저장
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                // point : rayHit이 찍히는 위치
                Vector3 nextVector = rayHit.point - transform.position;
                nextVector.y = 0;
                lookVector = transform.position + nextVector;
                transform.LookAt(lookVector);

            }
        }

    }

    void Attack()
    {
        attackDelay += Time.deltaTime;

        // 일반 공격 공속은 1초당 한번으로 ㄱ
        float attackRate = 1f;

        isReadyToAttack = attackRate < attackDelay;

        if (attackDown && isReadyToAttack && !isDodge && !isUlti)
        {
            sword.BaseAttack();
            anim.SetTrigger("doAttack");
            attackDelay = 0;
        }
    }

    void SpinAttack()
    {
        attackDelay += Time.deltaTime;

        float spinAttackRate = 1f;

        isReadyToAttack = spinAttackRate < attackDelay;

        if (spinDown && isReadyToAttack && !isDodge && !isUlti)
        {
            sword.SpinAttack();
            anim.SetTrigger("doSpinAttack");
            attackDelay = 0;
        }
    }

    void Dodge()
    {
        if (dodgeDown && !isUlti && moveVector != Vector3.zero)
        {
            isDodge = true;
            dodgeVector = lookVector;
            anim.SetTrigger("doDodge");


            // speed *= 2.0f;
            Invoke("GetDodge", 0.1f);

            Invoke("DodgeOut", 0.2f);
        }
    }

    void GetDodge()
    {
        speed *= 2.0f;
    }

    void DodgeOut()
    {
        isDodge = false;
        speed *= 0.5f;
    }

    void Ultimate()
    {
        ultiDelay += Time.deltaTime;

        isReadyToShotUlti = ultiCoolTime < ultiDelay;


        if (ultiDown && !isDodge && isReadyToShotUlti)
        {
            isUlti = true;
            anim.SetTrigger("doUlti");
            sword.Ultimate();
            ultiDelay = 0;

            Invoke("StopUlti", 2.1f);
        }
    }

    void StopUlti()
    {
        isUlti = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BossAttack" || other.tag == "BossAttackOver")
        {
            bool isBossAttack = other.name == "Explosion";
            StartCoroutine(OnDamage(isBossAttack));
        }
    }

    void OnParticleTrigger()
    {
        //if (other.tag == "BossAttack" || other.tag == "BossAttackOver")
        //{
          //  bool isBossAttack = other.name == "Explosion";
            StartCoroutine(OnDamage(false));
        //}
    }

    IEnumerator OnDamage(bool isBossAttack)
    {
        yield return new WaitForSeconds(0.1f);

        if (isBossAttack)
        {
            rigid.AddForce(transform.forward * -100, ForceMode.Impulse);
        }

        print("is Attacked!!");

        yield return new WaitForSeconds(3f);

        if (isBossAttack)
            rigid.velocity = Vector3.zero;
    }
}
