using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;

public class MaPagePlayerController : MonoBehaviourPun ,IPunObservable
{
    public float speed;

    float hAxis;
    float vAxis;

  


    // ȸ�� : space
    bool jDown;

    // ȸ�� ����
    bool isDodge;

    // �� �浹 �÷��� bool ������ ����
    bool isBorder;

    Vector3 moveVec;

    // ȸ�� ���� ���� ���� ���ϵ���
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    public int value;
    MyPageNetworkManager MN;
 



    private void Awake()
    {
      
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    
    }

  

    void Start()
    {

      
        MN = GameObject.FindWithTag("MyPageNetworkManager").GetComponent<MyPageNetworkManager>();
        

    }

    void Update()
    {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
        Transform transform1 = transform;
        CM.Follow = transform1;
            CM.LookAt = transform;
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
        // fDown = Input.GetButton("Fire1");
    }

    void Move()
    {
        // normalized
        // �밢���̶�� �ӵ� �������� �ʰ�.
        // ���� ���� 1�� ������ ����
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // ȸ�Ǹ� �ϰ� �ִٸ�
        if (isDodge)
            moveVec = dodgeVec;

        // ���� �հ� ����������
        if (!isBorder)
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

    }

    void Dodge()
    {// ���� �հ� ����������
        if (jDown && moveVec != Vector3.zero && !isDodge && !isBorder)
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(value);  
        else value = (int)stream.ReceiveNext();
        
    }
    


}