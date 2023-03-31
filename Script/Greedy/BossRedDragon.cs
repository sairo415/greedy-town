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
    public bool isStartRunning;
    public bool isLanding;

    public GameObject scatterSpot;
    public GameObject flameScatterPrefab;
    public GameObject flameTsunamiPrefab;
    public GameObject[] tsunamiSpots1;
    public GameObject[] tsunamiSpots2;
    public GameObject[] bossSpots;
    public GameObject coreSpot;
    public GameObject corePrefab;
    public GameObject flameWallSpot;
    public GameObject flameWallPrefab;
    public GameObject[] meteorSpots;
    public GameObject meteorPrefab;
    private int bossSpot;

    private Transform ToGo;

    private enum BossState { Idle, Attack1, Attack2, Attack3, Attack4, Run, Dead };
    private BossState currentState;


    public Rigidbody rigid;
    public Transform target;
    public BoxCollider boxCollider;
    public BoxCollider[] meteorColliders;
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
        bossSpot = 0;
    }

    private void Start()
    {
        currentState = BossState.Idle;
    }

    private void Update()
    {
        if (!isRun && !isDead && !isStartRunning && !isLanding)
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
                    RunDragon();
                    break;
                case BossState.Dead:
                    DoDie();
                    break;
            }
        }

        else if (isRun && !isDead && !isAttack && !isStartRunning)
        {
            isLook = false;

            Vector3 direction = ToGo.position - transform.position;

            transform.LookAt(ToGo.position + direction);
            float distance = direction.magnitude;

            float moveDistance = Mathf.Min(distance, Time.deltaTime * 40f);

            transform.Translate(direction.normalized * moveDistance, Space.World);

            if (distance < 5f)
            {
                isLanding = true;
                anim.SetBool("isRun", false);
                StartCoroutine(StopRunning());
            }
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
        {
            StopAllCoroutines();
        }
    }


    void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    private void ChangeState()
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

            if (ranAction < 20)
            {
                currentState = BossState.Attack1;
            }
            else if (ranAction < 50)
            {
                currentState = BossState.Attack2;
            }
            else if (ranAction < 80)
            {
                currentState = BossState.Attack3;
            }
            else if (ranAction < 100)
            {
                currentState = BossState.Run;
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
        for (int i = 1; i < 4; i++)
        {
            if (bossSpot == 0)
                StartCoroutine(ShotFlameTsunami1(i * 1f, i - 1));
            else
                StartCoroutine(ShotFlameTsunami2(i * 1f, i - 1));
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
        yield return new WaitForSeconds(5f);
        currentState = BossState.Idle;
        isAttack = false;
    }


    void Attack3()
    {
        StartCoroutine(FlameWall());
        for (int idx = 1; idx < 7; idx++)
        {
            StartCoroutine(MakeMeteors(idx * 0.5f, idx - 1));
        }
        StartCoroutine(EndAttack3());
    }


    IEnumerator FlameWall()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doFlameAttack");

        yield return new WaitForSeconds(0.3f);
        GameObject instantFlameWall = Instantiate(flameWallPrefab, flameWallSpot.transform.position, flameWallSpot.transform.rotation);

        yield return new WaitForSeconds(5f);
        Destroy (instantFlameWall);
    }

    IEnumerator MakeMeteors(float delay, int idx)
    {
        yield return new WaitForSeconds(delay);
        GameObject instantMeteor = Instantiate(meteorPrefab, meteorSpots[idx].transform.position, Quaternion.Euler(0f, 0f, 180f));

        yield return new WaitForSeconds(1f);
        meteorColliders[idx].enabled = true;

        yield return new WaitForSeconds(2f);
        meteorColliders[idx].enabled = false;
        Destroy(instantMeteor);
    }

    IEnumerator EndAttack3()
    {
        yield return new WaitForSeconds(6.5f);
        currentState = BossState.Idle;
        isAttack = false;
    }


    void RunDragon()
    {
        isStartRunning = true;
        if (bossSpot == 0)
        {
            bossSpot = 1;
            ToGo = bossSpots[1].transform;
            StartCoroutine(MoveTo());
        }
        else if (bossSpot == 1)
        {
            bossSpot = 0;
            ToGo = bossSpots[0].transform;
            StartCoroutine(MoveTo());
        }
    }

    IEnumerator MoveTo()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("doTakeOff");

        yield return new WaitForSeconds(0.3f);
        isStartRunning = false;
        isRun = true;
        GameObject instantCore = Instantiate(corePrefab, coreSpot.transform.position, coreSpot.transform.rotation);
        anim.SetBool("isRun", true);

        yield return new WaitForSeconds(1f);
        Destroy(instantCore);
    }

    IEnumerator StopRunning()
    {
        yield return new WaitForSeconds(0.1f);
        isRun = false;
        isLook = true;
        GameObject instantCore = Instantiate(corePrefab, coreSpot.transform.position, coreSpot.transform.rotation);

        yield return new WaitForSeconds(1f);
        Destroy(instantCore);
        isLanding = false;
        currentState = BossState.Idle;
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
}
