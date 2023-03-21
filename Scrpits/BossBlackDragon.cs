using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBlackDragon : MonoBehaviour
{
    // ������ ����
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public bool isFlying = false;

    public GameObject flameStrikePrefab;

    // ������ ���� �ൿ
    private BossState currentState;

    // �÷��̾� Ÿ����
    public Transform target;

    public Transform ToGo;

    // �̰� ���� ���� ����
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent nav;
    public Animator anim;

    // �Ҳ� �������� ������
    public GameObject[] fallingSpots;
    public GameObject[] explosions;

    // ������ �̵� ������
    public GameObject[] bossDomains;

    public bool isAttack;
    public bool isChase = false;
    public bool isLook;

    public Transform mouth;

    Vector3 lookVector;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        nav.isStopped = true;
        isLook = true;
        isChase = false;
    }

    // ���� �ൿ ������ ���� ���µ�
   private enum BossState
    {
        Idle,
        Attack1,
        Attack2,
        Attack3,
        Dead,
        Fly,
        Flying
    }


    private void Start()
    {
        // ���� ���� �ʱ� ���� ����
        currentState = BossState.Attack1;
    }

    void Update()
    {
        if (!isFlying)
        {
            // ���� ���� ���� ���¿� ���� �ൿ ó��
            switch (currentState)
            {
                case BossState.Idle:
                    // Idle ���¿����� �ൿ ó��
                    ChangeState();
                    break;
                case BossState.Attack1:
                    // Attack1 ���¿����� �ൿ ó��
                    if (!isAttack)
                    {
                        isAttack = true;
                        Blizzard();
                    }
                    break;
                case BossState.Attack2:
                    // Attack2 ���¿����� �ൿ ó��
                    currentState = BossState.Idle;
                    break;
                case BossState.Attack3:
                    // Attack3 ���¿����� �ൿ ó��
                    currentState = BossState.Idle;
                    break;
                case BossState.Dead:
                    // Dead ���¿����� �ൿ ó��
                    break;
                case BossState.Fly:
                    TakeOff();
                    break;
                case BossState.Flying:
                    NowFlying();
                    break;
            }
        }

        else if (isFlying)
        {
            Vector3 direction = ToGo.position - transform.position;
            float distance = direction.magnitude;

            float moveDistance = Mathf.Min(distance, Time.deltaTime * 20f);

            transform.Translate(direction.normalized * moveDistance, Space.World);

            if (transform.position == ToGo.position)
            {
                anim.SetBool("isFlyMove", false);
                isFlying = false;
            }
        }


        if (isLook)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical);
            transform.LookAt(target.position + lookVector);
        }

        //IEnumerator GoNewPlace()
        //{
        //    yield return new WaitForSeconds(0.1f);
        //    int ranIdx = Random.Range(0, 6);

        //    ToGo = bossDomains[ranIdx].transform;

        //    Vector3 direction = ToGo.position - transform.position;
        //    float distance = direction.magnitude;

        //    float moveDistane = Mathf.Min(distance, Time.deltaTime * 20f);

        //    anim.SetBool("isFlyMove", true);
        //    isFlying = true;
        //    transform.Translate(direction.normalized * moveDistane, Space.World);

        //    yield return new WaitForSeconds(3f);
        //    anim.SetBool("isFlyMove", false);
        //    isFlying = false;
        //}
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

    // ���� ������ ���¸� �����ϴ� �Լ�
    void ChangeState()
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
            switch (currentState)
            {
                case BossState.Idle:
                    int ranAction = Random.Range(0, 100);

                    if (ranAction < 30)
                    {
                        currentState = BossState.Attack1;
                    }
                    else if (ranAction < 60)
                    {
                        currentState = BossState.Attack2;
                    }
                    else if (ranAction < 80) {
                        currentState = BossState.Attack3;
                    }
                    else if (ranAction < 100)
                    {
                        currentState = BossState.Fly;
                    }
                    break;
            }
        }
    }


    // Attack1
    void Blizzard()
    {
        List<int> ranNums = new List<int>();

        while (ranNums.Count < 5)
        {
            int num = Random.Range(0, 9);

            if (!ranNums.Contains(num))
            {
                ranNums.Add(num);
            }
        }

        foreach (int num in ranNums)
        {
            StartCoroutine(Strike(num));
        }

        StartCoroutine(EndAttack());
    }

    // Attack1 ���߱�
    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(4f);
        isAttack = false;
        currentState = BossState.Idle;
    }

    // Attack1���� ���� ��ü ��ȯ���ֱ�
    IEnumerator Strike(int idx)
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doAttack1");
        fallingSpots[idx].SetActive(true);

        yield return new WaitForSeconds(1f);
        fallingSpots[idx].SetActive(false);
        GameObject flameStrike = Instantiate(flameStrikePrefab, fallingSpots[idx].transform.position, fallingSpots[idx].transform.rotation);
        flameStrike.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        explosions[idx].SetActive(true);

        yield return new WaitForSeconds(1.5f);

        explosions[idx].SetActive(false);
        Destroy(flameStrike);
        isLook = true;
    }


    void TakeOff()
    {
        anim.SetTrigger("doFly");

        StartCoroutine(MakeFlying());
    }

    IEnumerator MakeFlying()
    {
        yield return new WaitForSeconds(3f);
        currentState = BossState.Flying;
    }

    void NowFlying()
    {
        anim.SetBool("isFly", true);

        int ranIdx = Random.Range(0, 6);

        ToGo = bossDomains[ranIdx].transform;

        isFlying = true;
        anim.SetBool("isFlyMove", true);

    }

    void OnCollisionEnter(Collision collision)
    {
        // ���� �浹�� �༮�� ���̾� Ȯ��
        int layer = collision.gameObject.layer;


        if (layer == LayerMask.NameToLayer("BossAttack"))
        {
            return;
        }
    }
}
