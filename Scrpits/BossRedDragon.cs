using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossRedDragon : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isAttack;
    public bool isLook;
    public bool isRun;

    public GameObject scatterSpot;
    public GameObject flameScatterPrefab;
    public GameObject flameTsunamiPrefab;
    public GameObject[] tsunamiSpots1;
    public GameObject[] tsunamiSpots2;
    public GameObject[] bossSpots;
    public int bossSpot;
    public GameObject breathSpot;
    public GameObject breathPrefab;

    private enum BossState { Idle, Attack1, Attack2, Attack3, Attack4, Run, Dead };
    private BossState currentState;


    public Rigidbody rigid;
    public Transform target;
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
            case BossState.Attack4:
                break;
            case BossState.Run:
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

            if (ranAction < 100)
            {
                currentState = BossState.Attack3;
            }
        }
    }

    void Attack1()
    {
        StartCoroutine(ClawSmash());
    }

    IEnumerator ClawSmash()
    {
        isLook = false;
        anim.SetTrigger("doClawAttack");
        yield return new WaitForSeconds(0.5f);
        GameObject instantScatter = Instantiate(flameScatterPrefab, scatterSpot.transform.position, scatterSpot.transform.rotation);

        yield return new WaitForSeconds(3.5f);
        isAttack = false;
        currentState = BossState.Idle;
        isLook = true;
        Destroy(instantScatter);
    }

    void Attack2()
    {
        anim.SetTrigger("doScream");
        for (int i = 0; i < 3; i++)
        {
            if (bossSpot == 0)
                StartCoroutine(ShotFlameTsunami1(i * 1.35f, i));
            else
                StartCoroutine(ShotFlameTsunami2(i * 1.35f, i));
        }

        StartCoroutine(EndAttack2());
    }

    IEnumerator ShotFlameTsunami1(float delay, int idx)
    {
        yield return new WaitForSeconds(delay);

        yield return new WaitForSeconds(0.5f);
        GameObject instantTsunami = Instantiate(flameTsunamiPrefab, tsunamiSpots1[idx].transform.position, Quaternion.Euler(0f, 45f, 0f));

        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(2f);
        Destroy(instantTsunami);
    } 


    IEnumerator ShotFlameTsunami2(float delay, int idx)
    {
        yield return new WaitForSeconds(delay);

        yield return new WaitForSeconds(0.5f);
        GameObject instantTsunami = Instantiate(flameTsunamiPrefab, tsunamiSpots2[idx].transform.position, Quaternion.Euler(0f, -45f, 0f));

        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(2f);
        Destroy(instantTsunami);
    }


    IEnumerator EndAttack2()
    {
        yield return new WaitForSeconds(7f);
        currentState = BossState.Idle;
        isAttack = false;
    }


    void Attack3()
    {
        StartCoroutine(FlameBreath());
    }


    IEnumerator FlameBreath()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doFlameAttack");
    }
}
