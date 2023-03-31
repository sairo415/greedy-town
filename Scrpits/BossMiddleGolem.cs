using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMiddleGolem : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isAttack;
    public bool isChase;
    public bool isLook;
    public bool isDead;

    public GameObject rockPrefab;
    public Rigidbody rigidRock;
    public GameObject rockSpot;

    private enum BossState { Idle, Attack1, Attack2, Chase, Dead };

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public Animator anim;
    public NavMeshAgent nav;
    private BossState currentState;

    Vector3 lookVector;

    public Transform target;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        nav.isStopped = true;
        isLook = true;
    }

    private void Start()
    {
        currentState = BossState.Idle;
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                if (!isAttack && !isChase)
                    ChangeState();
                break;
            case BossState.Attack1:
                if (!isAttack)
                {
                    isAttack = true;
                    Attack1();
                }
                break;
            case BossState.Attack2:
                if (!isAttack)
                {
                    isAttack = true;
                    Attack2();
                }
                break;
            case BossState.Chase:
                break;
            case BossState.Dead:
                break;
        }

        if (isLook)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical);
            transform.LookAt(target.position + lookVector);
        }
    }


    private void FixedUpdate()
    {
        FreezeVelocity();
        if (isDead)
            StopAllCoroutines();
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
            currentState = BossState.Dead;
        }
        else if (isAttack || isChase)
        {
            return;
        }
        else
        {
            switch (currentState)
            {
                case BossState.Idle:
                    int ranAction = Random.Range(0, 11);
                    
                    if (ranAction < 10)
                    {
                        currentState = BossState.Attack2;
                    }
                    break;
            }
        }
    }

    void Attack1()
    {
        StartCoroutine(ThrowRock());
    }

    IEnumerator ThrowRock()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doAttack1");

        yield return new WaitForSeconds(0.5f);
        GameObject instantRock = Instantiate(rockPrefab, rockSpot.transform.position, rockSpot.transform.rotation);

        while (Vector3.Distance(transform.position, instantRock.transform.position) > 0.1f)
        {
            instantRock.transform.position = Vector3.Lerp(instantRock.transform.position, transform.position, Time.deltaTime * 3f);
            yield return null;
        }

        yield return new WaitForSeconds(5f);
        Vector3 throwForce = (instantRock.transform.position - target.position).normalized;
        rigidRock.AddForce(throwForce * 5f, ForceMode.Impulse);
        currentState = BossState.Idle;
        isAttack = false;
    }

    void Attack2()
    {
        StartCoroutine(RockPunch());
    }

    IEnumerator RockPunch()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doAttack2");

        yield return new WaitForSeconds(0.5f);
        GameObject instantRock = Instantiate(rockPrefab, rockSpot.transform.position, Quaternion.Euler(0f, 90f, 0f));

        yield return new WaitForSeconds(1f);
        Vector3 shotDirection = target.position - instantRock.transform.position;
        Rigidbody rigidRock = instantRock.GetComponent<Rigidbody>();
        rigidRock.AddForce(shotDirection * 0.5f, ForceMode.Impulse);
        // rigidRock.velocity = shotDirection * 10f;


        yield return new WaitForSeconds(3f);
        Destroy(instantRock);
        currentState = BossState.Idle;
        isAttack = false;
    }
}
