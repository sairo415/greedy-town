using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TownPlayerController : MonoBehaviourPun, IPunObservable
{
    public float speed;
    GameObject moveUI;
    string moveDest = "";
    Text moveText;
    float hAxis;
    float vAxis;

   

    private Vector3 start = new Vector3(-18, 5, -12);

    private string baseUrl = "http://j8a808.p.ssafy.io:8080/";
    //    private string baseUrl = "localhost:8080/";

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


    int dressNum;
    int backNum;
    int sheildNum;
    int weaponNum;
    int acsNum;
    int hairNum;
    int headNum;
    int hatNum;


    private void Awake()
    {

        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        /*      moveUI = GameObject.Find("Canvas").transform.Find("MoveYesNo").gameObject;
              moveText = moveUI.GetComponentInChildren<Text>();*/
    }

    [PunRPC]
    void GetOtherPlayer(int viewId)
    {
        TownPlayerController responsePlayer = PhotonView.Find(viewId).gameObject.GetComponent<TownPlayerController>();
        dressNum = responsePlayer.dressNum;
        backNum = responsePlayer.backNum;
        sheildNum = responsePlayer.sheildNum;
        weaponNum = responsePlayer.weaponNum;
        acsNum = responsePlayer.acsNum;
        hairNum = responsePlayer.hairNum;
        headNum = responsePlayer.headNum;
        hatNum = responsePlayer.hatNum;

        PV.RPC("SyncClothes", RpcTarget.Others, viewId, dressNum, backNum, sheildNum, weaponNum, acsNum, hairNum, headNum, hatNum);
    }

    [PunRPC]
    void SyncClothes(int viewId, int dressNum, int backNum, int sheildNum, int weaponNum, int acsNum, int hairNum, int headNum, int hatNum)
    {
        TownPlayerController me = PhotonView.Find(viewId).GetComponent<TownPlayerController>();

        me.transform.GetChild(dressNum).gameObject.SetActive(true);
        if (backNum != 100)
        {
            if (backNum < 3)
            {
                me.transform.GetChild(backNum + 20).gameObject.SetActive(true);
            }
            else
            {
                me.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(backNum - 3).gameObject.SetActive(true);
            }
        }
        if (sheildNum != 100)
        {
            me.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(sheildNum + 17).gameObject.SetActive(true);
        }
        me.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(weaponNum + 1).gameObject.SetActive(true);
        if (acsNum != 100)
        {
            me.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(acsNum).gameObject.SetActive(true);
        }
        if (hairNum != 100)
        {
            me.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(hairNum + 63).gameObject.SetActive(true);
        }
        me.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(headNum + 76).gameObject.SetActive(true);
        if (hatNum != 100)
        {
            me.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(hatNum + 96).gameObject.SetActive(true);
        }
        me.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("Eyebrow02").gameObject.SetActive(true);
    }


    void Start()
    {

        dressNum = PlayerPrefs.GetInt("dressNum");
        backNum = PlayerPrefs.GetInt("backNum");
        sheildNum = PlayerPrefs.GetInt("sheildNum");
        weaponNum = PlayerPrefs.GetInt("weaponNum");
        acsNum = PlayerPrefs.GetInt("acsNum");
        hairNum = PlayerPrefs.GetInt("hairNum");
        headNum = PlayerPrefs.GetInt("headNum");
        hatNum = PlayerPrefs.GetInt("hatNum");

        PV = photonView;
        NM = GameObject.FindWithTag("TownNetworkManager").GetComponent<TownNetworkManager>();
        //PV.RPC("SyncClothes", RpcTarget.All, PV.ViewID, PlayerPrefs.GetInt("dressNum"), PlayerPrefs.GetInt("backNum"), PlayerPrefs.GetInt("sheildNum"), PlayerPrefs.GetInt("weaponNum"), PlayerPrefs.GetInt("acsNum"), PlayerPrefs.GetInt("hairNum"), PlayerPrefs.GetInt("headNum"), PlayerPrefs.GetInt("hatNum"));
        if (PV.IsMine)
        {
            PV.RPC("SyncClothes", RpcTarget.All, PV.ViewID, dressNum, backNum, sheildNum, weaponNum, acsNum, hairNum, headNum, hatNum);
        } else
        {
            PV.RPC("GetOtherPlayer", RpcTarget.Others, PV.ViewID);
        }

            /*if(PV.IsMine)
            {
                Debug.Log(PlayerPrefs.GetInt("dressNum"));
                Debug.Log(PlayerPrefs.GetInt("backNum"));
                Debug.Log(PlayerPrefs.GetInt("sheildNum"));
                Debug.Log(PlayerPrefs.GetInt("weaponNum"));
                Debug.Log(PlayerPrefs.GetInt("acsNum"));
                Debug.Log(PlayerPrefs.GetInt("hairNum"));
                Debug.Log(PlayerPrefs.GetInt("headNum"));
                Debug.Log(PlayerPrefs.GetInt("hatNum"));

                transform.GetChild(PlayerPrefs.GetInt("dressNum")).gameObject.SetActive(true);
                if(PlayerPrefs.GetInt("backNum") != 100)
                {
                    if(PlayerPrefs.GetInt("backNum") < 3) 
                    {
                        transform.GetChild(PlayerPrefs.GetInt("backNum") + 20).gameObject.SetActive(true);
                    }
                    else
                    {
                        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(PlayerPrefs.GetInt("backNum") - 3).gameObject.SetActive(true);
                    }
                } 
                if(PlayerPrefs.GetInt("sheildNum") != 100)
                {
                    transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(PlayerPrefs.GetInt("sheildNum") + 17).gameObject.SetActive(true);
                }
                transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(PlayerPrefs.GetInt("weaponNum") + 1).gameObject.SetActive(true);
                if(PlayerPrefs.GetInt("acsNum") != 100)
                {
                    transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("acsNum")).gameObject.SetActive(true);
                }
                if(PlayerPrefs.GetInt("hairNum") != 100)
                {
                    transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("hairNum") + 63).gameObject.SetActive(true);
                }
                transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("headNum") + 76).gameObject.SetActive(true);
                if(PlayerPrefs.GetInt("hatNum") != 100)
                {
                    transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("hatNum") + 96).gameObject.SetActive(true);
                }
                transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("Eyebrow02").gameObject.SetActive(true);

            }*/

        }
        void Update()
    {
        if (PV.IsMine)
        {
            moveUI = GameObject.Find("Canvas").transform.Find("MoveYesNo").gameObject;
            moveText = moveUI.GetComponentInChildren<Text>();
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
            GetInput();
            Move();
            Turn();
            Dodge();
        }

        // �߶��ϸ� ������
        if (this.gameObject.transform.position.y < -50)
        {
            this.gameObject.transform.position = start;
        }
    }
    void OnCollisionEnter(Collision col)
    {
        string place = "";
        if (col.gameObject.name == "Boat")
        {
            place = "���̵�";
            moveDest = "Boat";
        }
        // ��ŷ ǥ���ǿ� ���� ��ŷ ��ȸ
        else if (col.gameObject.name == "PinnedWall")
        {
            StartCoroutine(Ranking());
            return;
        }
        // ���ⱸ�� ���� �켭�� �̵�
        else if (col.gameObject.name == "HotAirBalloon_Blue")
        {
            print("�켭 ����");
            moveDest = "HotAirBalloon_Blue";
            place = "�����̹�";
        }
        // �������� ���� �������� �̵�
        else if (col.gameObject.name == "Magic_Shop")
        {
            print("���� ����");
            moveDest = "Magic_Shop";
            place = "����";
        }
        // Ű����ũ�� ���� ī����� �̵�
        else if (col.gameObject.name == "Kiosk_Shop")
        {
            print("ī���� ����");
            moveDest = "Kiosk_Shop";
            place = "ī����";
        }
        // �������� ���� PVP�� �̵�
        else if (col.gameObject.name == "PoliceStation_1Light")
        {
            print("PVP ����");
            moveDest = "PoliceStation_1Light";
            place = "PVP";
        }
        else
        {
            return;
        }
        moveText.text = place + "�� ���ðڽ��ϱ�?";
        moveUI.GetComponent<RectTransform>().localScale = Vector3.one;
        moveUI.GetComponent<TownUI>().moveDest = moveDest;
    }



    public void Alert(string message)
    {

    }

    public void ToTheBossLobby()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LeaveRoom();
    }


    // ��ŷ
    private IEnumerator Ranking()
    {
        string url = baseUrl + "social/ranking";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // header�� accessToken ���
            request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("accessToken"));
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                // accessToken ����Ǿ�����
                if (request.responseCode == 401)
                {
                    print("��ū ����");
                    // accesToken ��߱� �� ��õ� (refreshToken �����ؾ� �ϹǷ�)
                    StartCoroutine(Reissue());
                    StartCoroutine(Ranking());
                }
                else
                {
                    print("��ŷ ��ȸ");
                    print(request.responseCode);
                    print(request.downloadHandler.text);
                    JArray response = JArray.Parse(request.downloadHandler.text);
                    print(response);
                    // 
                    int i = 0;
                    foreach (JObject jobj in response)
                    {
                        print(response[i]["clearTime"]);
                        print(response[i++]["userNickname"]);
                    }

                }

            }
            request.Dispose();
        }
    }

    private IEnumerator Reissue()
    {
        string url = baseUrl + "reissue";
        string userEmail = PlayerPrefs.GetString("userEmail");
        string refreshToken = "Bearer " + PlayerPrefs.GetString("refreshToken");
        ReissueRequest reissueRequest = new ReissueRequest(refreshToken, userEmail);
        string data = JsonConvert.SerializeObject(reissueRequest);
        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            request.uploadHandler.Dispose();
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                JObject response = JObject.Parse(request.downloadHandler.text);
                string message = response["message"].ToString();
                print(message);
                // message�� success�̸� 
                if ("success".Equals(message))
                {
                    string accessToken = response["accessToken"].ToString();
                    accessToken = accessToken.Replace("Bearer ", "");
                    // Playerprefs�� accesstoken �� �ٲ۴�.
                    print(PlayerPrefs.GetString("accessToken"));
                    PlayerPrefs.SetString("accessToken", accessToken);
                    print(PlayerPrefs.GetString("accessToken"));
                }
                else // �ƴϸ�
                {
                    print("���� �α׾ƿ�");
                    // �α׾ƿ�
                    PlayerPrefs.DeleteAll(); // ���� ���丮�� ���� ����
                    PhotonNetwork.Disconnect();
                    SceneManager.LoadScene("LogIn"); // ���� �������� �̵�
                }

            }
            request.Dispose();
        }
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