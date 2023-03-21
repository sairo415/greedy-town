using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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

    // ��ų
    public Transform swordForcePos;
    public GameObject swordForce;

    public Transform swordDancePos;
    public GameObject swordDance;

    public Transform eternalSlashDancePos;
    public GameObject eternalSlashDance;

    // �⺻ ���� ������
    float qSkillDelay;  // q ��� �� ����� �ð�
    public float qSkillRate;   // q ���� ��� �ð�
    bool isQSkillDelay; // q ��ų ��� ���� ����

    // w ���� ������
    float wSkillDelay;
    public float wSkillRate;
    bool isWSkillReady;
    //public int wSkillDamage;

    // e ���� ������
    float eSkillDelay;
    public float eSkillRate;
    bool isESkillReady;
    //public int eSkillDamage;

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

    }

    void WSkill()
    {
        wSkillDelay += Time.deltaTime;
        isWSkillReady = wSkillRate < wSkillDelay;

        if(wDown && isWSkillReady && !isDodge)
        {
            // ��ų ����
            StartCoroutine("WSkillStart");

            // �ִϸ��̼�
            anim.SetTrigger("doSwing1");

            // ������ �ʱ�ȭ
            wSkillDelay = 0;
        }
    }

    void ESkill()
    {
        eSkillDelay += Time.deltaTime;
        isESkillReady = eSkillRate < eSkillDelay;

        if(eDown && isESkillReady && !isDodge)
        {
            // ��ų ����
            StartCoroutine("ESkillStart");

            // �ִϸ��̼�
            anim.SetTrigger("doSwing2");

            // ������ �ʱ�ȭ
            eSkillDelay = 0;
        }
    }

    void RSkill()
    {
        rSkillDelay += Time.deltaTime;
        isRSkillReady = rSkillRate < rSkillDelay;

        if(rDown && isRSkillReady && !isDodge)
        {
            // ��ų ����
            StartCoroutine("RSkillStart");

            // �ִϸ��̼�
            anim.SetTrigger("doSwing3");

            // ������ �ʱ�ȭ
            rSkillDelay = 0;
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    IEnumerator WSkillStart()
    {
        GameObject skillAreaObj = Instantiate(swordForce, swordForcePos.position, swordForcePos.rotation);
        skillAreaObj.SetActive(true);

        Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
        skillAreaRigid.velocity = swordForcePos.forward * 50;

        yield return null;
    }

    IEnumerator ESkillStart()
    {
        GameObject skillAreaObj = Instantiate(swordDance, swordDancePos.position, swordDancePos.rotation);
        skillAreaObj.SetActive(true);

        Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
        skillAreaRigid.velocity = swordForcePos.forward;

        yield return null;
    }

    IEnumerator RSkillStart()
    {
        GameObject skillAreaObj = Instantiate(eternalSlashDance, eternalSlashDancePos.position, eternalSlashDancePos.rotation);
        skillAreaObj.SetActive(true);

        Rigidbody skillAreaRigid = skillAreaObj.GetComponent<Rigidbody>();
        skillAreaRigid.velocity = swordForcePos.forward;

        yield return null;
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