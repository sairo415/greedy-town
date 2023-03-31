using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossIceLich : MonoBehaviour
{
    // ���� �ൿ ���� ����
    private enum State { Idle, Attack1, Attack2, Die, Teleport };

    // ���� ü�µ�
    public int currentHealth;
    public int maxHealth;
    public bool isDead = false;
    public bool isLook;
    public bool isTeleport;

    public GameObject moveEffectSpot;
    public GameObject moveEffectPrefab;


    Vector3 lookVector;

    // �߰� ���
    public Transform target;

    public GameObject[] lichSpots;

    // ���� ����
    private State currentState;
    public CapsuleCollider capsuleCollider;
    public Rigidbody rigid;
    public Animator anim;
    public NavMeshAgent nav;

    // bool����
    bool isAttack;

    // ��ġ
    public GameObject[] tornados;
    public GameObject lazerPoint;

    // ����
    public GameObject tornadoPrefabs;
    public GameObject lazerBeamPrefabs;

    private PhotonView pv;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();

        nav.isStopped = true;
        isLook = true;
    }

    void Start()
    {
        currentState = State.Idle;
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
        //���� ��ų�� ������ �ľ�.
        //�ش� �����ְ� �� ������ ��ġ�� ���� �÷��̾���̵�� ������ Ʈ���� ����
        //���� �Ϸ��� RPC Other

        if(other != null && (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver" || other.tag == "PlayerAttackDot"))
        {
            // ���� ��ų�� ������
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // ���� ��ġ�� Ŭ���̾�Ʈ
            BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

            // Owner ID
            int myPlayerID = curClient.pv.ViewID;

            // ���� ����� ��ų�� �ƴ� ��� ���� ���� ����.
            if(skillOwnerID != myPlayerID)
                return;

            if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
            {
                currentHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(currentHealth < 0)
                    currentHealth = 0;

                // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // ȸ����Ų �� ����ȭ �ʿ��� ��...
                    // isVampirism == true �� ��, q �� ����� ������, ���⼭ SyncBossHealth �� �� ó��
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = currentHealth;

                // ���� ���� ü�°� ����ȭ
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // ���� ����� �� ����Ʈ �����ǵ��� Destroy() ȣ��
                // tag PlayerAttack => ������ �����Ǵ� ����Ʈ
                // tag PlayerAttackOver => ������ �������� �ʴ� ����Ʈ
                if(other.tag == "PlayerAttack")
                {
                    Destroy(other.gameObject);
                    other.gameObject.SetActive(false);
                }

                StartCoroutine("OnDamage");
            }
            else if(other.tag == "PlayerAttackDot")
            {
                other.GetComponent<BossPlayerSkill>().isInBoss = true;
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other != null && other.tag == "PlayerAttackDot")
        {
            other.GetComponent<BossPlayerSkill>().isInBoss = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other != null && other.CompareTag("PlayerAttackDot") && other.GetComponent<BossPlayerSkill>().isInBoss)
        {

            Debug.Log("DotDam1");

            other.GetComponent<BossPlayerSkill>().damageTimer += Time.deltaTime;

            if(other.GetComponent<BossPlayerSkill>().damageTimer >= other.GetComponent<BossPlayerSkill>().damageInterval)
            {
                currentHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(currentHealth < 0)
                    currentHealth = 0;

                BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

                // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // ȸ����Ų �� ����ȭ �ʿ��� ��...
                    // isVampirism == true �� ��, q �� ����� ������, ���⼭ SyncBossHealth �� �� ó��
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = currentHealth;

                // ���� ���� ü�°� ����ȭ
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                StartCoroutine("OnDamage");

                other.GetComponent<BossPlayerSkill>().damageTimer = 0f;
            }
        }
    }

    // ���� ü���� �ٸ� Ŭ���̾�Ʈ�� ���� ü�°� ����ȭ
    [PunRPC]
    void SyncBossHealth(int health)
    {
        currentHealth = health;
    }
}
