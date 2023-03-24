using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossIceLich : MonoBehaviour
{
    // 보스 행동 패턴 상태
    private enum State { Idle, Attack1, Attack2, Die };

    // 보스 체력들
    public int currentHealth;
    public int maxHealth;

    // 추격 대상
    public Transform target;

    // 상태 관련
    private State currentState;
    public CapsuleCollider capsuleCollider;
    public Rigidbody rigid;
    public Animator anim;
    public NavMeshAgent nav;

    // bool값들
    bool isAttack;

    // 위치
    public GameObject[] tornados;
    public GameObject lazerPoint;

    // 공격
    public GameObject tornadoPrefabs;
    public GameObject lazerBeamPrefabs;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        nav.isStopped = true;
    }

    void Start()
    {
        currentState = State.Attack1;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                ChangeState();
                break;
            case State.Attack1:
                if (!isAttack)
                {
                    isAttack = true;
                    Attack1();
                }
                break;
            case State.Attack2:
                if (!isAttack)
                {
                    isAttack = true;
                    Attack2();
                }
                break;
            case State.Die:
                break;
        }
        print(currentState);
    }

    void FixedUpdate()
    {
        FreezeVelocity();
    }

    void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }


    void ChangeState()
    {
        if (currentHealth <= 0)
        {
            currentState = State.Die;
        }
        else if (isAttack)
        {
            return;
        }
        else
        {
            switch (currentState)
            {
                case State.Idle:
                    int ranAction = Random.Range(0, 10);

                    if (ranAction < 4)
                    {
                        currentState = State.Attack1;
                    }
                    else
                    {
                        currentState = State.Attack2;
                    }
                    break;
            }
        }
    }

    void Attack1()
    {
        anim.SetTrigger("doAttack1");
        isAttack = true;
        StartCoroutine(ShotLazerBeam());
    }


    IEnumerator ShotLazerBeam()
    {
        yield return new WaitForSeconds(2.5f);
        GameObject lazer = Instantiate(lazerBeamPrefabs, lazerPoint.transform.position, lazerPoint.transform.rotation);

        yield return new WaitForSeconds(2.7f);
        Destroy(lazer);

        yield return new WaitForSeconds(3f);

        currentState = State.Idle;
        isAttack = false;
    }

    void Attack2()
    {
        anim.SetTrigger("doAttack2");
        isAttack = true;
        for (int num = 0; num < 4; num++)
        {
            StartCoroutine(MakeTornado(num));
        }
    }

    IEnumerator MakeTornado(int idx)
    {
        yield return new WaitForSeconds(1f);
        GameObject tornado = Instantiate(tornadoPrefabs, tornados[idx].transform.position, tornados[idx].transform.rotation);
        int counts = 0;
        while (counts < 300)
        {
            tornado.transform.position += tornado.transform.forward * 50f * Time.deltaTime;
            counts += 1;
            yield return null;
        }

        yield return new WaitForSeconds(3f);
        Destroy(tornado);

        currentState = State.Idle;
        isAttack = false;
    }
}
