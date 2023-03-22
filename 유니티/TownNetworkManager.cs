using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class TownNetworkManager : MonoBehaviourPunCallbacks
{

    public TMP_Text PlayersText;
     
 
    private void Start()
    {
        Screen.SetResolution(1920,1080,false);
        Connect();

    }

    public void Connect()
    {
        Debug.Log("연결성공");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
  
        Debug.Log("마스터로 연결");

        PhotonNetwork.JoinOrCreateRoom("Room",new RoomOptions { MaxPlayers=3 }, null );
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장");
        PhotonNetwork.Instantiate("TownPlayer", new Vector3(-25.88f, 5, -17.61119f), Quaternion.identity);

    }
    




    

    

}
