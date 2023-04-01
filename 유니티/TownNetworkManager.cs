using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TownNetworkManager : MonoBehaviourPunCallbacks
{

    public TMP_Text PlayersText;
    private Vector3 start = new Vector3(-18, 10, -12);

    private string baseUrl = "http://j8a808.p.ssafy.io:8080/";
    //    private string baseUrl = "localhost:8080/";

    private void Start()
    {

        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
       
    }

    public void ToTheBossLobby()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "Town")
        {

            SceneManager.LoadScene("BossLobby");

            return;
        }
    }



    public override void OnConnectedToMaster()
    {

        Debug.Log("타운 룸으로 입장!");

        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        
        PhotonNetwork.Instantiate("TownPlayer", start, Quaternion.Euler(0, 180, 0));
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
                    print(response[0]["clearTime"]);
                    print(response[0]["userNickname"]);
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

    // 뱀서
    public IEnumerator ClearTime()
    {
        string url = baseUrl + "user/stat";

        Dictionary<string, string> clearTime = new Dictionary<string, string>();
        clearTime.Add("userClearTime", PlayerPrefs.GetString("userClearTime")); // userClearTime에 뱀서 클리어 타임 입력 필요 ("HH:mm:ss")
        string data = JsonConvert.SerializeObject(clearTime);

        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
            // header에 accessToken 담기
            request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("accessToken"));

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            request.uploadHandler.Dispose();
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler.Dispose();
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            print(request.responseCode);

            if (request.isDone)
            {
                // accessToken 만료되었으면
                if (request.responseCode == 401)
                {
                    print("토큰 만료");
                    // accesToken 재발급 후 재시도 (refreshToken 삭제해야 하므로)
                    StartCoroutine(Reissue());
                    StartCoroutine(ClearTime());
                }
                else
                {
                    print("뱀서 클리어타임 업데이트");
                    byte[] results = request.downloadHandler.data;
                    print(request.responseCode);
                    print(request.downloadHandler.text);
                    JObject response = JObject.Parse(request.downloadHandler.text); 
                    print(response);
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

}
