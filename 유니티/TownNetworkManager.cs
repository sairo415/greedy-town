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
    private Vector3 start = new Vector3(-18, 10, -12);



    private void Start()
    {

        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
       
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

        Debug.Log("타운 룸으로 입장!");

        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        
        PhotonNetwork.Instantiate("TownPlayer", start, Quaternion.Euler(0, 180, 0));
    }


    // 랭킹






}
