using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossPurpleDragon : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isAttack;
    public bool isLook;
    public bool isFlying = false;
    public bool isLanding = false;
    public bool isStartFlying = false;
    public bool isChase;


    public GameObject flameBreathPrefab;
    public GameObject flameSpot;
    public GameObject[] meteorSpots;
    public GameObject meteorPrefab;
    public GameObject explosionPrefab;
    public GameObject clawSpot;
    public GameObject clawPrefab;
    public GameObject[] bossDomains;
    private Transform ToGo;
    

    private enum BossState { Idle, Attack1, Attack2, Attack3, Dead, Fly, Flying };
    private BossState currentState;


    public Transform target;
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Animator anim;

    Vector3 lookVector;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        nav.isStopped = true;
        isLook = true;
        isAttack = false;
    }

    private void Start()
    {
        currentState = BossState.Idle;
    }

    private void Update()
    {
        if (!isFlying && !isLanding && !isStartFlying && !isDead)
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
                    }
                    break;
                case BossState.Attack2:
                    if (!isAttack)
                    {
                        isAttack = true;
                        Attack2();
                    }
                    break;
                case BossState.Attack3:
                    if (!isAttack)
                    {
                        isAttack = true;
                        Attack3();
                    }
                    break;
                case BossState.Fly:
                    TakeOff();
                    break;
                case BossState.Flying:
                    NowFlying();
                    break;
                case BossState.Dead:
                    if (!isDead)
                        DoDie();
                    break;
            }
        }

        else if (isFlying && !isLanding && !isStartFlying && !isDead)
        {
            isLook = false;

            nav.isStopped = false;
            nav.SetDestination(ToGo.position);

            Vector3 direction = ToGo.position - transform.position;

            float distance = direction.magnitude;

            if (distance < 10f)
            {
                StartCoroutine(Land());
            }
        }

        if (isLook && !isLanding && !isStartFlying && !isDead)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical);
            transform.LookAt(target.position + lookVector);
        }
    }


    private void ChangeState()
    {
        if (currentHealth <= 0)
        {
            currentState = BossState.Dead;
        }
        else if (isAttack)
        {
            return;
        }
        else
        {
            int ranAction = Random.Range(0, 100);

            if (ranAction < 20)
            {
                currentState = BossState.Attack1;
            }
            else if (ranAction < 40)
            {
                currentState = BossState.Attack2;
            }
            else if (ranAction < 80)
            {
                currentState = BossState.Attack3;
            }
            else if (ranAction < 100)
            {
                currentState = BossState.Fly;
            }
        }
    }

    void Attack1()
    {
        StartCoroutine(FlameBreath());
    }

    IEnumerator FlameBreath()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doFlameAttack");

        yield return new WaitForSeconds(0.5f);
        // GameObject instantFlame = Instantiate(flameBreathPrefab, flameSpot.transform.position, flameSpot.transform.rotation);
        flameBreathPrefab.SetActive(true);

        yield return new WaitForSeconds(2f);
        flameBreathPrefab.SetActive(false);
        isLook = true;

        yield return new WaitForSeconds(3f);
        isAttack = false;
        currentState = BossState.Idle;
    }

    void Attack2()
    {
        anim.SetTrigger("doScream");
        for (int i = 1; i < 7; i++)
        {
            float delay = 0.6f * i;
            StartCoroutine(Meteor(delay, i-1));
        }

        StartCoroutine(EndAttack2());
    }

    IEnumerator Meteor(float delay, int idx)
    {
        yield return new WaitForSeconds(delay);
        GameObject instantMeteor = Instantiate(meteorPrefab, meteorSpots[idx].transform.position + Vector3.up * 25f, meteorSpots[idx].transform.rotation);

        yield return new WaitForSeconds(0.5f);
        GameObject instantExplosion = Instantiate(explosionPrefab, meteorSpots[idx].transform.position, meteorSpots[idx].transform.rotation);

        yield return new WaitForSeconds(3f);
        Destroy(instantMeteor);
        Destroy(instantExplosion);
    }

    IEnumerator EndAttack2()
    {
        yield return new WaitForSeconds(6f);
        isLook = true;
        isAttack = false;
        currentState = BossState.Idle;
    }


    void Attack3()
    {
        StartCoroutine(ClawSlash());
    }


    IEnumerator ClawSlash()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doClawAttack");

        yield return new WaitForSeconds(1.5f);
        GameObject instantClaw = Instantiate(clawPrefab, clawSpot.transform.position, Quaternion.Euler(0f, 0f, 0f));


        yield return new WaitForSeconds(0.5f);
        Destroy(instantClaw);

        yield return new WaitForSeconds(2f);
        isLook = true;
        isAttack = false;
        currentState = BossState.Idle;
    }

    void TakeOff()
    {
        isStartFlying = true;
        boxCollider.enabled = false;
        anim.SetTrigger("doTakeOff");

        StartCoroutine(MakeFlying());
    }


    IEnumerator MakeFlying()
    {
        yield return new WaitForSeconds(3f);
        isStartFlying = false;
        currentState= BossState.Flying;
    }


    void NowFlying()
    {
        anim.SetBool("isFly", true);

        int ranIdx = Random.Range(0, 4);

        ToGo = bossDomains[ranIdx].transform;

        Vector3 direction = ToGo.position - transform.position;

        float distance = direction.magnitude;

        while (distance < 10f)
        {
            int newRanIdx = Random.Range(0, 4);

            ToGo = bossDomains[newRanIdx].transform;
        }

        isFlying = true;
        anim.SetBool("isFlying", true);
    }


    IEnumerator Land()
    {
        nav.isStopped = true;
        anim.SetBool("isFlying", false);
        isLook = true;
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetBool("isFlyFlameAttack", true);
        isAttack = true;

        yield return new WaitForSeconds(1f);
        flameBreathPrefab.SetActive(true);

        yield return new WaitForSeconds(2f);
        flameBreathPrefab.SetActive(false);
        anim.SetBool("isFlyFlameAttack", false);

        isAttack = false;
        isLanding = true;
        anim.SetBool("isFly", false);
        isFlying = false;
        isLook = true;
        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(1.5f);
        currentState = BossState.Idle;
        boxCollider.enabled = true;
        isLanding = false;
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
        yield return null;
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
