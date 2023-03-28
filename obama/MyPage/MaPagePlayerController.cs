using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;

public class MaPagePlayerController : MonoBehaviourPun ,IPunObservable
{
    public float speed;

    float hAxis;
    float vAxis;

  


    // 회피 : space
    bool jDown;

    // 회피 여부
    bool isDodge;

    // 벽 충돌 플래그 bool 변수를 생성
    bool isBorder;

    Vector3 moveVec;

    // 회피 도중 방향 조작 못하도록
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    public int value;
    MyPageNetworkManager MN;
 



    private void Awake()
    {
      
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    
    }

  

    void Start()
    {

      
        MN = GameObject.FindWithTag("MyPageNetworkManager").GetComponent<MyPageNetworkManager>();
        

    }

    void Update()
    {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
        Transform transform1 = transform;
        CM.Follow = transform1;
            CM.LookAt = transform;
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
        // fDown = Input.GetButton("Fire1");
    }

    void Move()
    {
        // normalized
        // 대각선이라고 속도 빨라지지 않게.
        // 방향 값이 1로 보정된 벡터
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 회피를 하고 있다면
        if (isDodge)
            moveVec = dodgeVec;

        // 벽을 뚫고 못지나가게
        if (!isBorder)
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

    }

    void Dodge()
    {// 벽을 뚫고 못지나가게
        if (jDown && moveVec != Vector3.zero && !isDodge && !isBorder)
        {
            // 움직임 벡터 -> 회피방향 벡터로 바뀌도록 구현
            dodgeVec = moveVec;
            speed *= 2.0f;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(value);  
        else value = (int)stream.ReceiveNext();
        
    }
    


}