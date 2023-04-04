using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BossPhotonManager : MonoBehaviourPunCallbacks
{
    // 버전 입력
    private readonly string version = "1.1f";
    // 사용자 아이디 입력
    public string userId = "BK";

	private void Awake()
    {
        // Start : 첫번째 프레임이 업데이트 되기 전에 호출
        // Awake : 스크립트가 시작이 되자마자(플레이를 하자마자) Start 보다 먼저 호출됨. 가장 빨리 세팅을 해놓는 함수

        // 방장이 새로운 Scene 을 로딩했을 때 해당 서버에 접속한 유저들도 자동으로 로딩을 해줌
        // 같은 룸의 유저들에게 자동으로 씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;
        // 같은 버전의 유저끼리 접속 허용
        PhotonNetwork.GameVersion = version;
        // 유저 아이디 할당
        PhotonNetwork.NickName = userId;
        // 포톤 서버와 통신 횟수 설정. 초당 30회(초기값)
        Debug.Log(PhotonNetwork.SendRate);
        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        // 콜백함수가 제대로 호출되었는지 확인
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // false
        PhotonNetwork.JoinLobby(); // 로비 입장
    }

    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // true
        // 랜덤 룸에 접속
        // 랜덤 매치메이킹 기능 제공
        // 이미 생성된 룸 중에서 무작위로 선택해 입장
        // 만약에 생성된 룸이 없을 경우 Failed message
        PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // returnCode 에러 코드
        // message 에러 메시지
        Debug.Log($"JoinRandom Failed {returnCode} : {message}");

        // 룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4; // 최대 접속자 수 : 4명
        ro.IsOpen = true; // 룸의 오픈 여부
        ro.IsVisible = true; // 로비에서 노출 여부(true : 로비에서 룸목록에 노출 시킴)

        // 룸 생성
        PhotonNetwork.CreateRoom("Boss Room", ro);
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        // 룸에 접속한 사용자 정보 확인
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            // 플레이어 닉네임, 고유 넘버 로그 출력
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
            // $ => String.Format()
        }

        BossGameManager bossGameManager = FindObjectOfType<BossGameManager>();

        // 캐릭터 생성
        GameObject bossPlayerObject = PhotonNetwork.Instantiate("BossPlayer", Vector3.zero, Quaternion.Euler(0, 0, 0), 0);
        bossGameManager.player = bossPlayerObject.GetComponent<BossPlayer>();
        bossPlayerObject.GetComponent<BossPlayer>().bossPlayerName = userId;
    }
}