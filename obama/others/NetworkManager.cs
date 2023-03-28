using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DisconnectPanel")]
    public GameObject DisconnectPanel;
    public TMP_InputField NicknameInput; 

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    
     
    private void Start()
    {
        Screen.SetResolution(540,960,false);
         
    
    }

    public void Connect()
    {
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room",new RoomOptions { MaxPlayers=3 }, null );
    }

    public override void OnJoinedRoom()
    {
        ShowPanel(RoomPanel); 
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }


    void ShowPanel(GameObject CurPanel)
    {
        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(false);
        CurPanel.SetActive(true);
    }





}
