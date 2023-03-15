using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 인스펙터 창에서 설정 가능하기 위해 public 으로
    public float speed;

    // 마우스로 방향 전환
    public Camera followCamera;

    // 값을 받을 전역변수 선언
    float hAxis;
    float vAxis;

    // space 입력을 받기 위해 (점프)
    bool jDown;

    // 키입력, 공격딜레이, 공격 준비 변수 선언
    bool fDown;

    // 점프 1회만 가능하도록 바닥에 닿았는지 판단
    bool isDodge;

    // 벽 충돌 플래그 bool 변수를 생성
    bool isBorder;

    Vector3 moveVec;

    // 회피 도중 방향 조작 못하도록
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        // 자식 오브젝트에 있는 컴포넌트를 가져옴
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Dodge();
    }

    void GetInput()
    {
        // GetAxisRaw() : Axis 값을 정수로 변환
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        // space 를 누르는 그 순간만 뛰도록 GetButtonDown 사용
        jDown = Input.GetButtonDown("Jump");
        // 기본적으로 마우스 왼쪽에 Fire1 이 들어가 있음
        fDown = Input.GetButton("Fire1");
    }

    void Move()
    {
        // normalized
        // 대각선이라고 속도 빨라지지 않게.
        // 방향 값이 1로 보정된 벡터
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 회피를 하고 있다면
        if(isDodge)
            moveVec = dodgeVec;

        // 벽을 뚫고 못지나가게
        if(!isBorder)
        {
            transform.position += moveVec * speed * Time.deltaTime;
        }

        // 애니메이션
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        // 플레이어 회전 (움직이는 방향으로 바라본다)
        transform.LookAt(transform.position + moveVec);

        if(fDown)
        {
            // 마우스에 의한 회전
            // 스크린에서 월드로 Ray 를 쏘는 함수 ScreenPointToRay();
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            // 무조건 닿도록 크게 줌 100
            // out return 처럼 반환 값을 주어진 변수에 저장하는 키워드
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                // 닿은 지점 - 플레이어의 위치 = 상대 위치
                // 그 위치로 플레이어가 바라봄
                Vector3 nextVec = rayHit.point - transform.position;
                // RayCastHit 의 높이는 무시하도록 y 축 값을 0으로
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Dodge()
    {// 벽을 뚫고 못지나가게
        if(jDown && moveVec != Vector3.zero && !isDodge && !isBorder)
        {
            // 움직임 벡터 -> 회피방향 벡터로 바뀌도록 구현
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 시간차 함수 호출 0.5 초
            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    //자동 회전 방지
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        // Raycast() : Ray 를 쏘아 닿는 오브젝트를 감지하는 함수
        isBorder = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
}