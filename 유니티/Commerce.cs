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
        // ��� ������ �� ���̰� ó�� ��
        StartCoroutine(MyItem());
    }
    public void GetMarket()
    {
        // ��� ������ ���̰� ó�� ��
        StartCoroutine(Market());
    }

    // ������ ���� : response�� ������ ������ ��ȸ�� �Ȱ���. : itemSeq, itemPrice 
    public IEnumerator Buy()
    {
        string url = baseUrl + "item/market";

        long itemSeq = 1; // �ӽ� : ��� �޾ƿ�? : Ŭ���� ��ư�� seq ��������
        int itemPrice = 1000; // �ӽ� : Ŭ���� ��ư�� ���� ��������
        BuyRequest buyRequest = new BuyRequest(itemSeq, itemPrice);
        string data = JsonConvert.SerializeObject(buyRequest);

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
                    StartCoroutine(Buy());
                }
                else
                {
                    print("������ ����");
                    byte[] results = request.downloadHandler.data;
                    print(request.responseCode);
                    print(request.downloadHandler.text);
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    JArray response2 = JArray.Parse(response["userItems"].ToString());
                    JArray response3 = JArray.Parse(response["wearingDtos"].ToString());
                    print(response);
                    print(response[0]["itemSeq"]);
                    // �̹��� ���� �� ���ĵΰ�
                    foreach (JObject jobj in response2)
                    {
                        // jobj["itemSeq"]�� ã�Ƽ�(Find) ���̰�(�Ǵ� �帮��) ó�� 
                    }
                    PlayerPrefs.SetString("money", response["userMoney"].ToString());
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
                    print(request.downloadHandler.text);
                    JObject response = JObject.Parse(request.downloadHandler.text);
                    JArray response2 = JArray.Parse(response["userItems"].ToString());
                    JArray response3 = JArray.Parse(response["wearingDtos"].ToString());
                    print(response);
                    print(response[0]["itemSeq"]);
                    // �̹��� ���� �� ���ĵΰ�
                    foreach (JObject jobj in response2)
                    {
                        // jobj["itemSeq"]�� ã�Ƽ�(Find) ���̰�(�Ǵ� �帮��) ó�� 
                    }

                }

            }
            request.Dispose();
        }
    }

    // Ŀ���͸���¡ : patch ��û, itemDto�� itemSeq ����
    public IEnumerator Custom()
    {
        string url = baseUrl + "itme/character-custom";

        // ���� ����
        List<ItemDto> list = new List<ItemDto>(); // �鿡�� wearingDto�� �޴� ��
        // �ݺ��� ���鼭 ������ �ֱ� : �̰� ��� �޾ƿ�?
        for(int i=0; i<8; i++)
        {
            ItemDto itemDto = new ItemDto();
            ItemBuilder itemBuilder = new ItemBuilder();
            //itemBuilder.SetItemSeq(); // �ش� ��ǥ�� seq �ؽ�Ʈ�� long���� ��ȯ?
            list.Add(itemDto);

        }
        //WearingDto wearingDto = new WearingDto(1, itemDto);

        string data = JsonConvert.SerializeObject(list);
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

    // ���� ��ȸ : seq, price, name, image, type �ʿ�
    // type ���� ��ȸ�ϰ� �ϱ�
    // seq, image, name, price ����
    private IEnumerator Market()
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
                    StartCoroutine(Market());
                }
                else
                {
                    print("���� �Դ�");
                    print(request.responseCode);
                    print(request.downloadHandler.text);
                    JArray response = JArray.Parse(request.downloadHandler.text);
                    print(response);
                    print(response[0]["itemSeq"]);
                    // �̹��� ���� �� ���ĵΰ�
                    foreach(JObject jobj in response)
                    {
                        // jobj["itemSeq"]�� ã�Ƽ�(Find) �� ���̰�(�Ǵ� �帮��) ó�� 
                    }


                }


                    

                

            }
            request.Dispose();
        }
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