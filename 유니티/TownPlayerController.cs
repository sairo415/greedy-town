using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;
using UnityEngine.SceneManagement;

public class TownPlayerController : MonoBehaviourPun, IPunObservable
{
    public float speed;

    float hAxis;
    float vAxis;

    private Vector3 start = new Vector3(-18, 5, -12);


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
    TownNetworkManager NM;
    PhotonView PV;



    private void Awake()
    {

        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        
    }



    void Start()
    {
       

        PV = photonView;
        NM = GameObject.FindWithTag("TownNetworkManager").GetComponent<TownNetworkManager>();

        Debug.Log(transform.Find("Body04").GetSiblingIndex()+" ���°��?");
        
        transform.Find("Body04").gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(0).gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").Find("Shield05").gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").Find("OHS16_Sword").gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("AC05_Horn04").gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("Hair13").gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("Head05_Santa").gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("Hat13").gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("Eyebrow02").gameObject.SetActive(true);

    }
    void Update()
    {
        if (PV.IsMine)
        {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
            GetInput();
            Move();
            Turn();
            Dodge();
        }

        // �߶��ϸ� ������
        if (this.gameObject.transform.position.y < -100)
        {
            this.gameObject.transform.position = start;
        }


    }
    // �迡 ������ �������̵� �κ�� �̵�
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Boat")
        {
            ToTheBossLobby();
        }
    }

    public void ToTheBossLobby()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LeaveRoom();
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