using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    
    public Camera followCamera;
    
    float hAxis;
    float vAxis;
    
    


    // ȸ�� : space
    bool jDown;


    // Ű�Է�, ���ݵ�����, ���� �غ� ���� ����
    // bool fDown;
    bool qDown;
    bool wDown;
    bool eDown;
    bool rDown;

    // ȸ�� ����
    bool isDodge;

    // �� �浹 �÷��� bool ������ ����
    bool isBorder;

    Vector3 moveVec;

    // ȸ�� ���� ���� ���� ���ϵ���
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    public Transform swordForcePos;
    public GameObject swordForce;

    public Transform swordDancePos;
    public GameObject swordDance;

    public Transform eternalSlashDancePos;
    public GameObject eternalSlashDance;


    // ���� ������ //
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
        jDown = Input.GetButtonDown("Jump");
        
        qDown = Input.GetButtonDown("Skill1");
        wDown = Input.GetButtonDown("Skill2");
        eDown = Input.GetButtonDown("Skill3");
        rDown = Input.GetButtonDown("Skill4");
    }

    void Move()
    {
        // normalized
        // �밢���̶�� �ӵ� �������� �ʰ�.
        // ���� ���� 1�� ������ ����
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // ȸ�Ǹ� �ϰ� �ִٸ�
        if(isDodge)
            moveVec = dodgeVec;

        // ���� �հ� ����������
        if(!isBorder)
        {
            transform.position += moveVec * speed * Time.deltaTime;
        }

        // �ִϸ��̼�
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        // �÷��̾� ȸ�� (�����̴� �������� �ٶ󺻴�)
        transform.LookAt(transform.position + moveVec);

        /*if(fDown)
        {
            // ���콺�� ���� ȸ��
            // ��ũ������ ����� Ray �� ��� �Լ� ScreenPointToRay();
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            // ������ �굵�� ũ�� �� 100
            // out return ó�� ��ȯ ���� �־��� ������ �����ϴ� Ű����
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                // ���� ���� - �÷��̾��� ��ġ = ��� ��ġ
                // �� ��ġ�� �÷��̾ �ٶ�
                Vector3 nextVec = rayHit.point - transform.position;
                // RayCastHit �� ���̴� �����ϵ��� y �� ���� 0����
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }*/
    }

    void Dodge()
    {// ���� �հ� ����������
        if(jDown && moveVec != Vector3.zero && !isDodge && !isBorder)
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

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
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