using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossIceLich : MonoBehaviour
{
    // 보스 행동 패턴 상태
    private enum State { Idle, Attack1, Attack2, Die, Teleport };

    // 보스 체력들
    public int currentHealth;
    public int maxHealth;
    public bool isDead = false;
    public bool isLook;
    public bool isTeleport;

    public GameObject moveEffectSpot;
    public GameObject moveEffectPrefab;


    Vector3 lookVector;

    // 추격 대상
    public Transform target;

    public GameObject[] lichSpots;

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
        isLook = true;
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
                if (!isDead)
                    ChangeState();
                break;
            case State.Attack1:
                if (!isAttack && !isTeleport && !isDead)
                {
                    isAttack = true;
                    Attack1();
                }
                break;
            case State.Attack2:
                if (!isAttack && !isTeleport && !isDead)
                {
                    isAttack = true;
                    Attack2();
                }
                break;
            case State.Die:
                if (!isDead)
                    DoDie();
                break;
            case State.Teleport:
                if (!isAttack && !isTeleport && !isDead)
                {
                    isTeleport = true;
                    DoTeleport();
                }
                break;
        }

        if (isLook && !isDead)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical);
            transform.LookAt(target.position + lookVector);
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity();
        if (isDead)
        {
            StopAllCoroutines();
        }
    }

    void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }


    void ChangeState()
    {
        if (isAttack || isTeleport)
        {
            return;
        }
        else
        {
            switch (currentState)
            {
                case State.Idle:
                    int ranAction = Random.Range(0, 11);

                    if (ranAction < 4)
                    {
                        currentState = State.Attack1;
                    }
                    else if (ranAction < 8)
                    {
                        currentState = State.Attack2;
                    }
                    else
                    {
                        currentState = State.Teleport;
                    }
                    break;
            }
        }
    }

    void Attack1()
    {
        isLook = false;
        anim.SetTrigger("doAttack1");
        isAttack = true;
        StartCoroutine(ShotLazerBeam());
    }


    IEnumerator ShotLazerBeam()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject lazer = Instantiate(lazerBeamPrefabs, lazerPoint.transform.position, lazerPoint.transform.rotation);

        yield return new WaitForSeconds(1.3f);
        Destroy(lazer);

        yield return new WaitForSeconds(0.5f);
        isLook = true;

        yield return new WaitForSeconds(1.0f);

        currentState = State.Idle;
        isAttack = false;
    }

    void Attack2()
    {
        isLook = false;
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

        isLook = true;

        yield return new WaitForSeconds(1.5f);
        Destroy(tornado);

        currentState = State.Idle;
        isAttack = false;
    }

    void DoTeleport()
    {
        isLook = false;
        anim.SetTrigger("doMove");
        StartCoroutine(ChangePosition());
    }

    IEnumerator ChangePosition()
    {
        capsuleCollider.enabled = false;
        GameObject moveEffect = Instantiate(moveEffectPrefab, moveEffectSpot.transform.position, moveEffectSpot.transform.rotation);
        yield return new WaitForSeconds(2f);

        int ranPosition = Random.Range(0, 4);

        Destroy(moveEffect);
        transform.position = lichSpots[ranPosition].transform.position;
        isLook = true;
        capsuleCollider.enabled = true;

        yield return new WaitForSeconds(1f);
        isTeleport = false;
        currentState = State.Idle;
    }

    void DoDie()
    {
        StartCoroutine(MakeDead());
    }

    IEnumerator MakeDead()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        isLook = false;
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            Sword sword = other.GetComponent<Sword>();
            currentHealth -= sword.damage;
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnHit(reactVector));
        }
    }

    IEnumerator OnHit(Vector3 reactVector)
    {
        yield return new WaitForSeconds(0.1f);
        print("isHit!");
        if (!isDead)
        {
            if (currentHealth <= 0)
            {
                reactVector = reactVector.normalized;
                reactVector += Vector3.up;
                rigid.AddForce(reactVector * 3, ForceMode.Impulse);
                DoDie();
            }

        }
    }
}
