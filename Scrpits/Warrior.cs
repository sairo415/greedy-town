using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : MonoBehaviour
{
    // 플레이어의 이동속도
    public float speed;

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

    // 물리 바디 선언
    Rigidbody rigid;
    // 애니메이션 선언
    Animator anim;


    // 이동하는 벡터 선언
    Vector3 moveVector;


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
    }


    void Move()
    {
        // 이동하는 벡터값을 키보드 값을 바탕으로 정해주자
        // y축으로는 이동하지 않으니 0으로 고정하고 모든 값을 평균치로 만들어주자
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

        transform.position += moveVector * speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVector != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVector);
    }

    void Attack()
    {
        attackDelay += Time.deltaTime;

        // 일반 공격 공속은 0.7초당 한번으로 ㄱ
        float attackRate = 0.7f;

        isReadyToAttack = attackRate < attackDelay;

        if (attackDown && isReadyToAttack)
        {
            sword.Use();
            anim.SetTrigger("doAttack");
            attackDelay = 0;
        }
    }

    void SpinAttack()
    {
        attackDelay += Time.deltaTime;

        float spinAttackRate = 1f;

        isReadyToAttack = spinAttackRate < attackDelay;

        if (spinDown && isReadyToAttack)
        {
            sword.Use();
            anim.SetTrigger("doSpinAttack");
            attackDelay = 0;
        }
    }
}
