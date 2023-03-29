using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BossPhotonManager : MonoBehaviourPunCallbacks
{
    // ���� �Է�
    private readonly string version = "1.1f";
    // ����� ���̵� �Է�
    public string userId = "BK";

	private void Awake()
    {
        // Start : ù��° �������� ������Ʈ �Ǳ� ���� ȣ��
        // Awake : ��ũ��Ʈ�� ������ ���ڸ���(�÷��̸� ���ڸ���) Start ���� ���� ȣ���. ���� ���� ������ �س��� �Լ�

        // ������ ���ο� Scene �� �ε����� �� �ش� ������ ������ �����鵵 �ڵ����� �ε��� ����
        // ���� ���� �����鿡�� �ڵ����� ���� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
        // ���� ������ �������� ���� ���
        PhotonNetwork.GameVersion = version;
        // ���� ���̵� �Ҵ�
        PhotonNetwork.NickName = userId;
        // ���� ������ ��� Ƚ�� ����. �ʴ� 30ȸ(�ʱⰪ)
        Debug.Log(PhotonNetwork.SendRate);
        // ���� ����
        PhotonNetwork.ConnectUsingSettings();
    }

    // ���� ������ ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        // �ݹ��Լ��� ����� ȣ��Ǿ����� Ȯ��
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // false
        PhotonNetwork.JoinLobby(); // �κ� ����
    }

    // �κ� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // true
        // ���� �뿡 ����
        // ���� ��ġ����ŷ ��� ����
        // �̹� ������ �� �߿��� �������� ������ ����
        // ���࿡ ������ ���� ���� ��� Failed message
        PhotonNetwork.JoinRandomRoom();
    }

    // ������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // returnCode ���� �ڵ�
        // message ���� �޽���
        Debug.Log($"JoinRandom Failed {returnCode} : {message}");

        // ���� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4; // �ִ� ������ �� : 4��
        ro.IsOpen = true; // ���� ���� ����
        ro.IsVisible = true; // �κ񿡼� ���� ����(true : �κ񿡼� ���Ͽ� ���� ��Ŵ)

        // �� ����
        PhotonNetwork.CreateRoom("Boss Room", ro);
    }

    // �� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    // �뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        // �뿡 ������ ����� ���� Ȯ��
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            // �÷��̾� �г���, ���� �ѹ� �α� ���
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
            // $ => String.Format()
        }

        BossGameManager bossGameManager = FindObjectOfType<BossGameManager>();

        // ĳ���� ����
        GameObject bossPlayerObject = PhotonNetwork.Instantiate("BossPlayer", Vector3.zero, Quaternion.Euler(0, 0, 0), 0);
        bossGameManager.player = bossPlayerObject.GetComponent<BossPlayer>();
        bossPlayerObject.GetComponent<BossPlayer>().bossPlayerName = userId;
    }
}