using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.IO;

public class Login : MonoBehaviourPunCallbacks
{
    public Text userEmail;
    public Text userPassword;
    public Button signInBtn;
    public Button signUpBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Signin()
    {
        print("signin 호출");
        string url = "http://localhost:8080/login";

        // 로그인 정보
        LoginRequest loginRequest = new LoginRequest(userEmail.text, userPassword.text);
        print(loginRequest.getUserEmail());
        print(loginRequest.getUserPassword());
        /*
        Dictionary<string, string> loginRequest = new Dictionary<string, string>();
        loginRequest.Add("userEmail", userEmail.text);
        loginRequest.Add("userPassword", userPassword.text);
        */

        // post 요청
        string data = JsonUtility.ToJson(loginRequest);
        print(data);
        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
       
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest(); // 응답 대기

            print(request.responseCode);
        
            if (request.responseCode == 200)
            {
            string authorization = request.GetResponseHeader("authorization");
            print(authorization);
            string[] tokens = authorization.Split("_AND_");
            string accessToken = tokens[0].Replace("Bearer ", "");
            string refreshToken = tokens[1].Replace("Bearer ", "");
            print(accessToken);
            print(refreshToken);
            PlayerPrefs.SetString("accessToken", accessToken);
            PlayerPrefs.SetString("refreshToken", refreshToken);
            // PhotonNetwork.ConnectUsingSettings();

            }
            else if (request.responseCode == 401)
            {
            Debug.Log("비밀번호를 확인해주세요");
            }
            else
            {
            Debug.Log("회원가입 하시겠습니까?");
            }

        }
    }
    public void Connect()
    {
        StartCoroutine(Signin());

        if(PlayerPrefs.GetString("accessToken") != null)
        {
            Debug.Log("연결");
            Debug.Log(PlayerPrefs.GetString("accessToken"));
            Debug.Log(PlayerPrefs.GetString("refreshToken"));
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속");
    }
}
