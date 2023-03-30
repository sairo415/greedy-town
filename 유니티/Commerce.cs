using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Commerce : MonoBehaviourPunCallbacks
{

    public TMP_Text income; // 뱀서, 레이드에서 번 돈. 도박에서는 안 보이는 텍스트 만들어서 이득-투입 여기에 띄워두고 받아야 겠다.
    public Button incomeTest;
    private string baseUrl = "http://j8a808.p.ssafy.io:8080/";
    //private string baseUrl = "localhost:8080/";
    private int gameInfo;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PostEarn()
    {
        StartCoroutine(Earn());
    }

    public IEnumerator Earn()
    {
        string url = baseUrl + "user/money";

        // 보낼 정보
        // 씬 위치에 따라 gameinfo 숫자 설정
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name.Equals("Town")) // 카지노
        {
            gameInfo = 1;
        } else if(scene.name.Equals("레이드"))
        {
            gameInfo = 2;

        } else if(scene.name.Equals("뱀서"))
        {
            gameInfo = 3;
        }
        print(gameInfo);
        long getMoney = long.Parse(income.text); // 번 돈 설정
        print(getMoney);
        EarnRequest earnRequest = new EarnRequest(gameInfo, getMoney);
        string data = JsonConvert.SerializeObject(earnRequest);
        print(data);

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
                    StartCoroutine(Earn());
                }
                else
                {
                    byte[] results = request.downloadHandler.data;
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    string message = response["message"].ToString();
                    print(message);

                    // 성공하면 playerprefs에 money 저장
                    if ("success".Equals(message))
                    {
                        print("돈 벌기 성공");
                        string money = response["money"].ToString();
                        print(PlayerPrefs.GetString("money"));
                        PlayerPrefs.SetString("money", money);
                        print(PlayerPrefs.GetString("money"));
                    }
                    else
                    {
                        print("돈 벌기 실패");
                    }
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

public class EarnRequest
{
    public int gameInfo;
    public long money;

    public EarnRequest(int gameInfo, long money)
    {
        this.gameInfo = gameInfo;
        this.money = money;
    }
}
