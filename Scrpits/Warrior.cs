using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : MonoBehaviour
{
    // �÷��̾��� ���� ���� ������
    public float speed;
    public int maxHealth;
    public int health;

    float horizontalAxis;
    float verticalAxis;

    // ���� ��ư ���� ����
    bool attackDown;

    // ���� ���� ������
    float attackDelay;
    bool isReadyToAttack;

    // ���⸦ �����س���
    public Sword sword;

    // ȸ���� ��ư
    bool spinDown;

    // ������ ��ư
    bool dodgeDown;

    // ������ �߿��� �ٸ��� ����
    bool isDodge;

    // �ñر� ��ư
    bool ultiDown;

    // �ñر� ������
    public bool isUlti;

    // ����
    float ultiCoolTime = 10f;

    // �� ������ ��� �ð�
    float ultiDelay;

    // �� ���� ����
    bool isReadyToShotUlti;

    // ������ ���� ����
    Vector3 dodgeVector;

    // ���� �ٵ� ����
    Rigidbody rigid;
    // �ִϸ��̼� ����
    Animator anim;
    // ���� ī�޶� ����
    public Camera followCamera;
    // ���� ��Ҵ��� �˷��ִ� ����
    bool isBorder;


    // �̵��ϴ� ���� ����
    Vector3 moveVector;

    // �ü� ���� ����
    Vector3 lookVector;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // ��ư �����°� �ν��ϴ� ģ��
        GetInput();
        // �̵� ���� ����
        Move();
        // ���� ���� ���� ����
        Turn();
        // ���� ���� ����
        Attack();
        // ȸ���� ����
        SpinAttack();
        // ������ ���� ����
        Dodge();
        // �ñر�
        Ultimate();
    }

    void GetInput()
    {
        // ����, �������� �̵��ϴ� �ప ���� ����
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        // �Ϲ� ����
        attackDown = Input.GetButtonDown("Fire1");
        // ȸ����
        spinDown = Input.GetButtonDown("Fire2");
        // ������
        dodgeDown = Input.GetButtonDown("Jump");
        // �ñر�
        ultiDown = Input.GetButtonDown("Ultimate");
    }


    void Move()
    {

        if (!isUlti && !isBorder)
        {
            // �̵��ϴ� ���Ͱ��� Ű���� ���� �������� ��������
            // y�����δ� �̵����� ������ 0���� �����ϰ� ��� ���� ���ġ�� ���������
            moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

            transform.position += moveVector * speed * Time.deltaTime;
            anim.SetBool("isWalk", moveVector != Vector3.zero);
        }

    }

    void StopToWall()
    {
        isBorder = Physics.Raycast(transform.position, transform.forward, 2, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        StopToWall();
    }

    void Turn()
    {
        if (!isUlti)
            transform.LookAt(transform.position + moveVector);

        if (!isUlti && !isDodge)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            // RaycastHit ������ �޾�����
            RaycastHit rayHit;
            // out: return�� ���� ��ȯ���� �־��� ������ �����ϴ� Ű����
            // �Ʒ��� ���� ��ȯ���� rayHit�� ����
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                // point : rayHit�� ������ ��ġ
                Vector3 nextVector = rayHit.point - transform.position;
                nextVector.y = 0;
                lookVector = transform.position + nextVector;
                transform.LookAt(lookVector);

            }
        }

    }

    void Attack()
    {
        attackDelay += Time.deltaTime;

        // �Ϲ� ���� ������ 1�ʴ� �ѹ����� ��
        float attackRate = 1f;

        isReadyToAttack = attackRate < attackDelay;

        if (attackDown && isReadyToAttack && !isDodge && !isUlti)
        {
            sword.BaseAttack();
            anim.SetTrigger("doAttack");
            attackDelay = 0;
        }
    }

    void SpinAttack()
    {
        attackDelay += Time.deltaTime;

        float spinAttackRate = 1f;

        isReadyToAttack = spinAttackRate < attackDelay;

        if (spinDown && isReadyToAttack && !isDodge && !isUlti)
        {
            sword.SpinAttack();
            anim.SetTrigger("doSpinAttack");
            attackDelay = 0;
        }
    }

    void Dodge()
    {
        if (dodgeDown && !isUlti && moveVector != Vector3.zero)
        {
            isDodge = true;
            dodgeVector = lookVector;
            anim.SetTrigger("doDodge");


            // speed *= 2.0f;
            Invoke("GetDodge", 0.1f);

            Invoke("DodgeOut", 0.2f);
        }
    }

    void GetDodge()
    {
        speed *= 2.0f;
    }

    void DodgeOut()
    {
        isDodge = false;
        speed *= 0.5f;
    }

    void Ultimate()
    {
        ultiDelay += Time.deltaTime;

        isReadyToShotUlti = ultiCoolTime < ultiDelay;


        if (ultiDown && !isDodge && isReadyToShotUlti)
        {
            isUlti = true;
            anim.SetTrigger("doUlti");
            sword.Ultimate();
            ultiDelay = 0;

            Invoke("StopUlti", 2.1f);
        }
    }

    void StopUlti()
    {
        isUlti = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BossAttack" || other.tag == "BossAttackOver")
        {
            bool isBossAttack = other.name == "Explosion";
            StartCoroutine(OnDamage(isBossAttack));
        }
    }

    void OnParticleTrigger()
    {
        //if (other.tag == "BossAttack" || other.tag == "BossAttackOver")
        //{
          //  bool isBossAttack = other.name == "Explosion";
            StartCoroutine(OnDamage(false));
        //}
    }

    IEnumerator OnDamage(bool isBossAttack)
    {
        yield return new WaitForSeconds(0.1f);

        if (isBossAttack)
        {
            rigid.AddForce(transform.forward * -100, ForceMode.Impulse);
        }

        print("is Attacked!!");

        yield return new WaitForSeconds(3f);

        if (isBossAttack)
            rigid.velocity = Vector3.zero;
    }
}
