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


    public GameObject flameBreathPrefab;
    public GameObject flameSpot;
    public GameObject[] meteorSpots;
    public GameObject meteorPrefab;
    

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
                break;
            case BossState.Fly:
                break;
            case BossState.Flying:
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

            if (ranAction < 20)
            {
                currentState = BossState.Attack2;
            }
            else if (ranAction < 40)
            {
                currentState = BossState.Attack2;
            }
            else if (ranAction < 80)
            {
                currentState = BossState.Attack2;
            }
            else if (ranAction < 100)
            {
                currentState = BossState.Attack2;
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
    }

    IEnumerator EndAttack2()
    {
        yield return new WaitForSeconds(6f);
        isLook = true;
        isAttack = false;
        currentState = BossState.Idle;
    }
}
