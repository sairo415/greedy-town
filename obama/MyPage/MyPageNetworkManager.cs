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
        Debug.Log("���Ἲ��");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
  
        Debug.Log("�����ͷ� ����");

        //�г��� �����ֱ�(���⿡ ���߿� ��� �г��� �־������)
        PhotonNetwork.LocalPlayer.NickName = ($"USER_{Random.Range(0, 100):00}");
        //PhotonNetwork.JoinOrCreateRoom("Room",new RoomOptions { MaxPlayers=3 }, null );
        PhotonNetwork.JoinLobby();
        check = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ񿬰�");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("�� ����");
        PhotonNetwork.Instantiate("TownPlayer", new Vector3(-25.88f, 5, -17.61119f), Quaternion.identity);

    }
    
    public void logout()
    {
        // 1. header�� accesstoken ������ ������� 403�̸�
        // 1-1. refreshtoken�� email�� /reissue�� ������ response�� message�� success�̸� 
        // Playerprefs�� accesstoken �� �ٲ۴�. �׸��� �ٽ� ��û
        // 1-2. response�� message�� success�� �ƴϸ�

        // 2. header�� accesstoken ������ ������� 200�̸�
        // ��û�� �� ����
    }








}
