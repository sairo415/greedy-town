using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlackDragon : MonoBehaviour
{
    // 상태창
    public int maxHealth;
    public int curHealth;
    public bool isDead;
    public bool isFly = false;

    // 플레이어 잡기
    public Transform target;
    public bool isDetected;

    // 기본 공격 범위
    public BoxCollider baseAttackArea;

    // 불덩이 내뱉기
    public GameObject fireBall;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Animator anim;

    // 플레이어를 바라보는 플래그 변수
    public bool isLook;

    // 플레이어 이동방향 위치를 조금 미리 확인하는 벡터
    Vector3 lookVector;

    // 불 쏠 위치(입)
    public Transform mouth;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        // 우선 움직이지 못하게 막아주기
        nav.isStopped = true;
        // 보기 시작
        isLook = true;
        // 패턴
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (!nav.isStopped)
        {
            print(target.position);
            nav.SetDestination(target.position);
        }

        if (isLook)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical) * 2f;
            transform.LookAt(target.position + lookVector);
        }
    }

    void FixedUpdate()
    {
        Targeting();
    }

    void Targeting()
    {
        isLook = false;
        print("Chasing");
        float targetRadius = 3f;
        float targetRange = 3.5f;

        RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,
                                        targetRadius,
                                        transform.forward,
                                        targetRange,
                                        LayerMask.GetMask("Player"));

        if (rayHits.Length > 0)
        {
            StartCoroutine(DoAttack());
        }
    }


    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.3f);

        // 랜덤 패턴 확률 정해주기
        int ranAction = Random.Range(0, 10);

        switch (ranAction)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                StartCoroutine(BaseAttack());
                print("기본 공격중");
                break;
            case 5:
            case 6:
            case 7:
                StartCoroutine(FireShot());
                break;
            case 8:
            case 9:
                StartCoroutine(TakeOff());
                break;
        }

    }

    //IEnumerator Flying()
    //{
    //    yield return new WaitForSeconds(0.1f);

    //    int ranAction = Random.Range(0, 6);
    //}

    IEnumerator BaseAttack()
    {
        yield return new WaitForSeconds(0.1f);
        nav.isStopped = false;
        anim.SetBool("isRun", true);
    }



    IEnumerator DoAttack()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doBaseAttack");

        yield return new WaitForSeconds(0.5f);
        baseAttackArea.enabled = true;

        yield return new WaitForSeconds(1f);
        baseAttackArea.enabled = false;

        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isRun", false);
        isLook = true;
        nav.isStopped = true;

        // StartCoroutine(Think());
    }

    IEnumerator FireShot()
    {
        isLook = false;
        anim.SetTrigger("doFire");
        yield return new WaitForSeconds(0.1f);
        Instantiate(fireBall, mouth.position, mouth.rotation);

        yield return new WaitForSeconds(2f);
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator TakeOff()
    {
        boxCollider.enabled = false;

        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doFly");

        transform.position += Vector3.up * 2f;

        yield return new WaitForSeconds(2.5f);
        anim.SetBool("isFly", true);

    }
}
