using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    float hAxis;
    float vAxis;

    private Vector3 start = new Vector3(-18, 5, -12);

    private string baseUrl = "http://j8a808.p.ssafy.io:8080/";
    //    private string baseUrl = "localhost:8080/";

    // 회피 : space
    bool jDown;

    // 회피 여부
    bool isDodge;

    // 벽 충돌 플래그 bool 변수를 생성
    bool isBorder;

    Vector3 moveVec;

    // 회피 도중 방향 조작 못하도록
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

        Debug.Log(PlayerPrefs.GetInt("dressNum"));
        Debug.Log(PlayerPrefs.GetInt("backNum"));
        Debug.Log(PlayerPrefs.GetInt("sheildNum"));
        Debug.Log(PlayerPrefs.GetInt("weaponNum"));
        Debug.Log(PlayerPrefs.GetInt("acsNum"));
        Debug.Log(PlayerPrefs.GetInt("hairNum"));
        Debug.Log(PlayerPrefs.GetInt("headNum"));
        Debug.Log(PlayerPrefs.GetInt("hatNum"));
        
        
        transform.GetChild(PlayerPrefs.GetInt("dressNum")).gameObject.SetActive(true);
        if(PlayerPrefs.GetInt("backNum") < 3)
        {
            transform.GetChild(PlayerPrefs.GetInt("backNum")+20).gameObject.SetActive(true);
        } else
        {
            transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(PlayerPrefs.GetInt("backNum")-3).gameObject.SetActive(true);
        }
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(PlayerPrefs.GetInt("sheildNum") + 17).gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(PlayerPrefs.GetInt("weaponNum") + 1).gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("acsNum")).gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("hairNum") + 63).gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("headNum") + 76).gameObject.SetActive(true);
        transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("hatNum") + 96).gameObject.SetActive(true);
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

        // 추락하면 리스폰
        if (this.gameObject.transform.position.y < -50)
        {
            this.gameObject.transform.position = start;
        }


    }
    void OnCollisionEnter(Collision col)
    {
        // 배에 닿으면 보스레이드 로비로 이동
        if (col.gameObject.name == "Boat")
        {
            ToTheBossLobby();
        }
        // 랭킹 표지판에 가면 랭킹 조회
        else if(col.gameObject.name == "PinnedWall")
        {
            StartCoroutine(Ranking());
        }
        // 열기구에 가면 뱀서로 이동
        else if(col.gameObject.name == "HotAirBalloon_Blue")
        {
            print("뱀서 간다");
            SceneManager.LoadScene("Vamsu-LSJ");
        }
        // 상점으로 가면 상점으로 이동
        else if (col.gameObject.name == "Magic_Shop")
        {
            print("상점 간다");
            SceneManager.LoadScene("Market");
        }
        // 키오스크로 가면 카지노로 이동
        else if (col.gameObject.name == "Kiosk_Shop")
        {
            print("카지노 간다");
            SceneManager.LoadScene("Casino");
        }
        // 경찰서로 가면 PVP로 이동
        else if (col.gameObject.name == "PoliceStation_1Light")
        {
            print("PVP 간다");
        }
    }

    public void Alert(string message)
    {

    }

    public void ToTheBossLobby()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LeaveRoom();
    }


    // 랭킹
    private IEnumerator Ranking()
    {
        string url = baseUrl + "social/ranking";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // header에 accessToken 담기
            request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("accessToken"));
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                // accessToken 만료되었으면
                if (request.responseCode == 401)
                {
                    print("토큰 만료");
                    // accesToken 재발급 후 재시도 (refreshToken 삭제해야 하므로)
                    StartCoroutine(Reissue());
                    StartCoroutine(Ranking());
                }
                else
                {
                    print("랭킹 조회");
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
                // message가 success이면 
                if ("success".Equals(message))
                {
                    string accessToken = response["accessToken"].ToString();
                    accessToken = accessToken.Replace("Bearer ", "");
                    // Playerprefs의 accesstoken 값 바꾼다.
                    print(PlayerPrefs.GetString("accessToken"));
                    PlayerPrefs.SetString("accessToken", accessToken);
                    print(PlayerPrefs.GetString("accessToken"));
                }
                else // 아니면
                {
                    print("강제 로그아웃");
                    // 로그아웃
                    PlayerPrefs.DeleteAll(); // 로컬 스토리지 정보 비우기
                    PhotonNetwork.Disconnect();
                    SceneManager.LoadScene("LogIn"); // 시작 페이지로 이동
                }

            }
            request.Dispose();
        }
    }

    void GetInput()
    {
        // GetAxisRaw() : Axis 값을 정수로 변환
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        // space 를 누르는 그 순간만 뛰도록 GetButtonDown 사용
        jDown = Input.GetButtonDown("Jump");
        // 기본적으로 마우스 왼쪽에 Fire1 이 들어가 있음
        // fDown = Input.GetButton("Fire1");
    }

    void Move()
    {
        // normalized
        // 대각선이라고 속도 빨라지지 않게.
        // 방향 값이 1로 보정된 벡터

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 회피를 하고 있다면
        if (isDodge)
            moveVec = dodgeVec;

        // 벽을 뚫고 못지나가게
        if (!isBorder)
        {
            transform.position += moveVec * speed * Time.deltaTime;
        }

        // 애니메이션
        anim.SetBool("isRun", moveVec != Vector3.zero);
    }

    void Turn()
    {
        // 플레이어 회전 (움직이는 방향으로 바라본다)
        transform.LookAt(transform.position + moveVec);

    }

    void Dodge()
    {// 벽을 뚫고 못지나가게
        if (jDown && moveVec != Vector3.zero && !isDodge && !isBorder)
        {
            // 움직임 벡터 -> 회피방향 벡터로 바뀌도록 구현
            dodgeVec = moveVec;
            speed *= 2.0f;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 시간차 함수 호출 0.5 초
            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    //자동 회전 방지
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        // Raycast() : Ray 를 쏘아 닿는 오브젝트를 감지하는 함수
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