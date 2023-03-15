using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �ν����� â���� ���� �����ϱ� ���� public ����
    public float speed;

    // ���콺�� ���� ��ȯ
    public Camera followCamera;

    // ���� ���� �������� ����
    float hAxis;
    float vAxis;

    // space �Է��� �ޱ� ���� (����)
    bool jDown;

    // Ű�Է�, ���ݵ�����, ���� �غ� ���� ����
    bool fDown;

    // ���� 1ȸ�� �����ϵ��� �ٴڿ� ��Ҵ��� �Ǵ�
    bool isDodge;

    // �� �浹 �÷��� bool ������ ����
    bool isBorder;

    Vector3 moveVec;

    // ȸ�� ���� ���� ���� ���ϵ���
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        // �ڽ� ������Ʈ�� �ִ� ������Ʈ�� ������
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Dodge();
    }

    void GetInput()
    {
        // GetAxisRaw() : Axis ���� ������ ��ȯ
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        // space �� ������ �� ������ �ٵ��� GetButtonDown ���
        jDown = Input.GetButtonDown("Jump");
        // �⺻������ ���콺 ���ʿ� Fire1 �� �� ����
        fDown = Input.GetButton("Fire1");
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

        if(fDown)
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
        }
    }

    void Dodge()
    {// ���� �հ� ����������
        if(jDown && moveVec != Vector3.zero && !isDodge && !isBorder)
        {
            // ������ ���� -> ȸ�ǹ��� ���ͷ� �ٲ�� ����
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // �ð��� �Լ� ȣ�� 0.5 ��
            Invoke("DodgeOut", 0.5f);
        }
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