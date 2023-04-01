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
//    private string baseUrl = "localhost:8080/";
    private int gameInfo;

    public Button[] buttons;


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

    public void GetMyItem()
    {
        // 모든 아이템 안 보이게 처리 후
        StartCoroutine(MyItem());
    }
    public void GetMarket()
    {
        // 모든 아이템 보이게 처리 후
        StartCoroutine(Market());
    }

    // 아이템 구매 : response가 소유한 아이템 조회와 똑같다. : itemSeq, itemPrice 
    public IEnumerator Buy()
    {
        string url = baseUrl + "item/market";

        long itemSeq = 1; // 임시 : 어떻게 받아옴? : 클릭한 버튼의 seq 가져오기
        int itemPrice = 1000; // 임시 : 클릭한 버튼의 가격 가져오기
        BuyRequest buyRequest = new BuyRequest(itemSeq, itemPrice);
        string data = JsonConvert.SerializeObject(buyRequest);

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
                    StartCoroutine(Buy());
                }
                else
                {
                    print("아이템 구매");
                    byte[] results = request.downloadHandler.data;
                    print(request.responseCode);
                    print(request.downloadHandler.text);
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    JArray response2 = JArray.Parse(response["userItems"].ToString());
                    JArray response3 = JArray.Parse(response["wearingDtos"].ToString());
                    print(response);
                    print(response[0]["itemSeq"]);
                    // 이미지 여러 장 겹쳐두고
                    foreach (JObject jobj in response2)
                    {
                        // jobj["itemSeq"]를 찾아서(Find) 보이게(또는 흐리게) 처리 
                    }
                    PlayerPrefs.SetString("money", response["userMoney"].ToString());
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
                    print(request.downloadHandler.text);
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    JArray response2 = JArray.Parse(response["userItems"].ToString());
                    JArray response3 = JArray.Parse(response["wearingDtos"].ToString());
                    print(response);
                    print(response[0]["itemSeq"]);
                    // 이미지 여러 장 겹쳐두고
                    foreach (JObject jobj in response2)
                    {
                        // jobj["itemSeq"]를 찾아서(Find) 보이게(또는 흐리게) 처리 
                    }

                }

            }
            request.Dispose();
        }
    }

    // 커스터마이징 : patch 요청, itemDto에 itemSeq 세팅
    public IEnumerator Custom()
    {
        string url = baseUrl + "itme/character-custom";

        // 보낼 정보
        List<ItemDto> list = new List<ItemDto>(); // 백에서 wearingDto로 받는 거
        // 반복문 돌면서 데이터 넣기 : 이거 어디서 받아옴?
        for(int i=0; i<8; i++)
        {
            ItemDto itemDto = new ItemDto();
            ItemBuilder itemBuilder = new ItemBuilder();
            //itemBuilder.SetItemSeq(); // 해당 좌표의 seq 텍스트를 long으로 변환?
            list.Add(itemDto);

        }
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

    // 상점 조회 : seq, price, name, image, type 필요
    // type 별로 조회하게 하기
    // seq, image, name, price 세팅
    private IEnumerator Market()
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
                    StartCoroutine(Market());
                }
                else
                {
                    print("상점 왔다");
                    print(request.responseCode);
                    print(request.downloadHandler.text);
                    JArray response = JArray.Parse(request.downloadHandler.text);
                    print(response);
                    print(response[0]["itemSeq"]);
                    // 이미지 여러 장 겹쳐두고
                    foreach(JObject jobj in response)
                    {
                        // jobj["itemSeq"]를 찾아서(Find) 안 보이게(또는 흐리게) 처리 
                    }


                }


                    

                

            }
            request.Dispose();
        }
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

public class ItemDto
{
    public long itemSeq;
    public string itemName;
    public int itemPrice;

    public string itemImage;

    public int itemTypeSeq;
    public string itemTypeName;

    public long achievementsSeq;

    public int itemColorSeq;
    public string itemColorName;
    public ItemDto(long itemSeq, string itemName, int itemPrice, string itemImage, long achievementsSeq, int itemColorSeq, string itemColorName, int itemTypeSeq, string itemTypeName)
    {
        this.itemSeq = itemSeq;
        this.itemName = itemName;
        this.itemPrice = itemPrice;
        this.itemImage = itemImage;
        this.achievementsSeq = achievementsSeq;
        this.itemColorSeq = itemColorSeq;
        this.itemColorName = itemColorName;
        this.itemTypeSeq = itemTypeSeq;
        this.itemTypeName = itemTypeName;
    }
    public ItemDto()
    {

    }

    public long GetItemSeq()
    {
        return itemSeq;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public int GetItemPrice()
    {
        return itemPrice;
    }

    public string GetItemImage()
    {
        return itemImage;
    }

    public int GetItemTypeSeq()
    {
        return itemTypeSeq;
    }
    public string GetitemTypeName()
    {
        return itemTypeName;
    }

}

public class ItemBuilder
{
    private ItemDto itemDto;
    public ItemBuilder()
    {
        itemDto = new ItemDto(); 
    }

    public ItemBuilder SetItemSeq(long itemSeq)
    {
        itemDto.itemSeq = itemSeq;
            return this;
    }

    public ItemBuilder SetItemName(string itemName)
    {
        itemDto.itemName = itemName;
            return this;
    }

    public ItemBuilder SetitemPrice(int itemPrice)
    {
        itemDto.itemPrice = itemPrice;
            return this;
    }

    public ItemBuilder SetItemImage(string itemImage)
    {
        itemDto.itemImage = itemImage;
            return this;
    }

    public ItemBuilder SetItemTypeSeq(int itemTypeSeq)
    {
        itemDto.itemTypeSeq = itemTypeSeq;
            return this;
    }

    public ItemBuilder SetItemTypeName(string itemTypeName)
    {
        itemDto.itemTypeName = itemTypeName;
            return this;
    }

}

public class WearingDto
{
    public ItemDto itemDto;

    public WearingDto(ItemDto itemDto)
    {
        this.itemDto = itemDto;
    }
}

public class BuyRequest
{
    public long itemSeq;
    public int itemPrice;

    public BuyRequest(long itemSeq, int itemPrice)
    {
        this.itemSeq = itemSeq;
        this.itemPrice = itemPrice;
    }
}

public class MyItemResponse
{
    public List<ItemDto> userItems;
    public long userMoney;
    public List<ItemDto> wearingDtos;

    public MyItemResponse(List<ItemDto> userItems, long userMoney, List<ItemDto> wearingDtos)
    {
        this.userItems = userItems;
        this.userMoney = userMoney;
        this.wearingDtos = wearingDtos;
    }
}

public class MarketResponse
{
    public ItemDto item;

    public MarketResponse(ItemDto item)
    {
        this.item = item;
    }

    public ItemDto GetItem()
    {
        return item;
    }
}