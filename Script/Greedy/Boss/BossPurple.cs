using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;


public class BossPurple : MonoBehaviourPunCallbacks, IPunObservable
{
    // ���� ü��
    public int maxHealth;
    public int curHealth;

    // ��ǥ�� ����
    [SerializeField]
    public BossPlayer targetPlayer;
    public Transform target;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Animator anim;
    NavMeshAgent nav;

    public GameObject attackSpot;
    public GameObject targetOnSpot;
    public GameObject clawSpot;
    // public GameObject clawEffect;
    public GameObject flameSpot;
    // public GameObject flameEffect;
    // public GameObject basicEffect;

    // Ÿ�� ����ð�
    float changeTargetTimeDelta;    // ����ġ
    float changeTargetTime;         // ����ġ

    // ���� ���� ��ȯ �ð�
    float changeSkillDelta;     // ����
    float changeSkillTime;     // ����ġ

    // �޸��� �ð�
    float runTimeDelta;
    float runTime;

    // ��� �÷��̾ ���
    bool isNobodyTarget;

    public bool isChase;
    bool isAttack;
    bool isDead;
    bool isLook;

    Vector3 lookVector;

    PhotonView pv;

    bool isScreamEnd;
    bool isScreamING;

    int selectedSkillIdx;

    enum BossState { Attack1, Attack2, Attack3, Scream };
    BossState curState;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        changeTargetTimeDelta = 100.0f;
        changeTargetTime = 10.0f;

        changeSkillDelta = 100.0f;     // ����
        changeSkillTime = 2.0f;     // ����ġ

        runTime = 10.0f;
        nav.isStopped = true;
        curState = BossState.Scream;

        Invoke("ChaseStart", 2);
    }


    void Start()
    {
        // ������ Ŭ���̾�Ʈ�� �ƴ� Ŭ���̾�Ʈ���� ü�� UI ����ȭ�� ���� ���� �޴����� �������� ������Ʈ�� �Է��Ѵ�.
        if (!photonView.IsMine)
        {
            BossGameManager gameManager = GameObject.FindObjectOfType<BossGameManager>();
            gameManager.bossPurple = gameObject.GetComponent<BossPurple>();
        }

        // ��ȿ
        StartCoroutine("DoScream");
    }


    // �߰�
    void ChaseStart()
    {
        isChase = true;
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (curState == BossState.Scream)
            return;

        if (photonView.IsMine)
        {
            if (isDead) return;

            if (!isDead && curHealth <= 0)
            {
                gameObject.layer = LayerMask.NameToLayer("BossDead");
                isDead = true;

                nav.isStopped = true;
                isChase = false;
                anim.SetBool("isRun", false);
                anim.SetTrigger("doDie");

                Destroy(gameObject, 4);
            }

            // Ÿ�� ���� �ð�
            changeTargetTimeDelta += Time.deltaTime;

            // ����ִ� �÷��̾� �� Ÿ���� �������� ����
            if (changeTargetTimeDelta >= changeTargetTime && !isAttack)
            {
                BossPlayer[] bossPlayers = FindObjectsOfType<BossPlayer>();
                List<BossPlayer> bossPlayersAlive = new List<BossPlayer>();

                foreach (BossPlayer bossPlayer in bossPlayers)
                {
                    if (!bossPlayer.isDie)
                    {
                        bossPlayersAlive.Add(bossPlayer);
                    }
                }

                if (bossPlayersAlive.Count == 0) return;

                int ranidx = Random.Range(0, bossPlayersAlive.Count);
                targetPlayer = bossPlayersAlive[ranidx];
                target = targetPlayer.transform;

                changeTargetTimeDelta = 0.0f;
            }

            curState = (BossState)UnityEngine.Random.Range(0, 3);

            if (target != null)
            {
                runTimeDelta += Time.deltaTime;

                if (!isScreamEnd) return;

                if (isAttack) return;


                switch (curState)
                {
                    case BossState.Attack1:
                        anim.SetBool("isRun", true);
                        nav.isStopped = false;
                        isChase = true;
                        
                        nav.SetDestination(target.position);

                        runTimeDelta += Time.deltaTime;
                        if (runTimeDelta >= runTime || targetOnSpot.GetComponent<BossTarget>().isTargetOn)
                        {
                            nav.isStopped = true;
                            isChase = false;
                            anim.SetBool("isRun", false);

                            isAttack = true;
                            StartCoroutine("DoClawAttack");
                        }
                        break;
                    case BossState.Attack2:
                        anim.SetBool("isRun", true);
                        nav.isStopped = false;
                        isChase = true;

                        nav.SetDestination(target.position);

                        runTimeDelta += Time.deltaTime;
                        if (runTimeDelta >= runTime || targetOnSpot.GetComponent<BossTarget>().isTargetOn)
                        {
                            nav.isStopped = true;
                            isChase = false;
                            anim.SetBool("isRun", false);

                            isAttack = true;
                            StartCoroutine("DoFlameAttack");
                        }
                        break;
                    case BossState.Attack3:
                        anim.SetBool("isRun", true);
                        nav.isStopped = false;
                        isChase = true;

                        nav.SetDestination(target.position);

                        runTimeDelta += Time.deltaTime;
                        if (runTimeDelta >= runTime || targetOnSpot.GetComponent<BossTarget>().isTargetOn)
                        {
                            nav.isStopped = true;
                            isChase = false;
                            anim.SetBool("isRun", false);

                            isAttack = true;
                            StartCoroutine("DoBasicAttack");
                        }
                        break;
                }
            }
        }
    }

    IEnumerator DoScream()
    {
        isScreamING = true;
        yield return new WaitForSeconds(1.0f);

        anim.SetTrigger("doScream");

        yield return new WaitForSeconds(4.0f);
        isScreamEnd = true;
        curState = BossState.Attack1;
    }

    IEnumerator DoClawAttack()
    {
        anim.SetTrigger("doClawAttack");
        yield return new WaitForSeconds(1.0f);

        GameObject instantClawEffect = PhotonNetwork.Instantiate("ClawSlash", clawSpot.transform.position, clawSpot.transform.rotation);
        // attackSpot.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        Destroy(instantClawEffect);
        // attackSpot.SetActive(false);
        isAttack = false;
        runTimeDelta = 0.0f;
    }

    IEnumerator DoFlameAttack()
    {
        anim.SetTrigger("doFlameAttack");
        yield return new WaitForSeconds(1.0f);

        // flameEffect.SetActive(true);
        GameObject instantAbyss = PhotonNetwork.Instantiate("AbyssEffect", transform.position, transform.rotation);


        yield return new WaitForSeconds(3.0f);
        // flameEffect.SetActive(false);
        Destroy(instantAbyss);

        yield return new WaitForSeconds(1f);
        isAttack = false;
        runTimeDelta = 0.0f;
    }

    IEnumerator DoBasicAttack()
    {
        anim.SetTrigger("doBasicAttack");
        yield return new WaitForSeconds(0.5f);

        GameObject instantBasicEffect = PhotonNetwork.Instantiate("BasicReleaseEffect", flameSpot.transform.position, flameSpot.transform.rotation);

        // attackSpot.SetActive(true);
        // basicEffect.SetActive(true);

        // yield return new WaitForSeconds(0.5f);
        // basicEffect.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        // attackSpot.SetActive(false);
        Destroy(instantBasicEffect);

        yield return new WaitForSeconds(0.5f);
        isAttack = false;
        runTimeDelta = 0.0f;

    }


    // �÷��̾�� ���� �浹�� ���� ����ٴ��� ���ϴ� ���� �ذ�
    void FixedUpdate()
    {
        FreezeVelocity();
    }


    void OnTriggerEnter(Collider other)
    {
        if (isDead)
            return;

        //���� ��ų�� ������ �ľ�.
        //�ش� �����ְ� �� ������ ��ġ�� ���� �÷��̾���̵�� ������ Ʈ���� ����
        //���� �Ϸ��� RPC Other

        if (other != null && (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver" || other.tag == "PlayerAttackDot"))
        {
            // ���� ��ų�� ������
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // ���� ��ġ�� Ŭ���̾�Ʈ
            BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

            // Owner ID
            int myPlayerID = curClient.pv.ViewID;

            // ���� ����� ��ų�� �ƴ� ��� ���� ���� ����.
            if (skillOwnerID != myPlayerID)
                return;

            if (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
            {
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if (curHealth < 0)
                    curHealth = 0;

                StartCoroutine("OnDamage");

                // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
                if (curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 2;
                    if (vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // ȸ����Ų �� ����ȭ �ʿ��� ��...
                    // isVampirism == true �� ��, q �� ����� ������, ���⼭ SyncBossHealth �� �� ó��
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // ���� ���� ü�°� ����ȭ
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // ���� ����� �� ����Ʈ �����ǵ��� Destroy() ȣ��
                // tag PlayerAttack => ������ �����Ǵ� ����Ʈ
                // tag PlayerAttackOver => ������ �������� �ʴ� ����Ʈ
                if (other.tag == "PlayerAttack")
                {
                    Destroy(other.gameObject);
                    other.gameObject.SetActive(false);
                }

                StartCoroutine("OnDamage");
            }
            else if (other.tag == "PlayerAttackDot")
            {
                other.GetComponent<BossPlayerSkill>().isInBoss = true;
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isDead)
            return;

        if (other != null && other.tag == "PlayerAttackDot")
        {
            other.GetComponent<BossPlayerSkill>().isInBoss = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isDead)
            return;

        if (other != null && other.CompareTag("PlayerAttackDot") && other.GetComponent<BossPlayerSkill>().isInBoss)
        {
            other.GetComponent<BossPlayerSkill>().damageTimer += Time.deltaTime;

            if (other.GetComponent<BossPlayerSkill>().damageTimer >= other.GetComponent<BossPlayerSkill>().damageInterval)
            {
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if (curHealth < 0)
                    curHealth = 0;

                StartCoroutine("OnDamage");

                BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

                // �����ڰ� ������ ������ ������ ü���� ȸ����Ų��.
                if (curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if (vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // ȸ����Ų �� ����ȭ �ʿ��� ��...
                    // isVampirism == true �� ��, q �� ����� ������, ���⼭ SyncBossHealth �� �� ó��
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // ���� ���� ü�°� ����ȭ
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                StartCoroutine("OnDamage");

                other.GetComponent<BossPlayerSkill>().damageTimer = 0f;
            }
        }
    }

    IEnumerator OnDamage()
    {
        yield return new WaitForSeconds(0.1f);

        if (curHealth <= 0)
        {
            gameObject.layer = 11;
            isDead = true;

            // ���� �ߴ�
            nav.isStopped = true;
            FreezeVelocity();
            isChase = false;
            anim.SetBool("isRun", false);

            anim.SetTrigger("doDie");

            Debug.Log("���");

            Destroy(gameObject, 20);
        }
    }

    // ���� ü���� �ٸ� Ŭ���̾�Ʈ�� ���� ü�°� ����ȭ
    [PunRPC]
    void SyncBossHealth(int health)
    {
        curHealth = health;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
        }
        else
        {
            // Network player, receive data
        }
    }
}