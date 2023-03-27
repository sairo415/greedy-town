using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TownNetworkManager : MonoBehaviourPunCallbacks
{

    public TMP_Text PlayersText;

    //여기에 아이디 넣어야됨
    private string UserId;


    private void Start()
    {

      PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 3 }, null);
        /*  Screen.SetResolution(1920, 1080, false);
           Connect();*/

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
