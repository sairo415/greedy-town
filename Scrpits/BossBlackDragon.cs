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
    public bool isLanding = false;
    public bool isStartFlying = false;

    // Attack1
    public GameObject flameStrikePrefab;
    public GameObject iceFieldPrefab;
    public GameObject TailAttackPrefab;

    // Attack2
    public GameObject iceBallPrefab;

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

    // �ʹ� ���ƴٴϸ� �ȵǴϱ�
    bool wasFlied;

    public bool isAttack;
    public bool isChase = false;
    public bool isLook;

    public Transform mouth;
    public Transform TailRegion;

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
        if (!isFlying && !isLanding && !isStartFlying)
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
                    if (!isAttack)
                    {
                        isAttack = true;
                        ShotIceBall();
                    }
                    break;
                case BossState.Attack3:
                    // Attack3 ���¿����� �ൿ ó��
                    if (!isAttack)
                    {
                        isAttack = true;
                        TailAttack();
                    }
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


        // ���鼭 �̵���
        else if (isFlying && !isLanding && !isStartFlying)
        {
            isLook = false;

            Vector3 direction = ToGo.position - transform.position;

            transform.LookAt(ToGo.position + direction);
            float distance = direction.magnitude;

            float moveDistance = Mathf.Min(distance, Time.deltaTime * 20f);

            transform.Translate(direction.normalized * moveDistance, Space.World);

            if (distance < 1f)
            {
                // update ���� ����
                isLanding = true;
                anim.SetBool("isFlyMove", false);
                anim.SetBool("isFly", false);
                StartCoroutine(Land());
            }
        }

        else if ((target.position - transform.position).magnitude <= 10f)
        {
            currentState = BossState.Attack3;
        }


        // ���� ���� ĳ���� ����
        if (isLook && !isLanding && !isStartFlying)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(horizontal, 0, vertical);
            transform.LookAt(target.position + lookVector);
        }
    }


    // ���� �����ϴ� �ڷ�ƾ
    IEnumerator Land()
    {
        yield return new WaitForSeconds(3f);
        isFlying = false;
        isLook = true;
        currentState = BossState.Idle;
        wasFlied = true;
        boxCollider.enabled = true;
        isLanding = false;
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

                    if (ranAction < 25)
                    {
                        currentState = BossState.Attack1;
                    }
                    else if (ranAction < 60)
                    {
                        currentState = BossState.Attack2;
                    }
                    else if (ranAction < 80)
                    {
                        currentState = BossState.Attack3;
                    }
                    else if (ranAction < 100 && !wasFlied)
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
        wasFlied = false;
        currentState = BossState.Idle;
    }

    // Attack1���� ���� ��ü ��ȯ���ֱ�
    IEnumerator Strike(int idx)
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        // boxCollider.enabled = false;
        anim.SetTrigger("doAttack1");
        GameObject iceField = Instantiate(iceFieldPrefab, fallingSpots[idx].transform.position, Quaternion.Euler(0f, 0f, 0f));
        // fallingSpots[idx].SetActive(true);
        iceField.SetActive(true);

        yield return new WaitForSeconds(2f);
        // fallingSpots[idx].SetActive(false);
        Destroy(iceField);
        GameObject flameStrike = Instantiate(flameStrikePrefab, fallingSpots[idx].transform.position, fallingSpots[idx].transform.rotation);
        flameStrike.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        // explosions[idx].SetActive(true);

        yield return new WaitForSeconds(1.5f);

        // explosions[idx].SetActive(false);
        Destroy(flameStrike);
        // boxCollider.enabled = true;
        isLook = true;
    }


    void ShotIceBall()
    {
        StartCoroutine(IceBall());
    }

    IEnumerator IceBall()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doFire");

        yield return new WaitForSeconds(0.5f);
        GameObject IceBall = Instantiate(iceBallPrefab, mouth.position, mouth.rotation);
        Rigidbody rigidIce = IceBall.GetComponent<Rigidbody>();
        rigidIce.velocity = transform.forward * 75f;

        yield return new WaitForSeconds(2f);
        isLook = true;
        isAttack = false;
        wasFlied = false;
        currentState = BossState.Idle;

        yield return new WaitForSeconds(3f);
        Destroy(IceBall);
    }

    void TailAttack()
    {
        StartCoroutine(DoTailAttack());
    }

    IEnumerator DoTailAttack()
    {
        yield return new WaitForSeconds(0.1f);
        isLook = false;
        anim.SetTrigger("doTailAttack");

        yield return new WaitForSeconds(0.5f);
        GameObject TailSlash = Instantiate(TailAttackPrefab, TailRegion.position, TailRegion.rotation);


        yield return new WaitForSeconds(2f);
        isLook = true;
        isAttack = false;
        wasFlied = false;
        currentState = BossState.Idle;
        Destroy(TailSlash);

    }

    void TakeOff()
    {
        isStartFlying = true;
        boxCollider.enabled = false;
        anim.SetTrigger("doFly");

        StartCoroutine(MakeFlying());
    }

    IEnumerator MakeFlying()
    {
        yield return new WaitForSeconds(2.3f);
        isStartFlying = false;
        currentState = BossState.Flying;
    }

    void NowFlying()
    {
        anim.SetBool("isFly", true);

        int ranIdx = Random.Range(0, 6);

        ToGo = bossDomains[ranIdx].transform;

        Vector3 direction = ToGo.position - transform.position;

        while (direction.magnitude < 10f)
        {
            int newRanIdx = Random.Range(0, 6);

            ToGo = bossDomains[newRanIdx].transform;
        }

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
