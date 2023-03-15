using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespace makes specific our class
namespace Com.MyTutorial.Photon
{
    public class Launcher : MonoBehaviourPunCallbacks
    {

        #region Private Filed
        // <summary>
        // Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        // we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        // Typically this is used for the OnConnectedToMaster() callback.
        // </summary>
        bool isConnecting;
        #endregion

        #region Private Serializable Fields

        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        #endregion

        #region Private Fields

        // <summary>
        // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        // </summary>
        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        // Start is called before the first frame update
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion


        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
           
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Room for 1");


            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            isConnecting = false;
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        #endregion



        #region Public Methods

        // <summary>
        // Start the connection process.
        // - If already connected, we attempt joining a random room
        // - if not yet connected, Connect this application instance to Photon Cloud Network
        // </summary>
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                isConnecting  = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

    

        #endregion


    }
}