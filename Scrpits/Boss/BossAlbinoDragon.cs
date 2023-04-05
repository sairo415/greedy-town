using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class BossAlbinoDragon : MonoBehaviourPunCallbacks, IPunObservable
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

    // 박치기 공격 진행중. 공격을 하는 동안 다른 공격을 하지 않도록 제한함.
    bool isHornAttackING;

    // 공격 대상 인식 범위
    public GameObject targetOnSpot;     // 박치기 영역에 들어옴
    public GameObject jumpTargetOnSpot; // 점프 공격 영역에 들어옴

    // 공격 지점
    public Transform hornPos;
    public Transform jumpPos;
    public Transform windPos1;
    public Transform windPos2;
    public Transform windPos3;
    public Transform windPos4;
    public Transform windPos5;

    // 달리기 시간
    float runTimeDelta; // 측정
    float runTime;      // 기준치

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

        // 달리는 시간
        runTime = 10.0f;

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
            gameManager.bossAlbino = gameObject.GetComponent<BossAlbinoDragon>();
        }

        // 포효
        StartCoroutine("DoScream");
    }

    void Update()
    {
        // 포효가 종료되면 보스 공격 시작됨
        if(!isSreamEnd)
            return;

        // 마스터 클라이언트 안에 보스의 모든 행동 결정 권한이 있음
        if(photonView.IsMine)
        {
            // 죽음
            if(!isDie && curHealth <= 0)
            {
                // 모든 오브젝트가 통과할 수 있는 레이어
                gameObject.layer = 11;
                isDie = true;

                // 추적 중단
                nav.isStopped = true;
                FreezeVelocity();
                isChase = false;
                anim.SetBool("isRun", false);

                // 애니메이션
                anim.SetTrigger("doDie");

                // 일정 시간 뒤 파괴
                Destroy(gameObject, 4);
            }

            // 죽었으면 이후 로직을 실행할 필요 없음
            if(isDie)
                return;

            // 타겟 변경 시간 측정
            changeTargetTimeDelta += Time.deltaTime;

            // 살아있는 플레이어 중 타겟을 랜덤 선택
            if(changeTargetTimeDelta >= changeTargetTime && !isAttackING)
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
                targetPlayerTransform = targetPlayer.transform;

                changeTargetTimeDelta = 0.0f;
            }

            // 공격 패턴 랜덤 선택
            selectedSkillIdx = Random.Range(0, 3);

            // 플레이어가 인식된다면
            if(targetPlayerTransform != null)
            {
                // 달리는 시간 측정
                runTimeDelta += Time.deltaTime;

                if(isAttackING)
                    return;

                if(isHornAttackING)
                    selectedSkillIdx = 0;

                // 박치기 패턴
                if(selectedSkillIdx == 0)
                {
                    // 이 패턴이 시작되면 다른 패턴으로 전환하지 않도록 제한함.
                    isHornAttackING = true;

                    // 달리기
                    nav.isStopped = false;
                    isChase = true;
                    anim.SetBool("isRun", true);
                    nav.SetDestination(targetPlayerTransform.position);

                    // 일정 시간동안 달린 후 박치기, 또는 도중에 플레이어를 만나면 박치기
                    runTimeDelta += Time.deltaTime;
                    if(runTimeDelta >= runTime || targetOnSpot.GetComponent<BossTarget>().isTargetOn)
                    {
                        // 추적 중단
                        nav.isStopped = true;
                        FreezeVelocity();
                        isChase = false;
                        anim.SetBool("isRun", false);

                        // 공격
                        isAttackING = true;
                        anim.SetTrigger("doAttack2");
                        StartCoroutine("DoHornAttack");
                    }
                }
                // 회오리 패턴
                else if(selectedSkillIdx == 1)
                {
                    // 추적 중단
                    nav.isStopped = true;
                    FreezeVelocity();
                    isChase = false;
                    anim.SetBool("isRun", false);

                    transform.LookAt(targetPlayerTransform);

                    // 공격
                    isAttackING = true;
                    anim.SetTrigger("doAttack");
                    StartCoroutine("DoWindAttack");
                }
                // 점프 패턴
                else if(selectedSkillIdx == 2)
                {
                    // 점프 피격 범위 내에 플레이어 있을 경우 점프 공격
                    if(jumpTargetOnSpot.GetComponent<BossAlbinoJumpTarget>().isTargetOn)
                    {
                        // 추적 중단
                        nav.isStopped = true;
                        FreezeVelocity();
                        isChase = false;
                        anim.SetBool("isRun", false);

                        // 공격
                        isAttackING = true;
                        anim.SetTrigger("doJump");
                        StartCoroutine("DoJumpAttack");
                    }
                }
            }
        }
    }

    // 스폰되면 포효
    IEnumerator DoScream()
    {
        yield return new WaitForSeconds(1.0f);

        anim.SetTrigger("doScream");
        skillSound.clip = dragonSreamSound;
        skillSound.loop = false;
        skillSound.Play();

        yield return new WaitForSeconds(4.0f);

        isSreamEnd = true;

        yield return null;
    }

    // 박치기 공격
    IEnumerator DoHornAttack()
    {
        // 실제 공격 발동 전 애니메이션 시간 동안 대기
        yield return new WaitForSeconds(1.0f);

        // 이펙트
        GameObject skillAreaObj = PhotonNetwork.Instantiate("OneHandSmash", hornPos.position, hornPos.rotation);

		// 공격 지속 시간
		yield return new WaitForSeconds(1.0f);

        PhotonNetwork.Destroy(skillAreaObj);

        // 공격 후 대기
        yield return new WaitForSeconds(2.0f);

        isAttackING = false;
        isHornAttackING = false;
        runTimeDelta = 0.0f;

        yield return null;
    }

    // 바람 공격
    IEnumerator DoWindAttack()
    {
        yield return new WaitForSeconds(1.0f);

        GameObject skillAreaObj1 = PhotonNetwork.Instantiate("StormTornado", windPos1.position, windPos1.rotation);
        GameObject skillAreaObj2 = PhotonNetwork.Instantiate("StormTornado", windPos2.position, windPos2.rotation);
        GameObject skillAreaObj3 = PhotonNetwork.Instantiate("StormTornado", windPos3.position, windPos3.rotation);
        GameObject skillAreaObj4 = PhotonNetwork.Instantiate("StormTornado", windPos4.position, windPos4.rotation);
        GameObject skillAreaObj5 = PhotonNetwork.Instantiate("StormTornado", windPos5.position, windPos5.rotation);

        skillAreaObj1.SetActive(true);
        skillAreaObj2.SetActive(true);
        skillAreaObj3.SetActive(true);
        skillAreaObj4.SetActive(true);
        skillAreaObj5.SetActive(true);

        yield return new WaitForSeconds(0.6f);

        Rigidbody skillAreaRigid1 = skillAreaObj1.GetComponentInChildren<Rigidbody>();
        Rigidbody skillAreaRigid2 = skillAreaObj2.GetComponentInChildren<Rigidbody>();
        Rigidbody skillAreaRigid3 = skillAreaObj3.GetComponentInChildren<Rigidbody>();
        Rigidbody skillAreaRigid4 = skillAreaObj4.GetComponentInChildren<Rigidbody>();
        Rigidbody skillAreaRigid5 = skillAreaObj5.GetComponentInChildren<Rigidbody>();

        skillAreaRigid1.velocity = windPos1.forward * 50;
        skillAreaRigid2.velocity = windPos2.forward * 50;
        skillAreaRigid3.velocity = windPos3.forward * 50;
        skillAreaRigid4.velocity = windPos4.forward * 50;
        skillAreaRigid5.velocity = windPos5.forward * 50;

        yield return new WaitForSeconds(2.0f);

        isAttackING = false;

        yield return new WaitForSeconds(3.0f);

        skillAreaObj1.SetActive(false);
        skillAreaObj2.SetActive(false);
        skillAreaObj3.SetActive(false);
        skillAreaObj4.SetActive(false);
        skillAreaObj5.SetActive(false);

        PhotonNetwork.Destroy(skillAreaObj1);
        PhotonNetwork.Destroy(skillAreaObj2);
        PhotonNetwork.Destroy(skillAreaObj3);
        PhotonNetwork.Destroy(skillAreaObj4);
        PhotonNetwork.Destroy(skillAreaObj5);

        yield return null;
    }

    // 점프 공격
    IEnumerator DoJumpAttack()
	{
        yield return new WaitForSeconds(1.6f);

        GameObject skillAreaObj = PhotonNetwork.Instantiate("LumenJudgement", jumpPos.position, jumpPos.rotation);

        yield return new WaitForSeconds(1.0f);

        PhotonNetwork.Destroy(skillAreaObj);

        yield return new WaitForSeconds(2.0f);

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
                    PhotonNetwork.Destroy(other.gameObject);
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
