using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class BossRoomNetworkManager : MonoBehaviourPunCallbacks
{
    
    public void ToTheBossLobby()
    {
        SceneManager.LoadScene("BossLobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("캐릭터 생성");
        PhotonNetwork.Instantiate("BossRoomPlayer", new Vector3(0,10,0), Quaternion.identity);

    }


}
