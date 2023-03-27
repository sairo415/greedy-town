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

    // �ӵ�
    public float speed;

    // 1�ܰ� ��ų
    public enum LvOneSkill{NULL, SwordForce, Vampirism, Intent};
    public LvOneSkill lvOneSkill;

    // 2 �ܰ� ��ų
    public enum LvTwoSkill{NULL, SwordDance, BloodExplosion,Blessing};
    public LvTwoSkill lvTwoSkill;

    // 3 �ܰ� ��ų
    public enum LvThreeSkill {NULL, SwordFlash, RestrictionOfBlood, Resurrection};
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

    public float swordForceRate;        // ��ų ���� ��� �ð�
    public float swordForcePlayTime;    // ��ų ������Ʈ ���� �ð�

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

    // w ���� ������
    float wSkillDelay;
    float wSkillRate;
    bool isWSkillReady;

    // e ���� ������
    float eSkillDelay;
    float eSkillRate;
    bool isESkillReady;

    // r ���� ������
    float rSkillDelay;
    float rSkillRate;
    bool isRSkillReady;

    // ������ ����
    public AudioClip dodgeSound;

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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        skillSound = GetComponent<AudioSource>();
        bossGameManager = GetComponent<BossGameManager>();

        // ü�� �ʱ�ȭ
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
            // �ڽ��� ĳ������ ��� �ó׸ӽ� ī�޶� ����
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;

            // ���� �޴��� ����
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
        // ���� �հ� ����������
        if(sDown && moveVec != Vector3.zero && !isDodge && !isBorder)
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
            // ��ų ����
            pv.RPC("QSkillStart", RpcTarget.All);

            // �ִϸ��̼�
            anim.SetTrigger("doSwing1");

            // ������ �ʱ�ȭ
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
            // ��ų ����
            pv.RPC("WSkillStart", RpcTarget.All);

            // �ִϸ��̼� -> Ŭ���� ���� �ٸ� �ִϸ��̼�����
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

            // ������ �ʱ�ȭ
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
            // ��ų ����
            pv.RPC("ESkillStart", RpcTarget.All);

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
                anim.SetTrigger("doSwing2");
                break;
            }

            // ������ �ʱ�ȭ
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
            // ��ų ����
            pv.RPC("RSkillStart", RpcTarget.All);

            // �ִϸ��̼� -> Ŭ���� ���� �ٸ� �ִϸ��̼�����
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

	// �÷��̾� ������ ����
	void OnTriggerEnter(Collider other)
	{
        if(other.CompareTag("DamageObject"))
        {
            // �÷��̾� ��Ÿ�� ����Ʈ Ȱ��ȭ
            transform.Find("Skill/Fire").gameObject.SetActive(true);
        }
        /*else if(other.tag == "PlayerAttack" || other.tag == "PlayerAttackOver")
        {
            int damage = other.GetComponent<BossPlayerSkill>().damage;
            //curHealth -= other.GetComponent<BossPlayerSkill>().damage;
            //if(curHealth < 0)
            //    curHealth = 0;

            // ���� ����� �� �����ǵ��� Destroy() ȣ��
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
            // �÷��̾� ��Ÿ�� ����Ʈ ��Ȱ��ȭ
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
