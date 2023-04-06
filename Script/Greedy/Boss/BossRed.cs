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
    private BossPlayer targetPlayer;
    private Transform target;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Animator anim;
    NavMeshAgent nav;

    public GameObject targetOnSpot;
    public GameObject targetOnLongSpot;
    public GameObject clawSpot;
    public GameObject[] tsunamiSpots;
    public GameObject fireSpot;

    // 타겟 변경시간
    float changeTargetTimeDelta;    // 측정치
    float changeTargetTime;         // 기준치

    // 공격 패턴 전환 시간
    float changeSkillDelta;     // 측정
    float changeSkillTime;     // 기준치

    // 달리기 시간
    float runTimeDelta;
    float runTime;

    bool isNobodyTarget;
    bool isChase;
    bool isAttack;
    bool isDead;
    public bool isLine;


    // 피통 30퍼 이하
    float mapLavaTimeDelta;   // 측정
    float mapLavaTime;        // 기준치

    bool isScreamEnd;
    bool isScreamING;

    AudioSource skillSound;
    public AudioClip dragonScreamSound;

    enum BossState { Attack1, Attack2, Attack3, Scream };
    BossState curState;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        skillSound = GetComponent<AudioSource>();

        changeTargetTimeDelta = 100.0f;
        changeTargetTime = 10.0f;

        changeSkillDelta = 5.0f;
        changeSkillTime = 2.0f;

        mapLavaTime = 5.0f;

        runTime = 10.0f;
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

            if (isDead) return;

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

            int ranAction = Random.Range(0, 100);

            if (ranAction < 40)
            {
                curState = BossState.Attack1;
            }
            else if (ranAction < 80)
            {
                curState = BossState.Attack2;
            }
            else
            {
                curState = BossState.Attack3;
            }
            // curState = (BossState)UnityEngine.Random.Range(0, 3);

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
                            FreezeVelocity();
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
                            FreezeVelocity();
                            isChase = false;

                            isAttack = true;
                            StartCoroutine("DoBasicAttack");
                        }
                        break;
                    case BossState.Attack3:
                        isAttack = true;
                        FreezeVelocity();
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
        skillSound.clip = dragonScreamSound;
        skillSound.loop = false;
        skillSound.Play();

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
        PhotonNetwork.Destroy(instantClawEffect);
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

        yield return new WaitForSeconds(1.5f);
        isLine = false;
        anim.SetTrigger("doBasicAttack");
        GameObject instantTsunami5 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[0].transform.position, tsunamiSpots[0].transform.rotation);
        GameObject instantTsunami6 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[1].transform.position, tsunamiSpots[1].transform.rotation);
        GameObject instantTsunami7 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[2].transform.position, tsunamiSpots[2].transform.rotation);
        GameObject instantTsunami8 = PhotonNetwork.Instantiate("FlameTsunami", tsunamiSpots[3].transform.position, tsunamiSpots[3].transform.rotation);

        yield return new WaitForSeconds(1.0f);
        PhotonNetwork.Destroy(instantTsunami1);
        PhotonNetwork.Destroy(instantTsunami2);
        PhotonNetwork.Destroy(instantTsunami3);
        PhotonNetwork.Destroy(instantTsunami4);
        PhotonNetwork.Destroy(instantTsunami5);
        PhotonNetwork.Destroy(instantTsunami6);
        PhotonNetwork.Destroy(instantTsunami7);
        PhotonNetwork.Destroy(instantTsunami8);
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
        PhotonNetwork.Destroy(fireShot1);
        PhotonNetwork.Destroy(fireShot2);
        PhotonNetwork.Destroy(fireShot3);
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

        GameObject[] beforeEffect = new GameObject[selectedFields.Length];

        for (int i = 0; i < selectedFields.Length; i++)
        {
            beforeEffect[i] = PhotonNetwork.Instantiate("LavaStartEffect", selectedFields[i].transform.position, selectedFields[i].transform.rotation);
            // PhotonNetwork.Destroy(instantLavaEffect, 2.0f);
        }

        yield return new WaitForSeconds(1.5f);

        GameObject[] hitEffect = new GameObject[selectedFields.Length];

        for (int i = 0; i < selectedFields.Length; i++)
        {
            hitEffect[i] = PhotonNetwork.Instantiate("MagmaField", selectedFields[i].transform.position, selectedFields[i].transform.rotation);
            // PhotonNetwork.Destroy(instantLavaAttack, 3.0f);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < selectedFields.Length; i++)
        {
            PhotonNetwork.Destroy(beforeEffect[i]);
        }

        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < selectedFields.Length; i++)
        {
            PhotonNetwork.Destroy(hitEffect[i]);
        }

        yield return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead)
            return;

        //맞은 스킬의 소유주 파악.
        //해당 소유주가 이 보스가 위치한 곳의 플레이어아이디와 같으면 트리거 적용
        //적용 완료후 RPC Other

        if (other != null && (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver" || other.tag == "PlayerAttackDot"))
        {
            // 맞은 스킬의 시전자
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // 현재 위치한 클라이언트
            BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

            // Owner ID
            int myPlayerID = curClient.pv.ViewID;

            // 내가 사용한 스킬이 아닐 경우 로직 실행 안함.
            if (skillOwnerID != myPlayerID)
                return;

            if (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
            {
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if (curHealth < 0)
                    curHealth = 0;

                StartCoroutine("OnDamage");

                // 시전자가 피흡을 가지고 있으면 체력을 회복시킨다.
                if (curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 2;
                    if (vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // 회복시킨 후 동기화 필요할 듯...
                    // isVampirism == true 일 때, q 를 사용할 때마다, 여기서 SyncBossHealth 한 것 처럼
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // 서버 보스 체력과 동기화
                photonView.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // 적과 닿았을 때 이펙트 삭제되도록 Destroy() 호출
                // tag PlayerAttack => 닿으면 삭제되는 이펙트
                // tag PlayerAttackOver => 닿으면 삭제되지 않는 이펙트
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

                // 시전자가 피흡을 가지고 있으면 체력을 회복시킨다.
                if (curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if (vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // 회복시킨 후 동기화 필요할 듯...
                    // isVampirism == true 일 때, q 를 사용할 때마다, 여기서 SyncBossHealth 한 것 처럼
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // 서버 보스 체력과 동기화
                photonView.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

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

            // 추적 중단
            nav.isStopped = true;
            FreezeVelocity();
            isChase = false;
            anim.SetBool("isRun", false);

            anim.SetTrigger("doDie");

            Debug.Log("사망");

            Destroy(gameObject, 20);
        }
    }

    // 보스 체력을 다른 클라이언트의 보스 체력과 동기화
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
