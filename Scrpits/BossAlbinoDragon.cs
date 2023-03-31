using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.AI;

public class BossAlbinoDragon : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isAttack;
    public bool isLook;
    public bool isRun;
    public bool wasRun;

    public GameObject hornSpot;
    public GameObject hornEffectPrefab;
    public BoxCollider hornDamageArea;
    public GameObject clawEffectPrefab;
    public GameObject tornadoPrefab;
    public GameObject[] tornadoSpots;


    private enum BossState { Idle, Attack1, Attack2, Run, Dead };
    private BossState currentState;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public Transform target;
    public NavMeshAgent nav;
    public Animator anim;

    Vector3 lookVector;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        isLook = true;
        isAttack = false;
        nav.isStopped = true;
    }

    private void Start()
    {
        currentState = BossState.Idle;
    }


    private void Update()
    {
        Vector3 direction = target.position - transform.position;

        float distance = direction.magnitude;

        if (!isRun)
        {
            switch (currentState)
            {
                case BossState.Idle:
                    if (!isAttack)
                        ChangeState();
                    break;
                case BossState.Attack1:
                    if (!isAttack)
                    {
                        isAttack = true;
                        Attack1();
                        wasRun = false;
                    }
                    break;
                case BossState.Attack2:
                    if (!isAttack)
                    {
                        isAttack = true;
                        Attack2();
                        wasRun = false;
                    }
                    break;
                case BossState.Run:
                    if (!isRun)
                    {
                        isRun = true;
                        Chase();
                    }
                    else if (isRun && (distance < 10f))
                    {
                        StopCoroutine(MakeRun());
                        isRun = false;
                        currentState = BossState.Idle;
                    }
                    break;
                case BossState.Dead:
                    if (!isDead)
                    {
                        isDead = true;
                        DoDie();
                    }
                    break;
            }
        }

        if (isLook)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3 (horizontal , 0, vertical);
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
        else if (isAttack || isRun)
        {
            return;
        }
        else
        {
            int ranAction = Random.Range(0, 100);
            if (ranAction < 40)
            {
                currentState = BossState.Attack1;
            }
            else if (ranAction < 80)
            {
                currentState = BossState.Attack2;
            }
            else if (ranAction < 100 && !wasRun)
            {
                currentState = BossState.Run;
            }
        }
    }

    void Attack1()
    {
        StartCoroutine(HornAttack());
    }

    IEnumerator HornAttack()
    {
        isLook = false;
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doHornAttack");

        yield return new WaitForSeconds(1f);
        GameObject instantHornEffect = Instantiate(hornEffectPrefab, hornSpot.transform.position, hornSpot.transform.rotation);
        hornDamageArea.enabled = true;

        yield return new WaitForSeconds(1f);
        hornDamageArea.enabled = false;
        Destroy(instantHornEffect);

        yield return new WaitForSeconds(1f);
        isLook = true;
        currentState = BossState.Idle;
        isAttack = false;
    }

    void Attack2()
    {
        StartCoroutine(ClawAttack());
    }

    IEnumerator ClawAttack()
    {
        isLook = false;
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doClawAttack");

        yield return new WaitForSeconds(1.6f);
        GameObject instantEffect = Instantiate(clawEffectPrefab, transform.position + Vector3.forward * 3f, Quaternion.Euler(0f, 0f, 180f));
        GameObject instantTornado1 = Instantiate(tornadoPrefab, tornadoSpots[0].transform.position, transform.rotation);
        GameObject instantTornado2 = Instantiate(tornadoPrefab, tornadoSpots[1].transform.position, transform.rotation);
        GameObject instantTornado3 = Instantiate(tornadoPrefab, tornadoSpots[2].transform.position, transform.rotation);

        yield return new WaitForSeconds(2f);
        Destroy(instantEffect);
        isLook = true;
        currentState = BossState.Idle;
        isAttack = false;

        yield return new WaitForSeconds(2f);
        Destroy(instantTornado1);
        Destroy(instantTornado2);
        Destroy(instantTornado3);
    }

    void Chase()
    {
        wasRun = true;
        StartCoroutine(MakeRun());
    }

    IEnumerator MakeRun()
    {
        anim.SetBool("isRun", true);
        yield return new WaitForSeconds(0.1f);
        nav.isStopped = false;
        nav.SetDestination(target.transform.position);

        yield return new WaitForSeconds(2f);
        nav.isStopped = true;
        anim.SetBool("isRun", false);
        isRun = false;
        currentState = BossState.Idle;
    }

    void DoDie()
    {
        StartCoroutine(MakeDead());
    }

    IEnumerator MakeDead()
    {
        anim.SetTrigger("doDie");
        isLook = false;
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
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
        yield return null;
        print("isHit!!");
        if (!isDead)
        {
            if (currentHealth <= 0)
            {
                reactVector = reactVector.normalized;
                reactVector += Vector3.up;
                rigid.AddForce(reactVector * 10, ForceMode.Impulse);
                DoDie();
            }
        }
    }
}