using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class VamsuGameManager : MonoBehaviourPunCallbacks
{
    public static VamsuGameManager instance;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime;

    [Header("# Player Info")]
    public float health;
    public float maxHealth;
    public int level;
    public int kill;
    public float exp;
    //각 레벨당 요구량
    public int[] nextExp;
    public float gold = 0;

    [Header("# Game Object")]
    public PoolManager pool;
    public VamsuPlayer player;
    public GameObject canvas;
    public LevelUp uiLevelUp;

    //10퍼센트 -> 0.1로 치환. 백분율이 추가되는 방식
    [Header("# Player Stat")]
    public float extraDamage;//ok
    public float extraCoolDown;
    public float extraArmor;//ok
    public float extraExp;//ok
    public float extraGold;
    public float extraHealth;//ok
    public float extraSpeed;//ok

    private string baseUrl = "http://j8a808.p.ssafy.io:8080/";
    //    private string baseUrl = "localhost:8080/";

    private void Awake()
    {
        //백엔드 호출 -> 플레이어 옷 설정

        Time.timeScale = 0;
        instance = this;
        maxHealth = 120 * (1 + extraHealth);
        health = maxHealth;
        player.speed *= (1 + extraSpeed);
        maxGameTime = 30 * 5 * 6f;

        nextExp = new int[30];
        for (int i = 0; i < 30; i++)
        {
            nextExp[i] = 5 + i * 15;
        }
    }
    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        player.transform.GetChild(PlayerPrefs.GetInt("dressNum")).gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("backNum") != 100)
        {
            if (PlayerPrefs.GetInt("backNum") < 3)
            {
                player.transform.GetChild(PlayerPrefs.GetInt("backNum") + 20).gameObject.SetActive(true);
            }
            else
            {
                player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(PlayerPrefs.GetInt("backNum") - 3).gameObject.SetActive(true);
            }
        }
        if (PlayerPrefs.GetInt("sheildNum") != 100)
        {
            player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(PlayerPrefs.GetInt("sheildNum") + 17).gameObject.SetActive(true);
        }
        player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(PlayerPrefs.GetInt("weaponNum") + 1).gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("acsNum") != 100)
        {
            player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("acsNum")).gameObject.SetActive(true);
        }
        if (PlayerPrefs.GetInt("hairNum") != 100)
        {
            player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("hairNum") + 63).gameObject.SetActive(true);
        }
        player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("headNum") + 76).gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("hatNum") != 100)
        {
            player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(PlayerPrefs.GetInt("hatNum") + 96).gameObject.SetActive(true);
        }
        player.transform.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").Find("Eyebrow02").gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isLive)
            return;



        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            //game win
            canvas.transform.Find("Victory").gameObject.SetActive(true);
            canvas.transform.Find("Restart").gameObject.SetActive(true);
            StartCoroutine(transform.GetComponent<Commerce>().Earn((long)gold));
            StartCoroutine(ClearTime(gameTime));
            Time.timeScale = 0;
        }

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 10f;
        }*/
    }

    public void GetExp()
    {
        exp += (1 + extraExp);
        if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            health += (maxHealth / 5);
            if (health > maxHealth)
                health = maxHealth;
            //레벨업 로직
            uiLevelUp.Show();
        }
    }

    public void GetDamage(float damage)
    {
        health -= (damage * (1 - extraArmor));
        if (health <= 0)
        {
            //gameOver
            canvas.transform.Find("Dead").gameObject.SetActive(true);

            //얻은 금화 처리, 시간 저장 백엔드 API 호출 -> gold, gameTime 변수 사용
            StartCoroutine(transform.GetComponent<Commerce>().Earn((long)gold));
            StartCoroutine(ClearTime(gameTime));

            canvas.transform.Find("Restart").gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void GameStart()
    {
        Resume();

        canvas.transform.Find("Title").gameObject.SetActive(false);
        canvas.transform.Find("Start").gameObject.SetActive(false);
        canvas.transform.Find("HUD").gameObject.SetActive(true);

        //총 선택
        uiLevelUp.Select(1);

        //uiLevelUp.Show();
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }

    public void TotheTown()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Town");
    }
    // 뱀서
    public IEnumerator ClearTime(float clearSeconds)
        {
        string url = baseUrl + "user/stat";

        string hhmmssTime = string.Format("{0:D2}", Mathf.FloorToInt(clearSeconds / 60)) + ":" + string.Format("{0:D2}", Mathf.FloorToInt(clearSeconds % 60));

        Debug.Log(hhmmssTime);

        Dictionary<string, string> clearTime = new Dictionary<string, string>();
        clearTime.Add("userClearTime", hhmmssTime); // userClearTime에 뱀서 클리어 타임 입력 필요 ("HH:mm:ss")
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
                    StartCoroutine(ClearTime(clearSeconds));
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
