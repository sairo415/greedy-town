using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossGolem : MonoBehaviour
{
    // 체력
    public int maxHealth;
    public int curHealth;

    // 목표물
    public BossPlayer targetPlayer;
    public Transform target;

    Rigidbody rigid;
    BoxCollider boxCollider;
    NavMeshAgent nav;

    // 타겟 변경 시간
    float changeTargetTimeDelta;    // 측정
    float changeTargetTime;         // 기준치

    // 추적을 결정하는 bool 변수
    public bool isChase;

    bool isAttackING; // 공격 진행중

    Animator anim;

    // 공격
    public GameObject attackSpot;
    public GameObject targetOnSpot;

    // Photon
    private PhotonView pv;

    // 죽음
    bool isDie;

    private void Awake()
	{
        pv = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        changeTargetTimeDelta = 100.0f;
        changeTargetTime = 10.0f;

        Invoke("ChaseStart", 2);
    }

    // 추적 개시
    void ChaseStart()
    {
        isChase = true;
    }

    void FreezeVelocity()
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if(isDie)
            return;

        // 마스터 클라이언트 기준으로 타겟을 지정함.
        if(PhotonNetwork.IsMasterClient)
        {
            //플레이어 정보들을 받아온 다음 그 중에서 랜덤으로 타겟을 지정함. 타겟은 10초마다 변경될 것
            changeTargetTimeDelta += Time.deltaTime;
            if(changeTargetTimeDelta >= changeTargetTime)
            {
                BossPlayer[] bossPlayers = FindObjectsOfType<BossPlayer>();
                List<BossPlayer> bossPlayersAlive = new List<BossPlayer>();

                foreach(BossPlayer bossPlayer in bossPlayers)
                {
                    if(!bossPlayer.isDie)
                    {
                        bossPlayersAlive.Add(bossPlayer);
                    }
                }

                if(bossPlayersAlive.Count == 0)
                    return;

                int index = Random.Range(0, bossPlayersAlive.Count);
                targetPlayer = bossPlayersAlive[index];
                target = bossPlayersAlive[index].transform;

                changeTargetTimeDelta = 0.0f;
            }

            // 타겟 정보를 다른 클라이언트에 공유
            if(target != null)
                pv.RPC("ShareTargetPlayerViewID", RpcTarget.Others, targetPlayer.pv.ViewID);
        }

        if(target != null)
        {
            if(isAttackING)
                return;

            if(targetOnSpot.GetComponent<BossTarget>().isTargetOn)
            {
                // 추적 중단
                nav.isStopped = true;
                FreezeVelocity();
                isChase = false;
                anim.SetBool("isRun", false);

                // 공격
                isAttackING = true;
                anim.SetTrigger("doAttack");
                StartCoroutine("DoAttack");
            }
            else
            {
                // 추적 재시작
                nav.isStopped = false;
                isChase = true;
                anim.SetBool("isRun", true);
                nav.SetDestination(target.position);
            }
        }
    }

	[PunRPC]
    void ShareTargetPlayerViewID(int viewID)
    {
        target = PhotonView.Find(viewID).GetComponent<BossPlayer>().transform;
    }

    IEnumerator DoAttack()
    {
        yield return new WaitForSeconds(1.7f);

        attackSpot.SetActive(true);

        yield return new WaitForSeconds(0.4f);

        attackSpot.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        isAttackING = false;

        yield return null;
    }

	// 플레이어와 물리 충돌이 나면 따라다니질 못하는 문제 해결
	void FixedUpdate()
    {
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if(isDie)
            return;

        //맞은 스킬의 소유주 파악.
        //해당 소유주가 이 보스가 위치한 곳의 플레이어아이디와 같으면 트리거 적용
        //적용 완료후 RPC Other

        if(other != null && (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver" || other.tag == "PlayerAttackDot"))
        {
            anim.SetTrigger("doGetHit");

            // 맞은 스킬의 시전자
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // 현재 위치한 클라이언트
            BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

            // Owner ID
            int myPlayerID = curClient.pv.ViewID;

            // 내가 사용한 스킬이 아닐 경우 로직 실행 안함.
            if(skillOwnerID != myPlayerID)
                return;

            if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
            {
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(curHealth < 0)
                    curHealth = 0;

                StartCoroutine("OnDamage");

                // 시전자가 피흡을 가지고 있으면 체력을 회복시킨다.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 2;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // 회복시킨 후 동기화 필요할 듯...
                    // isVampirism == true 일 때, q 를 사용할 때마다, 여기서 SyncBossHealth 한 것 처럼
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // 서버 보스 체력과 동기화
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // 적과 닿았을 때 이펙트 삭제되도록 Destroy() 호출
                // tag PlayerAttack => 닿으면 삭제되는 이펙트
                // tag PlayerAttackOver => 닿으면 삭제되지 않는 이펙트
                if(other.tag == "PlayerAttack")
                {
                    Destroy(other.gameObject);
                    other.gameObject.SetActive(false);
                }
            }
            else if(other.tag == "PlayerAttackDot")
            {
                other.GetComponent<BossPlayerSkill>().isInBoss = true;
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if(isDie)
            return;

        if(other != null && other.tag == "PlayerAttackDot")
        {
            other.GetComponent<BossPlayerSkill>().isInBoss = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(isDie)
            return;

        if(other != null && other.CompareTag("PlayerAttackDot") && other.GetComponent<BossPlayerSkill>().isInBoss)
        {
            other.GetComponent<BossPlayerSkill>().damageTimer += Time.deltaTime;

            if(other.GetComponent<BossPlayerSkill>().damageTimer >= other.GetComponent<BossPlayerSkill>().damageInterval)
            {
                curHealth -= other.GetComponent<BossPlayerSkill>().damage;
                if(curHealth <= 0)
                {
                    curHealth = 0;
                    StartCoroutine("OnDamage");
                }

                BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

                // 시전자가 피흡을 가지고 있으면 체력을 회복시킨다.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // 회복시킨 후 동기화 필요할 듯...
                    // isVampirism == true 일 때, q 를 사용할 때마다, 여기서 SyncBossHealth 한 것 처럼
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // 서버 보스 체력과 동기화
                pv.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                other.GetComponent<BossPlayerSkill>().damageTimer = 0f;
            }
        }
    }

    IEnumerator OnDamage()
    {
        yield return new WaitForSeconds(0.1f);

        if(curHealth <= 0)
        {
            gameObject.layer = 11;
            isDie = true;

            // 추적 중단
            nav.isStopped = true;
            FreezeVelocity();
            isChase = false;
            anim.SetBool("isRun", false);

            anim.SetTrigger("doDie");

            Destroy(gameObject, 4); // 4초 후에 사라짐
        }
    }

    // 보스 체력을 다른 클라이언트의 보스 체력과 동기화
    [PunRPC]
    void SyncBossHealth(int health)
    {
        curHealth = health;
        StartCoroutine("OnDamage");
    }
}
