using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System;

public class ItemManager : MonoBehaviour
{
    //슬롯 버튼들

    public GameObject Hat;
    public GameObject Hair;
    public GameObject Head;
    public GameObject Acs;
    public GameObject Weapon;
    public GameObject Shield;
    public GameObject Body;
    public GameObject Back;

    //슬롯들
    public GameObject HatSlot;
    public GameObject HeadSlot;
    public GameObject AcsSlot;
    public GameObject HairSlot;
    public GameObject WeaponSlot;
    public GameObject SheildSlot;
    public GameObject DressSlot;
    public GameObject BackSlot;

    //현재 활성화된 슬롯
    private int ActiveSlot;

    public Transform player;

    //현재 입은 아이템 num
    private int hatNum;
    private int headNum;
    private int acsNum;
    private int hairNum;
    private int weaponNum;
    private int sheildNum;
    private int dressNum;
    private int backNum;

    private string baseUrl = "http://j8a808.p.ssafy.io:8080/";
    //    private string baseUrl = "localhost:8080/";

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.JoinLobby();
        // 유저 정보 조회
        StartCoroutine(Userinfo());

        ActiveSlot = 0; //처음엔 모자가 엑티스 슬롯
        player = player.GetComponent<Transform>();

        HatSlot.SetActive(true);
        HeadSlot.SetActive(false);
        AcsSlot.SetActive(false);
        HairSlot.SetActive(false);
        WeaponSlot.SetActive(false);
        SheildSlot.SetActive(false);
        DressSlot.SetActive(false);
        BackSlot.SetActive(false);

        //여기에 지금 입고 있는 옷들을 넣어줘야함!
        hatNum = 100;
        headNum = 0;
        acsNum = 100;
        hairNum = 100;
        weaponNum = 0;
        sheildNum = 100;
        dressNum = 0;
        backNum = 100;

        // 시작 시 모든 아이템 다 끄고 가지고 있는 거만 켜줘야 함
        for (int i = 0; i <= 13; i++)
        {
            HatSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HatSlots").GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i <= 19; i++)
        {
            HeadSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HeadSlots").GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i<= 38; i++)
        {
            AcsSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("AcsSlots").GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i<= 12; i++)
        {
            HairSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HairSlots").GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i<= 29; i++)
        {
            WeaponSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("WeaponSlots").GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i<= 19; i++)
        {
            SheildSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("SheildSlots").GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i<= 19; i++)
        {
            DressSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("BodySlots").GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i<= 5; i++)
        {
            BackSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("BackSlots").GetChild(i).gameObject.SetActive(false);
        }

        StartCoroutine(MyItem());

        //test


 /*       int[] hats = { 1, 2, 3, 4, 8 };

        for (int i = 0; i < hats.Length; i++)
        {
            HatSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HatSlots").GetChild(hats[i]).gameObject.SetActive(true);
        }
*/

    }



    public void ClickItem(Button Item)

    {   //모자 96부터 109까지
        Transform item = Item.GetComponent<Transform>();
        int num = item.GetSiblingIndex();

        switch (ActiveSlot) {
            case 0:
                //벗어주기
                if (hatNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(hatNum + 96).gameObject.SetActive(false);
                //착용안함 (그냥 벗기)
                if (num == 15){ hatNum = 100; break; }
                //입어주기
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num + 96).gameObject.SetActive(true);
                hatNum = num;
                break;
            case 1:
                //벗어주기
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(headNum + 76).gameObject.SetActive(false);
                //착용 안함이면 0번째 머리로 입어주기           
                if(num == 20) num = 0; 
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num + 76).gameObject.SetActive(true);
                headNum = num;
                Debug.Log("몇번이길래.." + num);
                break;
            case 2:
                if (hairNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(hairNum + 63).gameObject.SetActive(false);
                if (num == 13) { hairNum = 100; break; }
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num + 63).gameObject.SetActive(true);
                hairNum = num;
                break;
            case 3:
                if (acsNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(acsNum).gameObject.SetActive(false);
                if (num == 39) { acsNum = 100; break; }
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num).gameObject.SetActive(true);
                acsNum = num;
                break;
            case 4:
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(weaponNum+1).gameObject.SetActive(false);
                if (num == 30) num = 0;
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(num+1).gameObject.SetActive(true);
                weaponNum = num;
                break;
            case 5:
                if (sheildNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(sheildNum+17).gameObject.SetActive(false);
                if (num == 20) { sheildNum = 100; break; }
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_l").Find("upperarm_l").Find("lowerarm_l").Find("hand_l").Find("weapon_l").GetChild(num+17).gameObject.SetActive(true);
                sheildNum = num;
                break;
            case 6:
                player.GetChild(dressNum).gameObject.SetActive(false);
                if (num == 20) num = 0;
                player.GetChild(num).gameObject.SetActive(true);
                dressNum = num;
                break;
            case 7:
                //망토
                if (num < 3)
                {
                    //원래 가방이었는지 망토였는지 확인

                    if (backNum != 100)
                    {
                        //원래 망토였으면,
                        if (backNum < 3) player.GetChild(backNum + 20).gameObject.SetActive(false);
                        else player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(backNum - 3).gameObject.SetActive(false);
                    }
                    player.GetChild(num + 20).gameObject.SetActive(true);
                }
                //가방
                else
                {

                    if (backNum != 100)
                    {
                        if (backNum < 3) player.GetChild(backNum + 20).gameObject.SetActive(false);
                        else player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(backNum - 3).gameObject.SetActive(false);
                    }
                    //착용안함
                    if (num == 6) {backNum = 100; break; }
                    
                    player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("BackpackBone").GetChild(num-3).gameObject.SetActive(true);
                }

                backNum = num;
                break;
        }

        
    
    }



    public void OnClickParts(GameObject input)
    {
        if (input.GetComponent<Transform>().GetSiblingIndex() == 7)
           player.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        else player.transform.rotation = Quaternion.Euler(0, 158.131f, 0);
            
        
        HatSlot.SetActive(false);
        HeadSlot.SetActive(false);
        AcsSlot.SetActive(false);
        HairSlot.SetActive(false);
        WeaponSlot.SetActive(false);
        SheildSlot.SetActive(false);
        DressSlot.SetActive(false);
        BackSlot.SetActive(false);

        input.SetActive(true);
        //현재 활성화된 슬롯 넘버 저장
        Transform it = input.GetComponent<Transform>();
        ActiveSlot = it.GetSiblingIndex();


    }

    public IEnumerator MyItem()
    {
        Debug.Log("MyItem() 호출");
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
                    print(request.downloadHandler.text);
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    JArray response2 = JArray.Parse(response["userItems"].ToString());
                    JArray response3 = JArray.Parse(response["wearingDtos"].ToString());
                    print(response);
                    // 이미지 여러 장 겹쳐두고
                    foreach (JObject jobj in response2)
                    {
                        // jobj["itemSeq"]를 찾아서(Find) 보이게(또는 흐리게) 처리 
                        int itemSeq = int.Parse(jobj["itemSeq"].ToString());
                        Debug.Log(itemSeq);
                        if (itemSeq <= 14)
                        {
                            HatSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HatSlots").GetChild(itemSeq).gameObject.SetActive(true);
                        }
                        else if (itemSeq <= 34)
                        {
                            itemSeq -= 15;
                            HeadSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HeadSlots").GetChild(itemSeq).gameObject.SetActive(true);
                        }
                        else if (itemSeq <= 73)
                        {
                            itemSeq -= 35;
                            Debug.Log("악세사리에서 index : " + itemSeq);
                            AcsSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("AcsSlots").GetChild(itemSeq).gameObject.SetActive(true);
                        }
                        else if (itemSeq <= 86)
                        {
                            itemSeq -= 74;
                            HairSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HairSlots").GetChild(itemSeq).gameObject.SetActive(true);
                        }
                        else if (itemSeq <= 116)
                        {
                            itemSeq -= 87;
                            WeaponSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("WeaponSlots").GetChild(itemSeq).gameObject.SetActive(true);
                        }
                        else if (itemSeq <= 136)
                        {
                            itemSeq -= 117;
                            SheildSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("SheildSlots").GetChild(itemSeq).gameObject.SetActive(true);
                        }
                        else if (itemSeq <= 156)
                        {
                            itemSeq -= 137;
                            DressSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("BodySlots").GetChild(itemSeq).gameObject.SetActive(true);
                        }
                        else
                        {
                            itemSeq -= 157;
                            BackSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("BackSlots").GetChild(itemSeq).gameObject.SetActive(true);
                        }
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
                    PlayerPrefs.SetString("money", loginUser.GetUserMoney().ToString());
                    PlayerPrefs.SetString("userJoinDate", loginUser.GetUserJoinDate().ToString());
                    print(PlayerPrefs.GetString("userNickname"));
                    print(PlayerPrefs.GetString("money"));
                    print(PlayerPrefs.GetString("userJoinDate"));
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

            if (request.isDone)
            {
                // accessToken 만료되었으면
                if (request.responseCode == 401)
                {
                    print("토큰 만료");
                    // accesToken 재발급 후 재시도 (refreshToken 삭제해야 하므로)
                    StartCoroutine(Reissue());
                    StartCoroutine(Logout());
                }
                else
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

    // 커스터마이징 : patch 요청, itemDto에 itemSeq 세팅
    public IEnumerator Custom()
    {
        string url = baseUrl + "item/character-custom";

        // 보낼 정보
        List<Dictionary<string, Dictionary<string, int>>> list = new List<Dictionary<string, Dictionary<string, int>>>() { };

        // 8개 데이터 넣기
        if(hatNum != 100)
        {
            list.Add(new Dictionary<string, Dictionary<string, int>>()
            {
                { "itemDto", new Dictionary<string, int>(){ { "itemSeq", hatNum+1 } } }
            });
        }
        if(headNum != 100)
        {
            list.Add(new Dictionary<string, Dictionary<string, int>>()
            {
                { "itemDto", new Dictionary<string, int>(){ { "itemSeq", headNum + 15 } } }
            });

        }
        if(acsNum != 100)
        {
            list.Add(new Dictionary<string, Dictionary<string, int>>()
            {
                { "itemDto", new Dictionary<string, int>(){ { "itemSeq", acsNum + 35 } } }
            });

        }
        if(hairNum != 100)
        {
            list.Add(new Dictionary<string, Dictionary<string, int>>()
            {
                { "itemDto", new Dictionary<string, int>(){ { "itemSeq", hairNum + 74 } } }
            });

        } 
        if(weaponNum != 100)
        {
            list.Add(new Dictionary<string, Dictionary<string, int>>()
            {
                { "itemDto", new Dictionary<string, int>(){ { "itemSeq", weaponNum + 87 } } }
            });

        }
        if(sheildNum != 100)
        {
            list.Add(new Dictionary<string, Dictionary<string, int>>()
            {
                { "itemDto", new Dictionary<string, int>(){ { "itemSeq", sheildNum + 117 } } }
            });

        }
        if(dressNum != 100)
        {
            list.Add(new Dictionary<string, Dictionary<string, int>>()
            {
                { "itemDto", new Dictionary<string, int>(){ { "itemSeq", dressNum + 137 } } }
            });

        }
        if(backNum != 100)
        {
            list.Add(new Dictionary<string, Dictionary<string, int>>()
            {
                { "itemDto", new Dictionary<string, int>(){ { "itemSeq", backNum + 157 } } }
            });

        }

        PlayerPrefs.SetInt("hatNum", hatNum);
        PlayerPrefs.SetInt("headNum ", headNum);
        PlayerPrefs.SetInt("acsNum ", acsNum);
        PlayerPrefs.SetInt("hairNum ", hairNum);
        PlayerPrefs.SetInt("weaponNum ", weaponNum);
        PlayerPrefs.SetInt("sheildNum ", sheildNum);
        PlayerPrefs.SetInt("dressNum ", dressNum);
        PlayerPrefs.SetInt("backNum ", backNum);

        Debug.Log(PlayerPrefs.GetInt("dressNum"));
        Debug.Log(PlayerPrefs.GetInt("backNum"));
        Debug.Log(PlayerPrefs.GetInt("sheildNum"));
        Debug.Log(PlayerPrefs.GetInt("weaponNum"));
        Debug.Log(PlayerPrefs.GetInt("acsNum"));
        Debug.Log(PlayerPrefs.GetInt("hairNum"));
        Debug.Log(PlayerPrefs.GetInt("headNum"));
        Debug.Log(PlayerPrefs.GetInt("hatNum"));

        Debug.Log("---------------------");

        Debug.Log(dressNum);
        Debug.Log(backNum);
        Debug.Log(sheildNum);
        Debug.Log(weaponNum);
        Debug.Log(acsNum);
        Debug.Log(hairNum);
        Debug.Log(headNum);
        Debug.Log(hatNum);

        //WearingDto wearingDto = new WearingDto(1, itemDto);

        string data = JsonConvert.SerializeObject(list);
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
                    StartCoroutine(Custom());
                }
                else
                {
                    byte[] results = request.downloadHandler.data;
                    JObject response = JObject.Parse(request.downloadHandler.text);

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
