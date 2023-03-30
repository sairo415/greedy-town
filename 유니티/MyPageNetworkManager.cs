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
        // ���� ���� ��ȸ
        StartCoroutine(Userinfo());
        // ���� ������ ��ȸ
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
        Debug.Log("�κ񿬰�");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("�� ����");
        PhotonNetwork.Instantiate("TownPlayer", new Vector3(-25.88f, 5, -17.61119f), Quaternion.identity);

    }
    
    // 401 or 403
    public void CheckToken(UnityWebRequest request)
    {
        // ��ū ����Ǿ�����
        if (request.responseCode == 401)
        {
            // accesToken ��߱�
            StartCoroutine(Reissue());
        }
        else if (request.responseCode == 403)     // ���� �α׾ƿ� (403 �̸� �׳� �α׾ƿ�)
        {
            // api ���� ���� ������ ���� �α׾ƿ�
            PlayerPrefs.DeleteAll(); // ���� ���丮�� ���� ����
            SceneManager.LoadScene("LogIn"); // ���� �������� �̵�
        }
    }

    public void GetLogout()
    {
        StartCoroutine(Logout());
    }

    // ���� ���� ��ȸ
    private IEnumerator Userinfo()
    {
        string url = baseUrl + "user/info";
        print(url);
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
                    StartCoroutine(Userinfo());
                }
                else
                {
                    //print(request.responseCode);
                    print("���� ���� ��û ����");
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

    // ������ ������ ��ȸ
    private IEnumerator MyItem()
    {
        string url = baseUrl + "item/character-custom";
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
                    StartCoroutine(MyItem());
                }
                else
                {
                    print(request.responseCode);
                    print("�� ������ ��ȸ ����");
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    print(response);
                }

            }
            request.Dispose();
        }
    }

    // �α׾ƿ� ��û
    private IEnumerator Logout()
    {
        string url = baseUrl + "user/logout";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // header�� accessToken ���
            request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("accessToken"));
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if(request.isDone)
              {
                // accessToken ����Ǿ�����
                if (request.responseCode == 401) 
                {
                    print("��ū ����");
                    // accesToken ��߱� �� ��õ� (refreshToken �����ؾ� �ϹǷ�)
                    StartCoroutine(Reissue());
                    StartCoroutine(Logout());
                } else
                {
                    print("�α׾ƿ� ����");
                    PlayerPrefs.DeleteAll(); // ���� ���丮�� ���� ����
                    PhotonNetwork.Disconnect();
                    SceneManager.LoadScene("LogIn"); // ���� �������� �̵�
                }

           }
            request.Dispose();
        }
    }

    // refreshtoken�� email�� /reissue�� ������
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
                // message�� success�̸� 
                if("success".Equals(message))
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
