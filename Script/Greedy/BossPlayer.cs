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
    public enum LvThreeSkill {NULL, SwordBlade, BloodField, Resurrection};
    public LvThreeSkill lvThreeSkill;

    // Ư�� ��ų ����ϸ鼭 �ٸ� ��ų�� ������� ���ϰ� �ϰ� ���� ��� (�Ϲ� ���� ����)
    // ��ų ���� ���� false �� �����Ͽ� �ٸ� ��ų�� ��� ���ϰ� ��.
    [SerializeField]
    public bool isSkillReady;
    // Ư�� ��ų ����ϸ鼭 �������� �����ϰ� ���� ���
    // ��ų ���� ���� false �� �����Ͽ� �������� ����.
    [SerializeField]
    public bool isMoveReady;

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

    public float swordBladeRate;
    public float swordBladePlayTime;

    // Berserker
    public GameObject berserkerAttack;

    //public Transform vampirismPos;
    //public GameObject vampirism;

    public Transform bloodExplosionPos;
    public AudioClip bloodExplosionSound;
    public GameObject bloodExplosion;

    public Transform bloodFieldPos;
    public AudioClip bloodFieldSound;
    public GameObject bloodField;

    //public float vampirismRate;
    //public float vampirismPlayTime;

    public float bloodExplosionRate;
    public float bloodExplosionPlayTime;

    public float bloodFieldRate;
    public float bloodFieldPlayTime;

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

    [SerializeField]
    public Vector3 moveVec;    // ������ ����
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

    // �� ���� ���� ���� ���
    float healTimer = 0.0f;
    int healAmount = 10;
    float healInterval = 1.0f;

    bool inLava = false;        // ��� ��
    bool inHealArea = false;    // �� ���� ����
    bool isInvincible = false;  // ���� ����

    // ��Ȱ ���θ� Ȯ���ϱ� ���� �׾����� Ȯ��
    [SerializeField]
    public bool isDie = false;

    bool isInternalDelay = false;

    public bool doResurrection = false; // ��Ȱ ��ų ����.

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

            inLava = false;
            inHealArea = false;
            isInvincible = false;

            isDie = false;
            isInternalDelay = false;

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

    private void LateUpdate()
    {
        if(curHealth <= 0)
        {
            curHealth = 0;
            anim.SetTrigger("doDie");
            isSkillReady = false;
            isMoveReady = false;
            isDie = true;
        }
        else if(isDie && curHealth > 0)
        {
            anim.SetTrigger("doAlive");
            isSkillReady = true;
            isMoveReady = true;
            isDie = false;
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

        //Debug.Log("Time : " + eSkillDelay);

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

        if(doResurrection)
            return;

        if(lvThreeSkill == LvThreeSkill.NULL)
            return;

        switch(lvThreeSkill)
        {
        case LvThreeSkill.SwordBlade:
            rSkillRate = swordBladeRate;
            break;
        case LvThreeSkill.BloodField:
            rSkillRate = bloodFieldRate;
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
            case LvThreeSkill.BloodField:
                anim.SetTrigger("doSpell");
                break;
            case LvThreeSkill.Resurrection:
                anim.SetTrigger("doSpell");
                break;
            }

            if(!isInternalDelay)
                rSkillDelay = 0.0f;

            isInternalDelay = false;
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

        if(pv.IsMine)
        {
            skillSound.clip = swingSound;
            skillSound.Play();
        }
        
        yield return new WaitForSeconds(0.5f);

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

            if(pv.IsMine)
            {
                skillSound.clip = swordForceSound;
                skillSound.Play();
            }

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
    IEnumerator ESkillStart(int bossPlayerViewID)
    {
        GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;

        // �̰� ��ġ�� ���� �������� �ٲ�ߵǰ� ���ݿ� ID �ο��ؾߵ�.
        if(bossPlayerObj.GetComponent<BossPlayer>().lvTwoSkill == LvTwoSkill.SwordDance)
        {
            GameObject skillAreaObj = Instantiate(swordDance, swordDancePos.position, swordDancePos.rotation);

            skillAreaObj.GetComponent<BossPlayerSkill>().SetID(bossPlayerViewID);
            skillAreaObj.SetActive(true);

            if(pv.IsMine)
            {
                skillSound.clip = swordDanceSound;
                skillSound.Play();
            }

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = swordDancePos.forward;

            yield return new WaitForSeconds(swordDancePlayTime);

            if(skillAreaObj == null)
                yield break;

            skillAreaObj.SetActive(false);
            Destroy(skillAreaObj);
        }
        else if(bossPlayerObj.GetComponent<BossPlayer>().lvTwoSkill == LvTwoSkill.BloodExplosion)
        {
            GameObject skillAreaObj = Instantiate(bloodExplosion, bloodExplosionPos.position, bloodExplosionPos.rotation);

            skillAreaObj.GetComponent<BossPlayerSkill>().SetID(bossPlayerViewID);
            skillAreaObj.SetActive(true);

            bossPlayerObj.GetComponent<BossPlayer>().curHealth -= 10;
            pv.RPC("SyncPlayerHP", RpcTarget.All, curHealth);

            if(pv.IsMine)
            {
                skillSound.clip = bloodExplosionSound;
                skillSound.Play();
            }

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = swordDancePos.forward;

            yield return new WaitForSeconds(bloodExplosionPlayTime);

            if(skillAreaObj == null)
                yield break;

            skillAreaObj.SetActive(false);
            Destroy(skillAreaObj);
        }
        else if(bossPlayerObj.GetComponent<BossPlayer>().lvTwoSkill == LvTwoSkill.Blessing)
        {
            GameObject skillAreaObj = Instantiate(blessing, blessingPos.position, blessingPos.rotation);

            skillAreaObj.SetActive(true);

            if(pv.IsMine)
            {
                skillSound.clip = blessingSound;
                skillSound.Play();
            }

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = swordDancePos.forward;

            float startTime = Time.realtimeSinceStartup;

            yield return new WaitForSeconds(blessingPlayTime);

            if(skillAreaObj == null)
                yield break;

            skillAreaObj.SetActive(false);
            Destroy(skillAreaObj);
        }
    }

    [PunRPC]
    IEnumerator RSkillStart(int bossPlayerViewID)
    {
        GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;

        if(bossPlayerObj.GetComponent<BossPlayer>().lvThreeSkill == LvThreeSkill.SwordBlade)
        {
            GameObject skillAreaObj = Instantiate(swordFlash, swordFlashPos.position, swordFlashPos.rotation);

            skillAreaObj.GetComponent<BossPlayerSkill>().SetID(bossPlayerViewID);
            skillAreaObj.SetActive(true);

            if(pv.IsMine)
            {
                skillSound.clip = swordFlashSound;
                skillSound.Play();
            }

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = swordFlashPos.forward;

            //�÷��̾� ����
            bossPlayerObj.GetComponent<BossPlayer>().isInvincible = true;

            Vector3 originalScale = gameObject.transform.localScale;
            gameObject.transform.localScale = Vector3.zero;

            isMoveReady = false;
            isSkillReady = false;

            isInternalDelay = true;
            float startTime = Time.realtimeSinceStartup;

            yield return new WaitForSeconds(swordBladePlayTime);

            rSkillDelay = (Time.realtimeSinceStartup - startTime);

            gameObject.transform.localScale = originalScale;
            isMoveReady = true;
            isSkillReady = true;

            //�÷��̾� ���� ����
            bossPlayerObj.GetComponent<BossPlayer>().isInvincible = false;

            if(skillAreaObj == null)
                yield break;

            skillAreaObj.SetActive(false);
            Destroy(skillAreaObj);
        }
        else if(bossPlayerObj.GetComponent<BossPlayer>().lvThreeSkill == LvThreeSkill.BloodField)
        {
            GameObject skillAreaObj = Instantiate(bloodField, bloodFieldPos.position, bloodFieldPos.rotation);

            skillAreaObj.GetComponent<BossPlayerSkill>().SetID(bossPlayerViewID);
            skillAreaObj.SetActive(true);

            if(pv.IsMine)
            {
                skillSound.clip = bloodFieldSound;
                skillSound.Play();
            }

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = bloodFieldPos.forward;

            //isMoveReady = false;
            isSkillReady = false;

            isInternalDelay = true;
            float startTime = Time.realtimeSinceStartup;

            yield return new WaitForSeconds(bloodFieldPlayTime);

            rSkillDelay = (Time.realtimeSinceStartup - startTime);

            //isMoveReady = true;
            isSkillReady = true;

            if(skillAreaObj == null)
                yield break;

            skillAreaObj.SetActive(false);
            Destroy(skillAreaObj);
        }
        else if(bossPlayerObj.GetComponent<BossPlayer>().lvThreeSkill == LvThreeSkill.Resurrection)
        {            
            GameObject skillAreaObj = Instantiate(resurrection, resurrectionPos.position, resurrectionPos.rotation);

            skillAreaObj.SetActive(true);

            if(pv.IsMine)
            {
                skillSound.clip = resurrectionSound;
                skillSound.Play();
            }

            Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
            skillAreaRigid.velocity = resurrectionPos.forward;

            // ���� �ִ� �÷��̾� Ŭ�е��� ������ �����ͼ�, Dictionary �� ������Ʈ ��.
            BossPlayer[] bossPlayers = FindObjectsOfType<BossPlayer>();
            foreach(BossPlayer bossPlayer in bossPlayers)
            {
                int targetID = bossPlayer.pv.ViewID;
                pv.RPC("SyncResurrection", RpcTarget.All, targetID);
            }

            //float startTime = Time.realtimeSinceStartup;

            yield return new WaitForSeconds(resurrectionPlayTime);

            //rSkillDelay = (Time.realtimeSinceStartup - startTime);

            if(skillAreaObj == null)
                yield break;

            Debug.Log("��Ȱ ��ų ����");
            doResurrection = true;
            rSkillRate = 10000000.0f;

            skillAreaObj.SetActive(false);
            Destroy(skillAreaObj);
        }
    }

    [PunRPC]
    void SyncResurrection(int bossPlayerViewID)
    {
        GameObject bossPlayerObj = PhotonView.Find(bossPlayerViewID).gameObject;
        if(bossPlayerObj.GetComponent<BossPlayer>().curHealth == 0)
        {
            bossPlayerObj.GetComponent<BossPlayer>().curHealth = bossPlayerObj.GetComponent<BossPlayer>().maxHealth;
            //bossPlayerObj.GetComponent<BossPlayer>().anim.SetTrigger("doAlive");

            BossMemberManager memberManager = FindObjectOfType<BossMemberManager>();
            memberManager.UpdatePlayerInfoList();
            memberManager.UpdateUI();
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

            int damage = other.GetComponent<BossPlayerSkill>().damage - defense;
            if(damage < 0)
                damage = 0;

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
        else if(other.tag == "HealArea")
        {
            inHealArea = true;
        }
        else if(other.tag == "BossAttack" && pv.IsMine)
        {
            if(other.GetComponent<BossAttack>() == null)
                return;

            int damage = other.GetComponent<BossAttack>().damage - defense;
            if(damage < 0)
                damage = 0;

            curHealth -= damage;
            if(curHealth <= 0)
                curHealth = 0;

            pv.RPC("SyncPlayerHP", RpcTarget.All, curHealth);
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
        else if(other.tag == "HealArea")
        {
            inHealArea = false;

            if(pv.IsMine)
            {
                healTimer = 0.0f;
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

                if(curHealth <= 0)
                    curHealth = 0;

                damageTimer = 0.0f;
            }

            pv.RPC("SyncPlayerHP", RpcTarget.All, curHealth);
        }
        else if(other.CompareTag("HealArea") && inHealArea && pv.IsMine)
        {
            healTimer += Time.deltaTime;

            if(healTimer >= healInterval)
            {
                curHealth += healAmount;
                if(curHealth >= maxHealth)
                    curHealth = maxHealth;

                healTimer = 0.0f;
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
