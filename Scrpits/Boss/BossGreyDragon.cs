using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using System.Linq;

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

    bool startFlying;
    bool isFlyAttackING;

    // 공격 지점
    public Transform iceBallPos;
    public Transform flyAttackPos;
    public Transform windPos1;
    public Transform windPos2;
    public Transform windPos3;
    public Transform windPos4;

    // 날아다니는 시간
    // float flyTimeDelta; // 측정
    // float flyTime;      // 기준치

    // 맵 장애물 시간
    float mapIceTimeDelta; // 측정
    float mapIceTime;      // 기준치

    // 랜덤 스킬 선택 인덱스
    int selectedSkillIdx;

    bool isBossHPLow;

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
        mapIceTimeDelta = 100.0f;
        mapIceTime = 5.0f;

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
        StartCoroutine("DoScream");
    }

    void Update()
    {
        // 포효가 종료되면 보스 공격 시작됨
        if(!isSreamEnd)
            return;

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
            // 0 : 아이스 볼
            // 1 : 회오리 소환
            // 2 : 날아다니며 공격 => 추적
            selectedSkillIdx = Random.Range(0, 3);

            if(targetPlayerTransform != null)
            {
                if(isFlyAttackING)
                {
                    // 비행
                    nav.isStopped = false;
                    isChase = true;
                    anim.SetBool("isFlyMove", true);
                    nav.SetDestination(targetPlayerTransform.position);
                }

                mapIceTimeDelta += Time.deltaTime;

                if(((float)curHealth <= (float)maxHealth * 0.3) && (mapIceTimeDelta >= mapIceTime))
                {
                    StartCoroutine("MakeIceField");
                    mapIceTimeDelta = 0.0f;
                }

                if(isAttackING)
                    return;

                if(selectedSkillIdx == 0)
                {
                    // 공격
                    isAttackING = true;
                    StartCoroutine("DoIceBall");
                }
                else if(selectedSkillIdx == 1)
                {
                    // 공격
                    isAttackING = true;
                    StartCoroutine("DoWindAttack");
                }
                else if(selectedSkillIdx == 2)
                {
                    isAttackING = true;
                    StartCoroutine("DoFlyingAttack");
                }
            }
        }
    }

    IEnumerator MakeIceField()
	{
		IceField iceFieldInfo = GameObject.FindObjectOfType<IceField>();
        GameObject[] iceFieldPosInfo = iceFieldInfo.iceFieldPos;
        
        int selectCount = Random.Range(30, 40);
        GameObject[] selectedPos = new GameObject[selectCount];

        for(int i = 0; i < selectCount; i++)
        {
            int index = Random.Range(0, iceFieldPosInfo.Length);
            selectedPos[i] = iceFieldPosInfo[index];
            iceFieldPosInfo = iceFieldPosInfo.Where((val, idx) => idx != index).ToArray();
        }

        GameObject[] iceEffectObj = new GameObject[selectedPos.Length];

        for(int i = 0; i < selectedPos.Length; i++)
        {
            iceEffectObj[i] = PhotonNetwork.Instantiate("IceField", selectedPos[i].transform.position, selectedPos[i].transform.rotation);
        }

        yield return new WaitForSeconds(1.5f);

        GameObject[] iceAttackObj = new GameObject[selectedPos.Length];

        for(int i = 0; i < selectedPos.Length; i++)
        {
            iceAttackObj[i] = PhotonNetwork.Instantiate("IceBlockFissure", selectedPos[i].transform.position, Quaternion.Euler(-90, 0, 0));
        }

        yield return new WaitForSeconds(1.5f);

        for(int i = 0; i < selectedPos.Length; i++)
        {
            PhotonNetwork.Destroy(iceEffectObj[i]);
        }

        yield return new WaitForSeconds(1.5f);

        for(int i = 0; i < selectedPos.Length; i++)
        {
            PhotonNetwork.Destroy(iceAttackObj[i]);
        }

        yield return null;
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

    // 아이스 브레스
    IEnumerator DoIceBall()
    {
        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj1 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid1 = iceBallObj1.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid1.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(0.3f);

        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj2 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid2 = iceBallObj2.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid2.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(0.3f);

        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj3 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid3 = iceBallObj3.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid3.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(0.3f);

        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj4 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid4 = iceBallObj4.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid4.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(0.3f);

        transform.LookAt(targetPlayerTransform);

        anim.SetTrigger("doIceBall");

        yield return new WaitForSeconds(0.5f);

        GameObject iceBallObj5 = PhotonNetwork.Instantiate("IceFatalWheel", iceBallPos.position, iceBallPos.rotation);
        Rigidbody iceBallObjRigid5 = iceBallObj5.GetComponentInChildren<Rigidbody>();
        iceBallObjRigid5.velocity = iceBallPos.forward * 50;

        yield return new WaitForSeconds(5.0f);

        isAttackING = false;

        PhotonNetwork.Destroy(iceBallObj1);
        PhotonNetwork.Destroy(iceBallObj2);
        PhotonNetwork.Destroy(iceBallObj3);
        PhotonNetwork.Destroy(iceBallObj4);
        PhotonNetwork.Destroy(iceBallObj5);

        yield return null;
    }

    IEnumerator DoWindAttack()
    {
        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doTakeOff");

        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doFly");

        yield return new WaitForSeconds(1.0f);

        transform.LookAt(targetPlayerTransform);

        GameObject windObj1 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj2 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj3 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj4 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        yield return new WaitForSeconds(1.0f);

        transform.LookAt(targetPlayerTransform);

        GameObject windObj5 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj6 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj7 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj8 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        yield return new WaitForSeconds(1.0f);

        transform.LookAt(targetPlayerTransform);

        GameObject windObj9 = PhotonNetwork.Instantiate("SteelStorm", windPos1.position, windPos1.rotation);
        GameObject windObj10 = PhotonNetwork.Instantiate("SteelStorm", windPos2.position, windPos2.rotation);
        GameObject windObj11 = PhotonNetwork.Instantiate("SteelStorm", windPos3.position, windPos3.rotation);
        GameObject windObj12 = PhotonNetwork.Instantiate("SteelStorm", windPos4.position, windPos4.rotation);

        yield return new WaitForSeconds(4.0f);

        // Null 예외처리
        PhotonNetwork.Destroy(windObj1);
        PhotonNetwork.Destroy(windObj1);
        PhotonNetwork.Destroy(windObj2);
        PhotonNetwork.Destroy(windObj3);
        PhotonNetwork.Destroy(windObj4);
        PhotonNetwork.Destroy(windObj5);
        PhotonNetwork.Destroy(windObj6);
        PhotonNetwork.Destroy(windObj7);
        PhotonNetwork.Destroy(windObj8);
        PhotonNetwork.Destroy(windObj9);
        PhotonNetwork.Destroy(windObj10);
        PhotonNetwork.Destroy(windObj11);
        PhotonNetwork.Destroy(windObj12);

        anim.SetTrigger("doIdle");
        isAttackING = false;

        yield return null;
    }

    IEnumerator DoFlyingAttack()
    {
        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doTakeOff");

        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doFly");

        yield return new WaitForSeconds(1.0f);

        anim.SetBool("isFlyMove", true);

        isFlyAttackING = true;

        //InvokeRepeating("CallIceField", 1.0f, 0.5f);
        GameObject[] iceFieldArray = new GameObject[25];
        GameObject[] iceAttackArray = new GameObject[25];
        for(int i = 0; i < 20; i++)
        {
            iceFieldArray[i] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
            //iceAttackArray[i] = PhotonNetwork.Instantiate("IceBlockCrash", flyAttackPos.position, flyAttackPos.rotation);
            StartCoroutine("CallIceAttack");

            yield return new WaitForSeconds(0.5f);
        }

        for(int i = 0; i < 20; i++)
        {
            PhotonNetwork.Destroy(iceFieldArray[i]);
        }
        
        yield return new WaitForSeconds(2.0f);

        // 비행 정지
        isFlyAttackING = false;
        nav.isStopped = true;
        FreezeVelocity();
        isChase = false;
        anim.SetBool("isFlyMove", false);

        yield return new WaitForSeconds(2.0f);

        anim.SetTrigger("doIdle");

        yield return new WaitForSeconds(5.0f);

        isAttackING = false;

        yield return null;
    }

    /*IEnumerator CallIceField()
    {
        GameObject[] iceFieldArray = new GameObject[20];

        iceFieldArray[0] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[1] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[2] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[3] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[4] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[5] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[6] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[0]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[7] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[1]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[8] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[2]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[9] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[3]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[10] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[4]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[11] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[5]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[12] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[6]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[13] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[7]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[14] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[8]);

        yield return new WaitForSeconds(0.5f);
        
        iceFieldArray[15] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[9]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[16] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[10]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[17] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[11]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[18] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[12]);

        yield return new WaitForSeconds(0.5f);

        iceFieldArray[19] = PhotonNetwork.Instantiate("IceField", flyAttackPos.position, flyAttackPos.rotation);
        StartCoroutine("CallIceAttack");
        PhotonNetwork.Destroy(iceFieldArray[13]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[14]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[15]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[16]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[17]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[18]);

        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(iceFieldArray[19]);

        yield return null;
    }*/

    IEnumerator CallIceAttack()
    {
        GameObject iceAttack = PhotonNetwork.Instantiate("IceBlockCrash", flyAttackPos.position, flyAttackPos.rotation);

        yield return new WaitForSeconds(3.0f);

        PhotonNetwork.Destroy(iceAttack);

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
