using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BossLobbyNetworkManager : MonoBehaviourPunCallbacks
{


    private string userId = "hansaem";

    public TMP_InputField userIdText;
    public TMP_InputField roomNameText;

    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

    public GameObject roomPrefeb;

    public Transform scrollContent;

   
  
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;


    }

    private void Start()
    {
        
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
   
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("�κ� ����");
    }

    public void ToTheTown()
    {
        SceneManager.LoadScene("Town");
    }

    //�游���

    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� �Ϸ� : " + roomNameText.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� �Ϸ�");

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("�븮��Ʈ �ݹ��Լ� �����ϱ�");
        GameObject tempRoom = null;
        foreach (var room in roomList)
        {
            if (room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    Debug.Log("�� ����� check On");        
                    GameObject _room = Instantiate(roomPrefeb, scrollContent);
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, _room);
                    
                }
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }

   
    #region UI_BUTTON_CALLBACK

    public void OnMakeRoomClick()   
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 4;

        if (string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"ROOM_{Random.Range(1, 100):000}";
        }

        PhotonNetwork.CreateRoom(roomNameText.text, ro);

        //���̵�
        SceneManager.LoadScene("BossRoomReady");
    }

    #endregion

}
