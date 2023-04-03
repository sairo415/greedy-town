using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossGreyDragon : MonoBehaviourPunCallbacks, IPunObservable
{
    // 체력
    public int maxHealth;   // 최대 체력
    public int curHealth;   // 현재 체력

    // 목표 플레이어
    [SerializeField]
    private BossPlayer targetPlayer;
    private Transform targetPlayerTransform;

    // 목표 추적 AI
    NavMeshAgent nav;

    // 물리 컴포넌트
    Rigidbody rigid;

    // 애니메이션
    Animator anim;

    // 플래그 변수
    bool isDie;         // 사망
    bool isSreamEnd;    // 표호 종료

    // 사운드
    AudioSource skillSound;
    public AudioClip dragonSreamSound;

    // 타겟 변경 시간
    float changeTargetTimeDelta;    // 측정
    float changeTargetTime;         // 기준치

    // 추적을 결정하는 bool 변수 추가, 죽으면 false 로
    public bool isChase;

    // 공격 진행중. 공격을 하는 동안 다른 공격을 하지 않도록 제한함.
    bool isAttackING;

    // 공격 지점
    public Transform iceBallPos;
    public Transform flyAttackPos;

    // 날아다니는 시간
    float flyTimeDelta; // 측정
    float flyTime;      // 기준치

    // 랜덤 스킬 선택 인덱스
    int selectedSkillIdx;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        skillSound = GetComponent<AudioSource>();

        // 타겟을 변경하는 시간 측정치 초기화
        changeTargetTimeDelta = 100.0f;
        changeTargetTime = 10.0f;

        // 날아다니는 시간
        flyTime = 10.0f;

        Invoke("ChaseStart", 2);
    }

    // 추적 시작
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

    void Start()
    {
        // 마스터 클라이언트가 아닌 클라이언트들은 체력 UI 동기화를 위해 게임 메니저에 수동으로 컴포넌트를 입력한다.
        if(!photonView.IsMine)
        {
            BossGameManager gameManager = GameObject.FindObjectOfType<BossGameManager>();
            gameManager.bossGrey = gameObject.GetComponent<BossGreyDragon>();
        }

        // 포효
        //StartCoroutine("DoScream");
    }

    void Update()
    {
        // 포효가 종료되면 보스 공격 시작됨
        //if(!isSreamEnd)
        //    return;

        if(photonView.IsMine)
        {

        }
    }

    // 스폰되면 포효
    /*IEnumerator DoScream()
    {
        yield return new WaitForSeconds(1.0f);

        anim.SetTrigger("doScream");
        skillSound.clip = dragonSreamSound;
        skillSound.loop = false;
        skillSound.Play();

        yield return new WaitForSeconds(4.0f);

        isSreamEnd = true;

        yield return null;
    }*/

    // 플레이어와 물리 충돌이 나면 따라다니질 못하는 문제 해결
    void FixedUpdate()
    {
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if(isDie)
            return;

        // 여러 클라이언트에서 같은 ViewID 를 가진 클론의 한번의 공격이 클라이언트 마다 중복해서 여러번 적용되는 것을 제한함.
        //  1. 맞은 스킬의 시전자 파악.
        //  2. 해당 스킬의 시전자가 현재의 클라이언트와 동일한 경우에만 데미지 적용함.
        //  3. 적용 완료 후 체력 동기화를 위해 RPC (Others) 함수 호출
        if(other != null && (other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver" || other.tag == "PlayerAttackDot"))
        {
            // PlayerAttack     : 몬스터가 맞으면 이펙트가 사라지는 공격
            // PlayerAttackOver : 몬스터가 맞아도 이펙트가 사라지지 않는 공격
            // PlayerAttackDot  : 도트 데미지 공격

            // 맞은 스킬의 시전자
            int skillOwnerID = other.GetComponent<BossPlayerSkill>().GetID();
            // 현재 위치한 클라이언트
            BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

            if(curClient.pv == null)
                return;

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

                    // 피흡된 체력을 다른 클라이언트와 동기화
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // 보스 체력을 다른 클라이언트와 동기화
                photonView.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                // 적과 닿았을 때 이펙트 삭제되도록 Destroy() 호출
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
                if(curHealth < 0)
                    curHealth = 0;

                StartCoroutine("OnDamage");

                BossPlayer curClient = GameObject.FindObjectOfType<BossGameManager>().player;

                // 시전자가 피흡을 가지고 있으면 체력을 회복시킨다.
                if(curClient.isVampirism && other.GetComponent<BossPlayerSkill>().isVampirism)
                {
                    int vamHP = curClient.curHealth + 10;
                    if(vamHP > curClient.maxHealth)
                        vamHP = curClient.maxHealth;
                    curClient.curHealth = vamHP;

                    // 피흡된 체력을 다른 클라이언트와 동기화
                    curClient.CallSyncPlayerHPAll(curClient.curHealth);
                }

                int sendRPCBossHP = curHealth;

                // 보스 체력을 다른 클라이언트와 동기화
                photonView.RPC("SyncBossHealth", RpcTarget.Others, sendRPCBossHP);

                StartCoroutine("OnDamage");

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
        if(stream.IsWriting)
        {
            // We own this player: send the others our data
        }
        else
        {
            // Network player, receive data
        }
    }
}
