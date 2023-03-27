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

    // Camera
    private CinemachineVirtualCamera virtualCamera;

    // Photon
    private PhotonView pv;

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

    // 속도
    public float speed;

    // 1단계 스킬
    public enum LvOneSkill{NULL, SwordForce, Vampirism, Intent};
    public LvOneSkill lvOneSkill;

    // 2 단계 스킬
    public enum LvTwoSkill{NULL, SwordDance, BloodExplosion,Blessing};
    public LvTwoSkill lvTwoSkill;

    // 3 단계 스킬
    public enum LvThreeSkill {NULL, SwordFlash, RestrictionOfBlood, Resurrection};
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

    float normalAtkDelay;
    public float normalAtkRate;
    bool isNormalAtkReady;

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

    public Transform vampirismPos;
    public GameObject vampirism;

    public Transform bloodExplosionPos;
    public AudioClip bloodExplosionSound;
    public GameObject bloodExplosion;

    public Transform restrictionOfBloodPos;
    public AudioClip restrictionOfBloodSound;
    public GameObject restrictionOfBlood;

    public float vampirismRate;
    public float vampirismPlayTime;

    public float bloodExplosionRate;
    public float bloodExplosionPlayTime;

    public float restrictionOfBloodRate;
    public float restrictionOfBloodPlayTime;

    // Paladin
    public GameObject paladinkerAttack;

    public Transform intentPos;
    public GameObject intent;

    public Transform blessingPos;
    public AudioClip blessingSound;
    public GameObject blessing;

    public Transform resurrectionPos;
    public AudioClip resurrectionSound;
    public GameObject resurrection;

    public float intentRate;
    public float intentPlayTime;

    public float blessingRate;
    public float blessingPlayTime;

    public float resurrectionRate;
    public float resurrectionPlayTime;

    // w 공격 딜레이
    float wSkillDelay;
    float wSkillRate;
    bool isWSkillReady;

    // e 공격 딜레이
    float eSkillDelay;
    float eSkillRate;
    bool isESkillReady;

    // r 공격 딜레이
    float rSkillDelay;
    float rSkillRate;
    bool isRSkillReady;

    // 구르기 사운드
    public AudioClip dodgeSound;

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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        skillSound = GetComponent<AudioSource>();
        bossGameManager = GetComponent<BossGameManager>();

        // 체력 초기화
        curHealth = maxHealth;

        isSkillReady = true;
        isMoveReady = true;

        normalAtkDelay = 1000f;
        wSkillDelay = 1000f;
        eSkillDelay = 1000f;
        rSkillDelay = 1000f;
    }

    void Start()
    {
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        
        if(pv.IsMine)
        {
            // 자신의 캐릭터일 경우 시네머신 카메라를 연결
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;

            // 게임 메니져 세팅
            bossGameManager.player = gameObject.GetComponent<BossPlayer>();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == nextSceneIndex)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, 0, 0);

            curHealth = maxHealth;

            isSkillReady = true;
            isMoveReady = true;

            normalAtkDelay = 1000f;
            wSkillDelay = 1000f;
            eSkillDelay = 1000f;
            rSkillDelay = 1000f;

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
        // 벽을 뚫고 못지나가게
        if(sDown && moveVec != Vector3.zero && !isDodge && !isBorder)
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
        }
    }

    void QSkill()
    {
        if(!isSkillReady)
            return;

        normalAtkDelay += Time.deltaTime;
        isNormalAtkReady = normalAtkRate < normalAtkDelay;

        if(qDown && isNormalAtkReady && !isDodge)
        {
            // 스킬 시전
            pv.RPC("QSkillStart", RpcTarget.All);

            // 애니메이션
            anim.SetTrigger("doSwing1");

            // 딜레이 초기화
            normalAtkDelay = 0;
        }
    }

    void WSkill()
    {
        if(!isSkillReady)
            return;

        if(lvOneSkill == LvOneSkill.NULL)
            return;

        switch(lvOneSkill)
        {
        case LvOneSkill.SwordForce:
            wSkillRate = swordForceRate;
            break;
        case LvOneSkill.Vampirism:
            wSkillRate = vampirismRate;
            break;
        case LvOneSkill.Intent:
            wSkillRate = intentRate;
            break;
        }

        wSkillDelay += Time.deltaTime;
        isWSkillReady = wSkillRate < wSkillDelay;

        if(wDown && isWSkillReady && !isDodge)
        {
            // 스킬 시전
            pv.RPC("WSkillStart", RpcTarget.All);

            // 애니메이션 -> 클래스 별로 다른 애니메이션으로
            switch(lvOneSkill)
            {
            case LvOneSkill.SwordForce:
                anim.SetTrigger("doSwing2");
                break;
            case LvOneSkill.Vampirism:
                anim.SetTrigger("doSwing2");
                break;
            case LvOneSkill.Intent:
                anim.SetTrigger("doSwing2");
                break;
            } 

            // 딜레이 초기화
            wSkillDelay = 0;
        }
    }

    void ESkill()
    {
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
            // 스킬 시전
            pv.RPC("ESkillStart", RpcTarget.All);

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
                anim.SetTrigger("doSwing2");
                break;
            }

            // 딜레이 초기화
            eSkillDelay = 0;
        }
    }

    void RSkill()
    {
        if(!isSkillReady)
            return;

        if(lvThreeSkill == LvThreeSkill.NULL)
            return;

        switch(lvThreeSkill)
        {
        case LvThreeSkill.SwordFlash:
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
            // 스킬 시전
            pv.RPC("RSkillStart", RpcTarget.All);

            // 애니메이션 -> 클래스 별로 다른 애니메이션으로
            switch(lvThreeSkill)
            {
            case LvThreeSkill.SwordFlash:
                anim.SetTrigger("doSwing2");
                break;
            case LvThreeSkill.RestrictionOfBlood:
                anim.SetTrigger("doSwing2");
                break;
            case LvThreeSkill.Resurrection:
                anim.SetTrigger("doSwing2");
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
    IEnumerator QSkillStart()
    {
		GameObject skillAreaObj = null;

		switch(lvOneSkill)
		{
		case LvOneSkill.NULL:
			skillAreaObj = Instantiate(normalAttack, gameObject.transform.position, gameObject.transform.rotation);
			break;
		case LvOneSkill.SwordForce:
			skillAreaObj = Instantiate(swordMasterAttack, swordSwingPos.transform.position, swordSwingPos.transform.rotation);
			break;
		case LvOneSkill.Vampirism:
			skillAreaObj = Instantiate(berserkerAttack, swordSwingPos.transform.position, swordSwingPos.transform.rotation);
			break;
		case LvOneSkill.Intent:
			skillAreaObj = Instantiate(paladinkerAttack, swordSwingPos.transform.position, swordSwingPos.transform.rotation);
			break;
		}

		if(skillAreaObj == null)
			yield break;

		skillAreaObj.SetActive(true);
		skillSound.clip = swingSound;
		skillSound.Play();

		yield return new WaitForSeconds(1.0f);

		Destroy(skillAreaObj);
		skillAreaObj.SetActive(false);
	}

    [PunRPC]
    IEnumerator WSkillStart()
    {
        if(lvOneSkill == LvOneSkill.SwordForce)
        {
            GameObject skillAreaObj = Instantiate(swordForce, swordForcePos.position, swordForcePos.rotation);

            skillAreaObj.SetActive(true);
            skillSound.clip = swordForceSound;
            skillSound.Play();

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = swordForcePos.forward * 50;

            yield return new WaitForSeconds(swordForcePlayTime);

            if(skillAreaObj != null)
            {
                skillAreaObj.SetActive(false);
                Destroy(skillAreaObj);
            }
        }
        else if(lvOneSkill == LvOneSkill.Vampirism)
        {

        }
        else if(lvOneSkill == LvOneSkill.Intent)
        { 

        }
    }

    [PunRPC]
    IEnumerator ESkillStart()
    {
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
        if(lvThreeSkill == LvThreeSkill.SwordFlash)
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
            // 플레이어 불타는 이펙트 활성화
            transform.Find("Skill/Fire").gameObject.SetActive(true);
        }
        /*else if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
        {
            int damage = other.GetComponent<BossPlayerSkill>().damage;
            //curHealth -= other.GetComponent<BossPlayerSkill>().damage;
            //if(curHealth < 0)
            //    curHealth = 0;

            // 적과 닿았을 때 삭제되도록 Destroy() 호출
            if(other.tag == "PlayerAttack")
            {
                Destroy(other.gameObject);
                other.gameObject.SetActive(false);
            }

            //StartCoroutine("OnDamage");
            pv.RPC("OnDamage", RpcTarget.All, damage);

        }*/
    }

	void OnTriggerExit(Collider other)
	{
        if(other.CompareTag("DamageObject"))
        {
            // 플레이어 불타는 이펙트 비활성화
            transform.Find("Skill/Fire").gameObject.SetActive(false);
        }
    }

    [PunRPC]
    IEnumerator OnDamage(int damage)
    {
        curHealth -= damage;
        if(curHealth < 0)
            curHealth = 0;

        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            
        }
        else
        {
            //yield return new WaitForSeconds(10.0f);

            //int sceneNum = SceneManager.GetActiveScene().buildIndex + 1;

            anim.SetTrigger("doDie");
            isSkillReady = false;
            isMoveReady = false;
        }
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
