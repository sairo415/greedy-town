using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Camera
    public Camera followCamera;
    
    float hAxis;
    float vAxis;

    // 키입력
    bool sDown; // 회피 : space
    bool qDown; // 기본 공격
    bool wDown; // 스킬 1
    bool eDown; // 스킬 2
    bool rDown; // 스킬 3

    // 속도
    public float speed;

    // 스킬
    public Transform swordForcePos;
    public GameObject swordForce;

    public Transform swordDancePos;
    public GameObject swordDance;

    public Transform eternalSlashDancePos;
    public GameObject eternalSlashDance;

    // 기본 공격 딜레이
    float qSkillDelay;  // q 사용 후 경과한 시간
    public float qSkillRate;   // q 재사용 대기 시간
    bool isQSkillDelay; // q 스킬 사용 가능 여부

    // w 공격 딜레이
    float wSkillDelay;
    public float wSkillRate;
    bool isWSkillReady;
    //public int wSkillDamage;

    // e 공격 딜레이
    float eSkillDelay;
    public float eSkillRate;
    bool isESkillReady;
    //public int eSkillDamage;

    // r 공격 딜레이
    float rSkillDelay;
    public float rSkillRate;
    bool isRSkillReady;

    // 회피 여부
    bool isDodge;

    // 벽 충돌 여부
    bool isBorder;

    Vector3 moveVec;    // 움직일 벡터
    Vector3 dodgeVec;   // 회피 도중 방향 조작 못하도록

    Rigidbody rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {
        //입력
        GetInput();
        
        //움직임
        Move();
        Turn();
        
        //회피
        Dodge();
        
        //공격
        Attack();
        WSkill();
        ESkill();
        RSkill();
    }

    void GetInput()
    {
        // GetAxisRaw() : Axis 값을 정수로 변환
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        // space 를 누르는 그 순간만 회피하도록 GetButtonDown 사용
        sDown = Input.GetButtonDown("Jump");
        // 기본 공격 및 스킬 입력
        qDown = Input.GetButtonDown("Skill1");
        wDown = Input.GetButtonDown("Skill2");
        eDown = Input.GetButtonDown("Skill3");
        rDown = Input.GetButtonDown("Skill4");
    }

    void Move()
    {
        // normalized
        // 대각선이라고 속도 빨라지지 않게 방향 값이 1로 보정된 벡터로
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 회피를 하고 있다면 회피하는 방향으로 치환
        if(isDodge)
            moveVec = dodgeVec;

        // 벽을 뚫고 못지나가게
        if(!isBorder)
            transform.position += moveVec * speed * Time.deltaTime;

        // 애니메이션
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        // 플레이어 회전 (움직이는 방향으로 바라본다)
        transform.LookAt(transform.position + moveVec);
    }

    void Dodge()
    {
        // 벽을 뚫고 못지나가게
        if(sDown && moveVec != Vector3.zero && !isDodge && !isBorder)
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

    void Attack()
    {

    }

    void WSkill()
    {
        wSkillDelay += Time.deltaTime;
        isWSkillReady = wSkillRate < wSkillDelay;

        if(wDown && isWSkillReady && !isDodge)
        {
            // 스킬 시전
            StartCoroutine("WSkillStart");

            // 애니메이션
            anim.SetTrigger("doSwing1");

            // 딜레이 초기화
            wSkillDelay = 0;
        }
    }

    void ESkill()
    {
        eSkillDelay += Time.deltaTime;
        isESkillReady = eSkillRate < eSkillDelay;

        if(eDown && isESkillReady && !isDodge)
        {
            // 스킬 시전
            StartCoroutine("ESkillStart");

            // 애니메이션
            anim.SetTrigger("doSwing2");

            // 딜레이 초기화
            eSkillDelay = 0;
        }
    }

    void RSkill()
    {
        rSkillDelay += Time.deltaTime;
        isRSkillReady = rSkillRate < rSkillDelay;

        if(rDown && isRSkillReady && !isDodge)
        {
            // 스킬 시전
            StartCoroutine("RSkillStart");

            // 애니메이션
            anim.SetTrigger("doSwing3");

            // 딜레이 초기화
            rSkillDelay = 0;
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    IEnumerator WSkillStart()
    {
        GameObject skillAreaObj = Instantiate(swordForce, swordForcePos.position, swordForcePos.rotation);
        skillAreaObj.SetActive(true);

        Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
        skillAreaRigid.velocity = swordForcePos.forward * 50;

        yield return null;
    }

    IEnumerator ESkillStart()
    {
        GameObject skillAreaObj = Instantiate(swordDance, swordDancePos.position, swordDancePos.rotation);
        skillAreaObj.SetActive(true);

        Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
        skillAreaRigid.velocity = swordForcePos.forward;

        yield return null;
    }

    IEnumerator RSkillStart()
    {
        GameObject skillAreaObj = Instantiate(eternalSlashDance, eternalSlashDancePos.position, eternalSlashDancePos.rotation);
        skillAreaObj.SetActive(true);

        Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
        skillAreaRigid.velocity = swordForcePos.forward;

        yield return null;
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