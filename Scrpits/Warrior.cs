using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : MonoBehaviour
{
    // �÷��̾��� �̵��ӵ�
    public float speed;

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

    // ���� �ٵ� ����
    Rigidbody rigid;
    // �ִϸ��̼� ����
    Animator anim;


    // �̵��ϴ� ���� ����
    Vector3 moveVector;


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
    }


    void Move()
    {
        // �̵��ϴ� ���Ͱ��� Ű���� ���� �������� ��������
        // y�����δ� �̵����� ������ 0���� �����ϰ� ��� ���� ���ġ�� ���������
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

        transform.position += moveVector * speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVector != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVector);
    }

    void Attack()
    {
        attackDelay += Time.deltaTime;

        // �Ϲ� ���� ������ 0.7�ʴ� �ѹ����� ��
        float attackRate = 0.7f;

        isReadyToAttack = attackRate < attackDelay;

        if (attackDown && isReadyToAttack)
        {
            sword.Use();
            anim.SetTrigger("doAttack");
            attackDelay = 0;
        }
    }

    void SpinAttack()
    {
        attackDelay += Time.deltaTime;

        float spinAttackRate = 1f;

        isReadyToAttack = spinAttackRate < attackDelay;

        if (spinDown && isReadyToAttack)
        {
            sword.Use();
            anim.SetTrigger("doSpinAttack");
            attackDelay = 0;
        }
    }
}
