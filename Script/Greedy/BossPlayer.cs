using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.Rendering;


public class BossPlayer : MonoBehaviour
{
    //Game Manager
    private BossGameManager bossGameManager;

    // Player name
    public string bossPlayerName;

    // Camera
    private CinemachineVirtualCamera virtualCamera;

    // Photon
    public PhotonView pv;

    // 이동 관련
    float hAxis;
    float vAxis;

    // 키입력
    bool sDown; // 회피 : space
    bool qDown; // 기본 공격
    bool wDown; // 스킬 1
    bool eDown; // 스킬 2
    bool rDown; // 스킬 3

    // 체력
    public int maxHealth;
    public int curHealth;

    // 방어력
    public int defense;

    // 속도
    public float speed;

    // 1단계 스킬
    public enum LvOneSkill{NULL, SwordForce, Vampirism, Intent};
    public LvOneSkill lvOneSkill;

    // 2 단계 스킬
    public enum LvTwoSkill{NULL, SwordDance, BloodExplosion,Blessing};
    public LvTwoSkill lvTwoSkill;

    // 3 단계 스킬
    public enum LvThreeSkill {NULL, SwordBlade, RestrictionOfBlood, Resurrection};
    public LvThreeSkill lvThreeSkill;

    // 특정 스킬 사용하면서 다른 스킬을 사용하지 못하게 하고 싶은 경우 (일반 공격 포함)
    // 스킬 시전 동안 false 로 변경하여 다른 스킬을 사용 못하게 함.
    bool isSkillReady;
    // 특정 스킬 사용하면서 움직임을 제한하고 싶은 경우
    // 스킬 시전 동안 false 로 변경하여 움직임을 제한.
    bool isMoveReady;

    // 공통 일반 공격
    public GameObject normalAttack;
    public AudioClip swingSound;
    public Transform swordSwingPos;

    float qSkillDelay;
    public float qSkillRate;
    bool isQSkillReady;

    // SwordMaster
    public GameObject swordMasterAttack;
    
    public Transform swordForcePos;
    public AudioClip swordForceSound;
    public GameObject swordForce;

    public Transform swordDancePos;
    public AudioClip swordDanceSound;
    public GameObject swordDance;

    public Transform swordFlashPos;
    public AudioClip swordFlashSound;
    public GameObject swordFlash;

    public float swordForceRate;        // 스킬 재사용 대기 시간
    public float swordForcePlayTime;    // 스킬 오브젝트 유지 시간

    public float swordDanceRate;
    public float swordDancePlayTime;

    public float swordFlashRate;
    public float swordFlashPlayTime;

    // Berserker
    public GameObject berserkerAttack;

    //public Transform vampirismPos;
    //public GameObject vampirism;

    public Transform bloodExplosionPos;
    public AudioClip bloodExplosionSound;
    public GameObject bloodExplosion;

    public Transform restrictionOfBloodPos;
    public AudioClip restrictionOfBloodSound;
    public GameObject restrictionOfBlood;

    //public float vampirismRate;
    //public float vampirismPlayTime;

    public float bloodExplosionRate;
    public float bloodExplosionPlayTime;

    public float restrictionOfBloodRate;
    public float restrictionOfBloodPlayTime;

    // Paladin
    public GameObject paladinAttack;

    //public Transform intentPos;
    //public GameObject intent;

    public Transform blessingPos;
    public AudioClip blessingSound;
    public GameObject blessing;

    public Transform resurrectionPos;
    public AudioClip resurrectionSound;
    public GameObject resurrection;

    //public float intentRate;
    //public float intentPlayTime;

    public float blessingRate;
    public float blessingPlayTime;

    public float resurrectionRate;
    public float resurrectionPlayTime;

    // w 공격 딜레이
    float wSkillDelay;
    public float wSkillRate;
    bool isWSkillReady;

    // e 공격 딜레이
    float eSkillDelay;
    public float eSkillRate;
    bool isESkillReady;

    // r 공격 딜레이
    float rSkillDelay;
    public float rSkillRate;
    bool isRSkillReady;

    // 구르기 사운드
    public AudioClip dodgeSound;

    // 구르기 쿨타임
    float dodgeDelay;
    public float dodgeRate;
    bool isDodgeReady;

    // 회피 여부
    bool isDodge;

    // 벽 충돌 여부
    bool isBorder;

    Vector3 moveVec;    // 움직일 벡터
    Vector3 dodgeVec;   // 회피 도중 방향 조작 못하도록

    Rigidbody rigid;
    Animator anim;

    // 씬 이동 관련
    int nextSceneIndex;

    // 스킬 음
    AudioSource skillSound;

    // 패시브 스킬 여부
    bool isQPassive = false;
    bool isWPassive = false;
    bool isEPassive = false;
    bool isRPassive = false;

    // 흡혈 패시브 여부
    public bool isVampirism = false;

    // 용암 위에 있을 경우
    float damageTimer = 0.0f;
    public int damageAmount = 10;
    public float damageInterval = 1.0f;

    bool inLava = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        skillSound = GetComponent<AudioSource>();

        // 체력 초기화
        curHealth = maxHealth;

        isSkillReady = true;
        isMoveReady = true;

        qSkillDelay = 1000f;
        wSkillDelay = 1000f;
        eSkillDelay = 1000f;
        rSkillDelay = 1000f;
        dodgeDelay = 1000f;
    }

    void Start()
    {
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        BossMemberManager bossMemberManager = FindObjectOfType<BossMemberManager>();

        if(pv.IsMine)
        {
            // 자신의 캐릭터일 경우 시네머신 카메라를 연결
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;

            bossMemberManager.playerInfoList.Add(pv.ViewID, (bossPlayerName, (maxHealth, curHealth)));
            bossMemberManager.UpdateUI();
        }
        else
        {
            int requestID = pv.ViewID;
            pv.RPC("RequestOtherPlayerName", RpcTarget.Others, requestID, null);
        }
    }

    [PunRPC]
    void RequestOtherPlayerName(int requestID, PhotonMessageInfo info)
    {
        BossPlayer newBossPlayer = PhotonView.Find(requestID).gameObject.GetComponent<BossPlayer>();
        string responseName = newBossPlayer.bossPlayerName;
        
        pv.RPC("ResponseOtherPlayerName", RpcTarget.Others, requestID, responseName);

        //Debug.Log("RequestOtherPlayerName 함수");
        //Debug.Log("요청 아이디 : " + requestID);
        //Debug.Log("응답 이름 : " + responseName);
    }

    [PunRPC]
    void ResponseOtherPlayerName(int requestID, string responseName)
    {
        //Debug.Log("ResponseOtherPlayerName 함수");
        //Debug.Log("요청 아이디 : " + requestID);
        //Debug.Log("응답 이름 : " + responseName);
        
        PhotonView.Find(requestID).gameObject.GetComponent<BossPlayer>().bossPlayerName = responseName;

        BossMemberManager bossMemberManager = FindObjectOfType<BossMemberManager>();
        bossMemberManager.playerInfoList.Add(requestID, (responseName, (maxHealth, curHealth)));
        bossMemberManager.UpdateUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == nextSceneIndex)
        {
            pv = GetComponent<PhotonView>();
            virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

            if(pv.IsMine)
            {
                virtualCamera.Follow = transform;
                virtualCamera.LookAt = transform;

                BossGameManager bossGameManager = FindObjectOfType<BossGameManager>();
                bossGameManager.player = gameObject.GetComponent<BossPlayer>();

                // 패시브 효과 적용
                int bossPlayerViewID = pv.ViewID;
                GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;

                if(bossGameManager.stage == 2)
                {
                    if(bossPlayerObj.GetComponent<BossPlayer>().lvOneSkill == LvOneSkill.Vampirism)
                    {
                        isVampirism = true;
                        isWPassive = true;
                    }
                    else if(bossPlayerObj.GetComponent<BossPlayer>().lvOneSkill == LvOneSkill.Intent)
                    {
                        defense += 20;
                        isWPassive = true;
                    }
                }
            }

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, 0, 0);

            curHealth = maxHealth;

            isSkillReady = true;
            isMoveReady = true;

            qSkillDelay = 1000f;
            wSkillDelay = 1000f;
            eSkillDelay = 1000f;
            rSkillDelay = 1000f;
            dodgeDelay = 1000f;

            nextSceneIndex++;
        }
    }

    void Update()
    {
        if(pv.IsMine)
        {
            //입력
            GetInput();

            //움직임
            Move();
            Turn();

            //회피
            Dodge();

            //공격
            QSkill();
            WSkill();
            ESkill();
            RSkill();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        BossMemberManager bossMemberManager = FindObjectOfType<BossMemberManager>();

        if(bossMemberManager == null)
            return;

        if(pv == null)
            return;

        bossMemberManager.playerInfoList.Remove(pv.ViewID);
        bossMemberManager.UpdateUI();
    }

    void GetInput()
    {
        // GetAxisRaw() : Axis 값을 정수로 변환
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        // space 를 누르는 그 순간만 회피하도록 GetButtonDown 사용
        sDown = Input.GetButtonDown("Jump");
        // 기본 공격 및 스킬 입력
        qDown = Input.GetButtonDown("Skill1");
        wDown = Input.GetButtonDown("Skill2");
        eDown = Input.GetButtonDown("Skill3");
        rDown = Input.GetButtonDown("Skill4");
    }

    void Move()
    {
        if(!isMoveReady)
        {
            moveVec = Vector3.zero;
            return;
        }

        // normalized
        // 대각선이라고 속도 빨라지지 않게 방향 값이 1로 보정된 벡터로
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 회피를 하고 있다면 회피하는 방향으로 치환
        if(isDodge)
            moveVec = dodgeVec;

        // 벽을 뚫고 못지나가게
        if(!isBorder)
            transform.position += moveVec * speed * Time.deltaTime;

        // 애니메이션
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        // 플레이어 회전 (움직이는 방향으로 바라본다)
        transform.LookAt(transform.position + moveVec);
    }

    void Dodge()
    {
        if(!isMoveReady)
            return;

        dodgeDelay += Time.deltaTime;
        isDodgeReady = dodgeRate < dodgeDelay;

        // 벽을 뚫고 못지나가게
        if(sDown && moveVec != Vector3.zero && !isDodge && !isBorder && isDodgeReady)
        {
            // 움직임 벡터 -> 회피방향 벡터로 바뀌도록 구현
            dodgeVec = moveVec;
            speed *= 2.0f;

            skillSound.clip = dodgeSound;
            skillSound.Play();
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 시간차 함수 호출 0.5 초
            Invoke("DodgeOut", 0.5f);

            // 딜레이 초기화
            dodgeDelay = 0;
        }
    }

    void QSkill()
    {
        if(isQPassive)
            return;

        if(!isSkillReady)
            return;

        qSkillDelay += Time.deltaTime;
        isQSkillReady = qSkillRate < qSkillDelay;

        if(qDown && isQSkillReady && !isDodge)
        {
            int bossPlayerViewID = pv.ViewID;

            // 스킬 시전
            pv.RPC("QSkillStart", RpcTarget.All, bossPlayerViewID);

            // 애니메이션
            anim.SetTrigger("doSwing1");

            // 딜레이 초기화
            qSkillDelay = 0;
        }
    }

    void WSkill()
    {
        if(isWPassive)
            return;

        if(!isSkillReady)
            return;

        if(lvOneSkill == LvOneSkill.NULL)
            return;

        switch(lvOneSkill)
        {
        case LvOneSkill.SwordForce:
            wSkillRate = swordForceRate;
            break;
        }

        wSkillDelay += Time.deltaTime;
        isWSkillReady = wSkillRate < wSkillDelay;

        if(wDown && isWSkillReady && !isDodge)
        {
            int bossPlayerViewID = pv.ViewID;

            // 스킬 시전
            pv.RPC("WSkillStart", RpcTarget.All, bossPlayerViewID);

            // 애니메이션 -> 클래스 별로 다른 애니메이션으로
            switch(lvOneSkill)
            {
            case LvOneSkill.SwordForce:
                anim.SetTrigger("doSwing2");
                break;
            } 

            // 딜레이 초기화
            wSkillDelay = 0;
        }
    }

    void ESkill()
    {
        if(isEPassive)
            return;

        if(!isSkillReady)
            return;

        if(lvTwoSkill == LvTwoSkill.NULL)
            return;

        switch(lvTwoSkill)
        {
        case LvTwoSkill.SwordDance:
            eSkillRate = swordDanceRate;
            break;
        case LvTwoSkill.BloodExplosion:
            eSkillRate = bloodExplosionRate;
            break;
        case LvTwoSkill.Blessing:
            eSkillRate = blessingRate;
            break;
        }

        eSkillDelay += Time.deltaTime;
        isESkillReady = eSkillRate < eSkillDelay;

        if(eDown && isESkillReady && !isDodge)
        {
            int bossPlayerViewID = pv.ViewID;

            // 스킬 시전
            pv.RPC("ESkillStart", RpcTarget.All, bossPlayerViewID);

            // 애니메이션 -> 클래스 별로 다른 애니메이션으로
            switch(lvTwoSkill)
            {
            case LvTwoSkill.SwordDance:
                anim.SetTrigger("doSpell");
                break;
            case LvTwoSkill.BloodExplosion:
                anim.SetTrigger("doSwing2");
                break;
            case LvTwoSkill.Blessing:
                anim.SetTrigger("doSpell");
                break;
            }

            // 딜레이 초기화
            eSkillDelay = 0;
        }
    }

    void RSkill()
    {
        if(isRPassive)
            return;

        if(!isSkillReady)
            return;

        if(lvThreeSkill == LvThreeSkill.NULL)
            return;

        switch(lvThreeSkill)
        {
        case LvThreeSkill.SwordBlade:
            rSkillRate = swordFlashRate;
            break;
        case LvThreeSkill.RestrictionOfBlood:
            rSkillRate = restrictionOfBloodRate;
            break;
        case LvThreeSkill.Resurrection:
            rSkillRate = resurrectionRate;
            break;
        }

        rSkillDelay += Time.deltaTime;
        isRSkillReady = rSkillRate < rSkillDelay;

        if(rDown && isRSkillReady && !isDodge)
        {
            int bossPlayerViewID = pv.ViewID;

            // 스킬 시전
            pv.RPC("RSkillStart", RpcTarget.All, bossPlayerViewID);

            // 애니메이션 -> 클래스 별로 다른 애니메이션으로
            switch(lvThreeSkill)
            {
            case LvThreeSkill.RestrictionOfBlood:
                anim.SetTrigger("doSpell");
                break;
            case LvThreeSkill.Resurrection:
                anim.SetTrigger("doSpell");
                break;
            }

            // 딜레이 초기화
            rSkillDelay = 0;
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    [PunRPC]
    IEnumerator QSkillStart(int bossPlayerViewID)
    {
        GameObject skillAreaObj = null;

        GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;


        switch(bossPlayerObj.GetComponent<BossPlayer>().lvOneSkill)
        {
        case LvOneSkill.NULL:
            skillAreaObj = Instantiate(normalAttack, swordSwingPos.transform.position, swordSwingPos.transform.rotation);
            break;
        case LvOneSkill.SwordForce:
            skillAreaObj = Instantiate(swordMasterAttack, swordSwingPos.transform.position, swordSwingPos.transform.rotation);
            break;
        case LvOneSkill.Vampirism:
            skillAreaObj = Instantiate(berserkerAttack, swordSwingPos.transform.position, swordSwingPos.transform.rotation);
            break;
        case LvOneSkill.Intent:
            skillAreaObj = Instantiate(paladinAttack, swordSwingPos.transform.position, swordSwingPos.transform.rotation);
            break;
        }

        if(skillAreaObj == null)
            yield break;

        skillAreaObj.GetComponent<BossPlayerSkill>().SetID(bossPlayerViewID);
        skillAreaObj.SetActive(true);
        skillSound.clip = swingSound;
        skillSound.Play();

        yield return new WaitForSeconds(1.0f);

        skillAreaObj.SetActive(false);
        Destroy(skillAreaObj);
    }

    [PunRPC]
    IEnumerator WSkillStart(int bossPlayerViewID)
    {
        GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;

        if(bossPlayerObj.GetComponent<BossPlayer>().lvOneSkill == LvOneSkill.SwordForce)
        {
            GameObject skillAreaObj = Instantiate(swordForce, swordForcePos.position, swordForcePos.rotation);

            skillAreaObj.GetComponent<BossPlayerSkill>().SetID(bossPlayerViewID);
            skillAreaObj.SetActive(true);
            skillSound.clip = swordForceSound;
            skillSound.Play();

            // 이동하는 공격
            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = swordForcePos.forward * 50;

            yield return new WaitForSeconds(swordForcePlayTime);

            if(skillAreaObj == null)
                yield break;

            skillAreaObj.SetActive(false);
            Destroy(skillAreaObj);
        }
    }

    [PunRPC]
    IEnumerator ESkillStart()
    {
        // 이거 설치기 지속 공격으로 바꿔야되고 공격에 ID 부여해야됨.
        if(lvTwoSkill == LvTwoSkill.SwordDance)
        {
            GameObject skillAreaObj = Instantiate(swordDance, swordDancePos.position, swordDancePos.rotation);

            skillAreaObj.SetActive(true);
            skillSound.clip = swordDanceSound;
            skillSound.Play();

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = swordDancePos.forward;

            yield return new WaitForSeconds(swordDancePlayTime);

            if(skillAreaObj != null)
            {
                skillAreaObj.SetActive(false);
                Destroy(skillAreaObj);
            }
        }
        else if(lvTwoSkill == LvTwoSkill.BloodExplosion)
        {

        }
        else if(lvTwoSkill == LvTwoSkill.Blessing)
        {

        }
    }

    [PunRPC]
    IEnumerator RSkillStart()
    {
        if(lvThreeSkill == LvThreeSkill.SwordBlade)
        {
            GameObject skillAreaObj = Instantiate(swordFlash, swordFlashPos.position, swordFlashPos.rotation);

            skillAreaObj.SetActive(true);
            skillSound.clip = swordFlashSound;
            skillSound.Play();

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = swordFlashPos.forward;

            Vector3 originalScale = gameObject.transform.localScale;
            gameObject.transform.localScale = Vector3.zero;

            isMoveReady = false;
            isSkillReady = false;

            yield return new WaitForSeconds(swordFlashPlayTime);

            gameObject.transform.localScale = originalScale;
            isMoveReady = true;
            isSkillReady = true;

            if(skillAreaObj != null)
            {
                skillAreaObj.SetActive(false);
                Destroy(skillAreaObj);
            }
        }
        else if(lvThreeSkill == LvThreeSkill.RestrictionOfBlood)
        {

        }
        else if(lvThreeSkill == LvThreeSkill.Resurrection)
        {

        }
    }

	// 플레이어 데미지 입음
	void OnTriggerEnter(Collider other)
	{
        if(other.CompareTag("DamageObject"))
        {
            inLava = true;

            // 플레이어 불타는 이펙트 활성화
            transform.Find("Skill/Fire").gameObject.SetActive(true);
  
            BossGameManager bossGameManager = GameObject.FindObjectOfType<BossGameManager>();

            // 내 클라이언트의 경우만 로직 실행
            if(pv.IsMine)
            {
                bossGameManager.dangerPanel.SetActive(true);
            }

            //int damage = other.GetComponent<BossPlayerSkill>().damage;
            //curHealth -= damageAmount;
            //int sendRPCBossPlayerHP = curHealth;

            //if(curHealth < 0)
            //    curHealth = 0;

            //pv.RPC("SyncPlayerHP", RpcTarget.Others, sendRPCBossPlayerHP);
        }
		else if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
		{
            int skillOwner = other.GetComponent<BossPlayerSkill>().GetID();

            // 맞은 스킬의 시전자가 본인일 경우 데미지를 입지않도록 함.
            if(pv.ViewID == skillOwner)
                return;

            // 내 클라이언트의 경우만 로직 실행
            // 상대 클라이언트에서 내 플레이어가 데미지를 입는 로직은 실행 안함.
            //if(!pv.IsMine)
            //    return;

            int damage = other.GetComponent<BossPlayerSkill>().damage;
			curHealth -= damage;
            int sendRPCBossPlayerHP = curHealth;

            if(curHealth < 0)
			    curHealth = 0;

			if(other.tag == "PlayerAttack")
			{
				Destroy(other.gameObject);
				other.gameObject.SetActive(false);
			}

			//StartCoroutine("OnDamage");
			pv.RPC("SyncPlayerHP", RpcTarget.Others, sendRPCBossPlayerHP);
		}

        if(curHealth <= 0)
        {
            // 사망
            anim.SetTrigger("doDie");
            isSkillReady = false;
            isMoveReady = false;
        }
    }

	void OnTriggerExit(Collider other)
	{
        if(other.CompareTag("DamageObject"))
        {
            inLava = false;

            // 플레이어 불타는 이펙트 비활성화
            transform.Find("Skill/Fire").gameObject.SetActive(false);

            BossGameManager bossGameManager = GameObject.FindObjectOfType<BossGameManager>();

            if(pv.IsMine)
            {
                damageTimer = 0.0f;
                bossGameManager.dangerPanel.SetActive(false);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("DamageObject") && inLava && pv.IsMine)
        {
            damageTimer += Time.deltaTime;

            if(damageTimer >= damageInterval)
            {
                curHealth -= damageAmount;
                damageTimer = 0.0f;
            }

            pv.RPC("SyncPlayerHP", RpcTarget.All, curHealth);
        }
    }

    [PunRPC]
    void OnDamage(int sendRPCBossPlayerHP)
    {
        curHealth = sendRPCBossPlayerHP;
    }

    public void CallSyncPlayerHPAll(int sendRPCBossPlayerHP)
    {
        pv.RPC("SyncPlayerHP", RpcTarget.All, sendRPCBossPlayerHP);
    }

    public void CallSyncPlayerHPOther(int sendRPCBossPlayerHP)
    {
        pv.RPC("SyncPlayerHP", RpcTarget.Others, sendRPCBossPlayerHP);
    }

    [PunRPC]
    void SyncPlayerHP(int sendRPCBossPlayerHP)
    {
        curHealth = sendRPCBossPlayerHP;
        BossMemberManager memberManager = GameObject.FindObjectOfType<BossMemberManager>();
        memberManager.UpdatePlayerInfoList();
        memberManager.UpdateUI();
    }


    //자동 회전 방지
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        // Raycast() : Ray 를 쏘아 닿는 오브젝트를 감지하는 함수
        isBorder = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        if(pv.IsMine)
        {
            FreezeRotation();
            StopToWall();
        }
    }
}
