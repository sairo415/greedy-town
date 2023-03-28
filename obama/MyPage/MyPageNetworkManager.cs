using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MyPageNetworkManager : MonoBehaviourPunCallbacks
{

    public TMP_Text PlayersText;
    private bool check;
 
    private void Start()
    {
        Screen.SetResolution(1920,1080,false);
        Connect();

    }
    public void ChangeScene()
    {
        if(check) SceneManager.LoadScene("Town");
    
    }

    public void Connect()
    {
        Debug.Log("연결성공");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
  
        Debug.Log("마스터로 연결");

        //닉네임 정해주기(여기에 나중에 디비값 닉네임 넣어줘야함)
        PhotonNetwork.LocalPlayer.NickName = ($"USER_{Random.Range(0, 100):00}");
        //PhotonNetwork.JoinOrCreateRoom("Room",new RoomOptions { MaxPlayers=3 }, null );
        PhotonNetwork.JoinLobby();
        check = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비연결");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장");
        PhotonNetwork.Instantiate("TownPlayer", new Vector3(-25.88f, 5, -17.61119f), Quaternion.identity);

    }
    
    public void logout()
    {
        // 1. header에 accesstoken 보내서 결과값이 403이면
        // 1-1. refreshtoken과 email을 /reissue에 보내서 response의 message가 success이면 
        // Playerprefs의 accesstoken 값 바꾼다. 그리고 다시 요청
        // 1-2. response의 message가 success가 아니면

        // 2. header에 accesstoken 보내서 결과값이 200이면
        // 요청한 일 수행
    }








}
