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

    // �̵� ����
    float hAxis;
    float vAxis;

    // Ű�Է�
    bool sDown; // ȸ�� : space
    bool qDown; // �⺻ ����
    bool wDown; // ��ų 1
    bool eDown; // ��ų 2
    bool rDown; // ��ų 3

    // ü��
    public int maxHealth;
    public int curHealth;

    // ����
    public int defense;

    // �ӵ�
    public float speed;

    // 1�ܰ� ��ų
    public enum LvOneSkill{NULL, SwordForce, Vampirism, Intent};
    public LvOneSkill lvOneSkill;

    // 2 �ܰ� ��ų
    public enum LvTwoSkill{NULL, SwordDance, BloodExplosion,Blessing};
    public LvTwoSkill lvTwoSkill;

    // 3 �ܰ� ��ų
    public enum LvThreeSkill {NULL, SwordBlade, RestrictionOfBlood, Resurrection};
    public LvThreeSkill lvThreeSkill;

    // Ư�� ��ų ����ϸ鼭 �ٸ� ��ų�� ������� ���ϰ� �ϰ� ���� ��� (�Ϲ� ���� ����)
    // ��ų ���� ���� false �� �����Ͽ� �ٸ� ��ų�� ��� ���ϰ� ��.
    bool isSkillReady;
    // Ư�� ��ų ����ϸ鼭 �������� �����ϰ� ���� ���
    // ��ų ���� ���� false �� �����Ͽ� �������� ����.
    bool isMoveReady;

    // ���� �Ϲ� ����
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

    public float swordForceRate;        // ��ų ���� ��� �ð�
    public float swordForcePlayTime;    // ��ų ������Ʈ ���� �ð�

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

    // w ���� ������
    float wSkillDelay;
    public float wSkillRate;
    bool isWSkillReady;

    // e ���� ������
    float eSkillDelay;
    public float eSkillRate;
    bool isESkillReady;

    // r ���� ������
    float rSkillDelay;
    public float rSkillRate;
    bool isRSkillReady;

    // ������ ����
    public AudioClip dodgeSound;

    // ������ ��Ÿ��
    float dodgeDelay;
    public float dodgeRate;
    bool isDodgeReady;

    // ȸ�� ����
    bool isDodge;

    // �� �浹 ����
    bool isBorder;

    Vector3 moveVec;    // ������ ����
    Vector3 dodgeVec;   // ȸ�� ���� ���� ���� ���ϵ���

    Rigidbody rigid;
    Animator anim;

    // �� �̵� ����
    int nextSceneIndex;

    // ��ų ��
    AudioSource skillSound;

    // �нú� ��ų ����
    bool isQPassive = false;
    bool isWPassive = false;
    bool isEPassive = false;
    bool isRPassive = false;

    // ���� �нú� ����
    public bool isVampirism = false;

    // ��� ���� ���� ���
    float damageTimer = 0.0f;
    public int damageAmount = 10;
    public float damageInterval = 1.0f;

    bool inLava = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        skillSound = GetComponent<AudioSource>();

        // ü�� �ʱ�ȭ
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
            // �ڽ��� ĳ������ ��� �ó׸ӽ� ī�޶� ����
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

        //Debug.Log("RequestOtherPlayerName �Լ�");
        //Debug.Log("��û ���̵� : " + requestID);
        //Debug.Log("���� �̸� : " + responseName);
    }

    [PunRPC]
    void ResponseOtherPlayerName(int requestID, string responseName)
    {
        //Debug.Log("ResponseOtherPlayerName �Լ�");
        //Debug.Log("��û ���̵� : " + requestID);
        //Debug.Log("���� �̸� : " + responseName);
        
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

                // �нú� ȿ�� ����
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
            //�Է�
            GetInput();

            //������
            Move();
            Turn();

            //ȸ��
            Dodge();

            //����
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
        // GetAxisRaw() : Axis ���� ������ ��ȯ
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        // space �� ������ �� ������ ȸ���ϵ��� GetButtonDown ���
        sDown = Input.GetButtonDown("Jump");
        // �⺻ ���� �� ��ų �Է�
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
        // �밢���̶�� �ӵ� �������� �ʰ� ���� ���� 1�� ������ ���ͷ�
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // ȸ�Ǹ� �ϰ� �ִٸ� ȸ���ϴ� �������� ġȯ
        if(isDodge)
            moveVec = dodgeVec;

        // ���� �հ� ����������
        if(!isBorder)
            transform.position += moveVec * speed * Time.deltaTime;

        // �ִϸ��̼�
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        // �÷��̾� ȸ�� (�����̴� �������� �ٶ󺻴�)
        transform.LookAt(transform.position + moveVec);
    }

    void Dodge()
    {
        if(!isMoveReady)
            return;

        dodgeDelay += Time.deltaTime;
        isDodgeReady = dodgeRate < dodgeDelay;

        // ���� �հ� ����������
        if(sDown && moveVec != Vector3.zero && !isDodge && !isBorder && isDodgeReady)
        {
            // ������ ���� -> ȸ�ǹ��� ���ͷ� �ٲ�� ����
            dodgeVec = moveVec;
            speed *= 2.0f;

            skillSound.clip = dodgeSound;
            skillSound.Play();
            anim.SetTrigger("doDodge");
            isDodge = true;

            // �ð��� �Լ� ȣ�� 0.5 ��
            Invoke("DodgeOut", 0.5f);

            // ������ �ʱ�ȭ
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

            // ��ų ����
            pv.RPC("QSkillStart", RpcTarget.All, bossPlayerViewID);

            // �ִϸ��̼�
            anim.SetTrigger("doSwing1");

            // ������ �ʱ�ȭ
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

            // ��ų ����
            pv.RPC("WSkillStart", RpcTarget.All, bossPlayerViewID);

            // �ִϸ��̼� -> Ŭ���� ���� �ٸ� �ִϸ��̼�����
            switch(lvOneSkill)
            {
            case LvOneSkill.SwordForce:
                anim.SetTrigger("doSwing2");
                break;
            } 

            // ������ �ʱ�ȭ
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

            // ��ų ����
            pv.RPC("ESkillStart", RpcTarget.All, bossPlayerViewID);

            // �ִϸ��̼� -> Ŭ���� ���� �ٸ� �ִϸ��̼�����
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

            // ������ �ʱ�ȭ
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

            // ��ų ����
            pv.RPC("RSkillStart", RpcTarget.All, bossPlayerViewID);

            // �ִϸ��̼� -> Ŭ���� ���� �ٸ� �ִϸ��̼�����
            switch(lvThreeSkill)
            {
            case LvThreeSkill.RestrictionOfBlood:
                anim.SetTrigger("doSpell");
                break;
            case LvThreeSkill.Resurrection:
                anim.SetTrigger("doSpell");
                break;
            }

            // ������ �ʱ�ȭ
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

            // �̵��ϴ� ����
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
        // �̰� ��ġ�� ���� �������� �ٲ�ߵǰ� ���ݿ� ID �ο��ؾߵ�.
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

	// �÷��̾� ������ ����
	void OnTriggerEnter(Collider other)
	{
        if(other.CompareTag("DamageObject"))
        {
            inLava = true;

            // �÷��̾� ��Ÿ�� ����Ʈ Ȱ��ȭ
            transform.Find("Skill/Fire").gameObject.SetActive(true);
  
            BossGameManager bossGameManager = GameObject.FindObjectOfType<BossGameManager>();

            // �� Ŭ���̾�Ʈ�� ��츸 ���� ����
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

            // ���� ��ų�� �����ڰ� ������ ��� �������� �����ʵ��� ��.
            if(pv.ViewID == skillOwner)
                return;

            // �� Ŭ���̾�Ʈ�� ��츸 ���� ����
            // ��� Ŭ���̾�Ʈ���� �� �÷��̾ �������� �Դ� ������ ���� ����.
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
            // ���
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

            // �÷��̾� ��Ÿ�� ����Ʈ ��Ȱ��ȭ
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


    //�ڵ� ȸ�� ����
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        // Raycast() : Ray �� ��� ��� ������Ʈ�� �����ϴ� �Լ�
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