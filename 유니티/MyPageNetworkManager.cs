using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class MyPageNetworkManager : MonoBehaviourPunCallbacks
{

    public TMP_Text PlayersText;
    private bool check;

    private string baseUrl = "http://j8a808.p.ssafy.io:8080/";
//    private string baseUrl = "localhost:8080/";


    private void Start()
    {
        // 유저 정보 조회
        StartCoroutine(Userinfo());
        // 소유 아이템 조회
        StartCoroutine(MyItem());
        PhotonNetwork.JoinLobby();
        check = true;
    }
    public void ChangeScene()
    {
        if(check) SceneManager.LoadScene("Town");
    
    }


    public override void OnJoinedLobby()
    {
        Debug.Log("로비연결");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장");
        PhotonNetwork.Instantiate("TownPlayer", new Vector3(-25.88f, 5, -17.61119f), Quaternion.identity);

    }
    
    // 401 or 403
    public void CheckToken(UnityWebRequest request)
    {
        // 토큰 만료되었으면
        if (request.responseCode == 401)
        {
            // accesToken 재발급
            StartCoroutine(Reissue());
        }
        else if (request.responseCode == 403)     // 강제 로그아웃 (403 이면 그냥 로그아웃)
        {
            // api 접근 권한 없으면 강제 로그아웃
            PlayerPrefs.DeleteAll(); // 로컬 스토리지 정보 비우기
            SceneManager.LoadScene("LogIn"); // 시작 페이지로 이동
        }
    }

    public void GetLogout()
    {
        StartCoroutine(Logout());
    }

    // 유저 정보 조회
    private IEnumerator Userinfo()
    {
        string url = baseUrl + "user/info";
        print(url);
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
                    StartCoroutine(Userinfo());
                }
                else
                {
                    //print(request.responseCode);
                    print("유저 정보 요청 성공");
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    print(response);
                    LoginUser loginUser = JsonConvert.DeserializeObject<LoginUser>(request.downloadHandler.text);

                    print(loginUser.GetUserNickname());
                    print(loginUser.GetUserMoney());
                    print(loginUser.GetUserJoinDate());
                    PlayerPrefs.SetString("userNickname", loginUser.GetUserNickname());
                    PlayerPrefs.SetString("userMoney", loginUser.GetUserMoney().ToString());
                    PlayerPrefs.SetString("userJoinDate", loginUser.GetUserJoinDate().ToString());
                    print(PlayerPrefs.GetString("userNickname"));
                    print(PlayerPrefs.GetString("userMoney"));
                    print(PlayerPrefs.GetString("userJoinDate"));
                }       
            }
            request.Dispose();
        }
    }

    // 소유한 아이템 조회
    private IEnumerator MyItem()
    {
        string url = baseUrl + "item/character-custom";
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
                    StartCoroutine(MyItem());
                }
                else
                {
                    print(request.responseCode);
                    print("내 아이템 조회 성공");
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    print(response);
                }

            }
            request.Dispose();
        }
    }

    // 로그아웃 요청
    private IEnumerator Logout()
    {
        string url = baseUrl + "user/logout";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // header에 accessToken 담기
            request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("accessToken"));
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if(request.isDone)
              {
                // accessToken 만료되었으면
                if (request.responseCode == 401) 
                {
                    print("토큰 만료");
                    // accesToken 재발급 후 재시도 (refreshToken 삭제해야 하므로)
                    StartCoroutine(Reissue());
                    StartCoroutine(Logout());
                } else
                {
                    print("로그아웃 성공");
                    PlayerPrefs.DeleteAll(); // 로컬 스토리지 정보 비우기
                    PhotonNetwork.Disconnect();
                    SceneManager.LoadScene("LogIn"); // 시작 페이지로 이동
                }

           }
            request.Dispose();
        }
    }

    // refreshtoken과 email을 /reissue에 보내기
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

            if(request.isDone)
            {
                JObject response = JObject.Parse(request.downloadHandler.text);
                string message = response["message"].ToString();
                print(message);
                // message가 success이면 
                if("success".Equals(message))
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


}

public class LoginUser
{
    public string userNickname;
    public long userMoney;
    public DateTime userJoinDate;

    public LoginUser(string userNickname, long userMoney, DateTime userJoinDate)
    {
        this.userNickname = userNickname;
        this.userMoney = userMoney;
        this.userJoinDate = userJoinDate;
    }

    public string GetUserNickname()
    {
        return this.userNickname;
    }

    public long GetUserMoney()
    {
        return this.userMoney;
    }

    public DateTime GetUserJoinDate()
    {
        return this.userJoinDate;
    }
}
