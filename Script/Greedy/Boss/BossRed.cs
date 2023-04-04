using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using System.Linq;


public class BossRed : MonoBehaviourPunCallbacks, IPunObservable
{
    public int maxHealth;
    public int curHealth;

    [SerializeField]
    public BossPlayer targetPlayer;
    public Transform target;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Animator anim;
    NavMeshAgent nav;

    public GameObject targetOnSpot;
    public GameObject targetOnLongSpot;
    public GameObject clawSpot;
    public GameObject[] tsunamiSpots;
    public GameObject fireSpot;

    // Ÿ�� ����ð�
    float changeTargetTimeDelta;    // ����ġ
    float changeTargetTime;         // ����ġ

    // ���� ���� ��ȯ �ð�
    float changeSkillDelta;     // ����
    float changeSkillTime;     // ����ġ

    // �޸��� �ð�
    float runTimeDelta;
    float runTime;

    bool isNobodyTarget;
    bool isChase;
    bool isAttack;
    bool isDead;
    public bool isLine;


    // ���� 30�� ����
    float mapLavaTimeDelta;   // ����
    float mapLavaTime;        // ����ġ

    Vector3 lookVector;

    PhotonView pv;

    bool isScreamEnd;
    bool isScreamING;

    enum BossState { Attack1, Attack2, Attack3, Scream };
    BossState curState;


    void Awake()
    {
        pv = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        changeTargetTimeDelta = 100.0f;
        changeTargetTime = 10.0f;

        changeSkillDelta = 5.0f;
        changeSkillTime = 2.0f;

        mapLavaTime = 5.0f;

        runTime = 10.0f;
        nav.isStopped = true;
        boxCollider.enabled = false;
        curState = BossState.Scream;

        Invoke("DoScream", 2);
        Invoke("ChaseStart", 4);
    }


    void Start()
    {
        if (!photonView.IsMine)
        {
            BossGameManager gameManager = GameObject.FindObjectOfType<BossGameManager>();
            gameManager.bossRed = gameObject.GetComponent<BossRed>();
        }

        StartCoroutine("DoScream");
    }

    void ChaseStart()
    {
        isChase = true;
    }


    void FixedUpdate()
    {
        FreezeVelocity();
    }


    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (curState == BossState.Scream) return;

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

            changeTargetTimeDelta += Time.deltaTime;

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

                mapLavaTimeDelta += Time.deltaTime;

                if (((float)curHealth <= (float)maxHealth * 0.3) && (mapLavaTimeDelta >= mapLavaTime))
                {
                    StartCoroutine("LavaField");
                    mapLavaTimeDelta = 0.0f;
                }

                if (isAttack)
                    return;

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
                            anim.SetBool("isRun", false);
                            transform.LookAt(target);
                            nav.isStopped = true;
                            isChase = false;

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
                        if (runTimeDelta >= runTime || targetOnLongSpot.GetComponent<BossTarget>().isTargetOn)
                        {
                            anim.SetBool("isRun", false);
                            transform.LookAt(target);
                            nav.isStopped = true;
                            isChase = false;

                            isAttack = true;
                            StartCoroutine("DoBasicAttack");
                        }
                        break;
                    case BossState.Attack3:
                        isAttack = true;
                        StartCoroutine("DoFireShot");
                        break;
                }
            }
        }
    }

    IEnumerator DoScream()
    {
        isScreamING = true;
        yield return new WaitForSeconds(3.0f);

        anim.SetTrigger("doScream");

        yield return new WaitForSeconds(4.0f);
        isScreamEnd = true;
        boxCollider.enabled = true;
        nav.isStopped = false;
        curState = BossState.Attack1;
    }

    IEnumerator DoClawAttack()
    {
        anim.SetTrigger("doClawAttack");
        FreezeVelocity();
        yield return new WaitForSeconds(0.7f);

        GameObject instantClawEffect = PhotonNetwork.Instantiate("FlameScatter", clawSpot.transform.position, clawSpot.transform.rotation);

        yield return new WaitForSeconds(2.0f);
        Destroy(instantClawEffect);
        isAttack = false;
        runTimeDelta = 0.0f;
    }


    IEnumerator DoBasicAttack()
    {
        yield return null;

        yield return new WaitForSeconds(1.0f);
        isLine = true;
        anim.SetTrigger("doBasicAttack");
        GameObject instantTsunami1 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[0].transform.position, tsunamiSpots[0].transform.rotation);
        GameObject instantTsunami2 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[1].transform.position, tsunamiSpots[1].transform.rotation);
        GameObject instantTsunami3 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[2].transform.position, tsunamiSpots[2].transform.rotation);
        GameObject instantTsunami4 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[3].transform.position, tsunamiSpots[3].transform.rotation);

        yield return new WaitForSeconds(1.0f);
        Destroy(instantTsunami1);
        Destroy(instantTsunami2);
        Destroy(instantTsunami3);
        Destroy(instantTsunami4);


        yield return new WaitForSeconds(1.0f);
        isLine = false;
        anim.SetTrigger("doBasicAttack");
        GameObject instantTsunami5 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[0].transform.position, tsunamiSpots[0].transform.rotation);
        GameObject instantTsunami6 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[1].transform.position, tsunamiSpots[1].transform.rotation);
        GameObject instantTsunami7 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[2].transform.position, tsunamiSpots[2].transform.rotation);
        GameObject instantTsunami8 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[3].transform.position, tsunamiSpots[3].transform.rotation);

        yield return new WaitForSeconds(1.0f);
        Destroy(instantTsunami5);
        Destroy(instantTsunami6);
        Destroy(instantTsunami7);
        Destroy(instantTsunami8);
        isAttack = false;
        runTimeDelta = 0.0f;
    }


    IEnumerator DoFireShot()
    {
        nav.isStopped = true;
        transform.LookAt(target);
        FreezeVelocity();

        anim.SetTrigger("doFlameAttack");

        yield return new WaitForSeconds(0.5f);

        GameObject fireShot1 = PhotonNetwork.Instantiate("FireShot", fireSpot.transform.position, fireSpot.transform.rotation);

        yield return new WaitForSeconds(0.5f);

        transform.LookAt(target);
        FreezeVelocity();

        anim.SetTrigger("doFlameAttack");

        yield return new WaitForSeconds(0.5f);

        GameObject fireShot2 = PhotonNetwork.Instantiate("FireShot", fireSpot.transform.position, fireSpot.transform.rotation);

        yield return new WaitForSeconds(0.5f);

        transform.LookAt(target);
        FreezeVelocity();

        anim.SetTrigger("doFlameAttack");

        yield return new WaitForSeconds(0.5f);

        GameObject fireShot3 = PhotonNetwork.Instantiate("FireShot", fireSpot.transform.position, fireSpot.transform.rotation);

        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(2f);
        isAttack = false;
        Destroy(fireShot1);
        Destroy(fireShot2);
        Destroy(fireShot3);
    }

    IEnumerator LavaField()
    {
        yield return null;
        LavaSpots lavaSpots = GameObject.FindObjectOfType<LavaSpots>();
        GameObject[] lavaFields = lavaSpots.lavaSpots;

        int ranCounts = Random.Range(15, 20);
        GameObject[] selectedFields = new GameObject[ranCounts];

        for (int i = 0; i < ranCounts; i++)
        {
            int index = Random.Range(0, lavaFields.Length);
            selectedFields[i] = lavaFields[index];
            lavaFields = lavaFields.Where((val, idx) => idx != index).ToArray();
        }

        for (int i = 0; i < selectedFields.Length; i++)
        {
            GameObject instantLavaEffect = PhotonNetwork.Instantiate("LavaStartEffect", selectedFields[i].transform.position, selectedFields[i].transform.rotation);
            Destroy(instantLavaEffect, 2.0f);
        }

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < selectedFields.Length; i++)
        {
            GameObject instantLavaAttack = PhotonNetwork.Instantiate("MagmaField", selectedFields[i].transform.position, selectedFields[i].transform.rotation);
            Destroy(instantLavaAttack, 3.0f);
        }

        yield return null;
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
