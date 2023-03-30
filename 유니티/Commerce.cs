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

    public TMP_Text income; // �켭, ���̵忡�� �� ��. ���ڿ����� �� ���̴� �ؽ�Ʈ ���� �̵�-���� ���⿡ ����ΰ� �޾ƾ� �ڴ�.
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

        // ���� ����
        // �� ��ġ�� ���� gameinfo ���� ����
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name.Equals("Town")) // ī����
        {
            gameInfo = 1;
        } else if(scene.name.Equals("���̵�"))
        {
            gameInfo = 2;

        } else if(scene.name.Equals("�켭"))
        {
            gameInfo = 3;
        }
        print(gameInfo);
        long getMoney = long.Parse(income.text); // �� �� ����
        print(getMoney);
        EarnRequest earnRequest = new EarnRequest(gameInfo, getMoney);
        string data = JsonConvert.SerializeObject(earnRequest);
        print(data);

        using (UnityWebRequest request = UnityWebRequest.Post(url, data))
        {
            // header�� accessToken ���
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
                // accessToken ����Ǿ�����
                if (request.responseCode == 401)
                {
                    print("��ū ����");
                    // accesToken ��߱� �� ��õ� (refreshToken �����ؾ� �ϹǷ�)
                    StartCoroutine(Reissue());
                    StartCoroutine(Earn());
                }
                else
                {
                    byte[] results = request.downloadHandler.data;
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    string message = response["message"].ToString();
                    print(message);

                    // �����ϸ� playerprefs�� money ����
                    if ("success".Equals(message))
                    {
                        print("�� ���� ����");
                        string money = response["money"].ToString();
                        print(PlayerPrefs.GetString("money"));
                        PlayerPrefs.SetString("money", money);
                        print(PlayerPrefs.GetString("money"));
                    }
                    else
                    {
                        print("�� ���� ����");
                    }
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
