using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using Photon.Pun;

public class ShopItemManager : MonoBehaviour
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
        weaponNum = 100;
        sheildNum = 100;
        dressNum = 0;
        backNum = 100;



        //test


        /*       int[] hats = { 1, 2, 3, 4, 8 };

               for (int i = 0; i < hats.Length; i++)
               {
                   HatSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HatSlots").GetChild(hats[i]).gameObject.SetActive(true);
               }
       */

        StartCoroutine(Market());
    }

    public void GetMarket()
    {
        // 모든 아이템 보이게 처리 후
        StartCoroutine(Market());
    }

    // 상점 조회 : seq, price, name, image, type 필요
    // type 별로 조회하게 하기
    // seq, image, name, price 세팅
    private IEnumerator Market()
    {
        string url = baseUrl + "item/character-custom"; // 내 아이템 조회
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
                    StartCoroutine(Market());
                }
                else
                {
                    print("상점 왔다");
                    print(request.responseCode);
                    print(request.downloadHandler.text);
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    JArray itemResponse = JArray.Parse(response["userItems"].ToString());
                    JArray wearingResponse = JArray.Parse(response["wearingDtos"].ToString());
                    print(response);
                    // 이미지 여러 장 겹쳐두고
                    foreach (JObject jobj in itemResponse)
                    {
                        // jobj["itemSeq"]를 찾아서(Find) 안 보이게(또는 흐리게) 처리 
                        int itemSeq = int.Parse(jobj["itemSeq"].ToString());
                        if(itemSeq <= 13)
                        {
                            HatSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HatSlots").GetChild(itemSeq).gameObject.SetActive(false);
                        } else if (itemSeq <= 33)
                        {
                            itemSeq -= 13;
                            HeadSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HeadSlot").GetChild(itemSeq).gameObject.SetActive(false);
                        } else if(itemSeq <= 72)
                        {
                            itemSeq -= 33;
                            AcsSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("AcsSlot").GetChild(itemSeq).gameObject.SetActive(false);
                        } else if(itemSeq <= 85)
                        {
                            itemSeq -= 72;
                            HairSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("HairSlot").GetChild(itemSeq).gameObject.SetActive(false);
                        } else if(itemSeq <= 115)
                        {
                            itemSeq -= 85;
                            WeaponSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("WeaponSlot").GetChild(itemSeq).gameObject.SetActive(false);
                        } else if(itemSeq <= 135)
                        {
                            itemSeq -= 115;
                            SheildSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("SheildSlot").GetChild(itemSeq).gameObject.SetActive(false);
                        } else if(itemSeq <= 155)
                        {
                            itemSeq -= 135;
                            DressSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("DressSlot").GetChild(itemSeq).gameObject.SetActive(false);
                        } else
                        {
                            itemSeq -= 155;
                            BackSlot.GetComponent<Transform>().Find("Viewport").Find("Content").Find("BackSlot").GetChild(itemSeq).gameObject.SetActive(false);
                        }
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
                if (num == 39) { hairNum = 100; break; }
                player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("neck_01").Find("head").GetChild(num).gameObject.SetActive(true);
                acsNum = num;
                break;
            case 4:
                if (weaponNum != 100) player.Find("root").Find("pelvis").Find("spine_01").Find("spine_02").Find("spine_03").Find("clavicle_r").Find("upperarm_r").Find("lowerarm_r").Find("hand_r").Find("weapon_r").GetChild(weaponNum+1).gameObject.SetActive(false);
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
}
