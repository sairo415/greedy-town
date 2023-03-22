using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayer : MonoBehaviour
{
    // Camera
    public Camera followCamera;

    float hAxis;
    float vAxis;

    // Ű�Է�
    bool sDown; // ȸ�� : space
    bool qDown; // �⺻ ����
    bool wDown; // ��ų 1
    bool eDown; // ��ų 2
    bool rDown; // ��ų 3

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

    float normalAtkDelay;
    public float normalAtkRate;
    bool isNormalAtkReady;

    // SwordMaster
    public GameObject swordMasterAttack;

    public Transform swordForcePos;
    public GameObject swordForce;

    public Transform swordDancePos;
    public GameObject swordDance;

    public Transform swordFlashPos;
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
    public GameObject bloodExplosion;

    public Transform restrictionOfBloodPos;
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
    public GameObject blessing;

    public Transform resurrectionPos;
    public GameObject resurrection;

    public float intentRate;
    public float intentPlayTime;

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

    // ȸ�� ����
    bool isDodge;

    // �� �浹 ����
    bool isBorder;

    Vector3 moveVec;    // ������ ����
    Vector3 dodgeVec;   // ȸ�� ���� ���� ���� ���ϵ���

    Rigidbody rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        isSkillReady = true;
        isMoveReady = true;

        normalAtkDelay = 1000f;
        wSkillDelay = 1000f;
        eSkillDelay = 1000f;
        rSkillDelay = 1000f;
    }

    void Update()
    {
        //�Է�
        GetInput();

        //������
        Move();
        Turn();

        //ȸ��
        Dodge();

        //����
        Attack();
        WSkill();
        ESkill();
        RSkill();
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
            anim.SetTrigger("doDodge");
            isDodge = true;

            // �ð��� �Լ� ȣ�� 0.5 ��
            Invoke("DodgeOut", 0.5f);
        }
    }

    void Attack()
    {
        if(!isSkillReady)
            return;

        normalAtkDelay += Time.deltaTime;
        isNormalAtkReady = normalAtkRate < normalAtkDelay;

        if(qDown && isNormalAtkReady && !isDodge)
        {
            // ��ų ����
            StartCoroutine("QSkillStart");

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
            StartCoroutine("WSkillStart");

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
            StartCoroutine("ESkillStart");

            // �ִϸ��̼� -> Ŭ���� ���� �ٸ� �ִϸ��̼�����
            switch(lvTwoSkill)
            {
            case LvTwoSkill.SwordDance:
                anim.SetTrigger("doSwing2");
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
            StartCoroutine("RSkillStart");

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

    IEnumerator QSkillStart()
    {
        GameObject skillAreaObj = null;

        switch(lvOneSkill)
        {
        case LvOneSkill.NULL:
            skillAreaObj = Instantiate(normalAttack, gameObject.transform.position, gameObject.transform.rotation);
            break;
        case LvOneSkill.SwordForce:
            skillAreaObj = Instantiate(swordMasterAttack, gameObject.transform.position, gameObject.transform.rotation);
            break;
        case LvOneSkill.Vampirism:
            skillAreaObj = Instantiate(berserkerAttack, gameObject.transform.position, gameObject.transform.rotation);
            break;
        case LvOneSkill.Intent:
            skillAreaObj = Instantiate(paladinkerAttack, gameObject.transform.position, gameObject.transform.rotation);
            break;
        }

        if(skillAreaObj == null)
            yield break;

        skillAreaObj.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        Destroy(skillAreaObj);
        skillAreaObj.SetActive(false);
    }

    IEnumerator WSkillStart()
    {
        if(lvOneSkill == LvOneSkill.SwordForce)
        {
            GameObject skillAreaObj = Instantiate(swordForce, swordForcePos.position, swordForcePos.rotation);

            skillAreaObj.SetActive(true);

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

    IEnumerator ESkillStart()
    {
        if(lvTwoSkill == LvTwoSkill.SwordDance)
        {
            GameObject skillAreaObj = Instantiate(swordDance, swordDancePos.position, swordDancePos.rotation);

            skillAreaObj.SetActive(true);

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

    IEnumerator RSkillStart()
    {
        if(lvThreeSkill == LvThreeSkill.SwordFlash)
        {
            GameObject skillAreaObj = Instantiate(swordFlash, swordFlashPos.position, swordFlashPos.rotation);

            skillAreaObj.SetActive(true);

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
        FreezeRotation();
        StopToWall();
    }
}
