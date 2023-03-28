using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
public class TownChatNetworkManager : MonoBehaviour, IChatClientListener
{

    private ChatClient chatClient;

    bool isConnected;

    public TMP_Text outputText;
    string privateReceiver = "";
    string currentChat;


    [SerializeField] TMP_InputField chatField;
    [SerializeField] TMP_Text chatDisplay;

    [SerializeField] string userId;

    public void SubmitPulbicChatOnClick()
    {
        if (privateReceiver == "")
        {
            chatClient.PublishMessage("RegionChannel", currentChat);
            chatField.text = "";
            currentChat = "";

        }
    }
    public void UserIdOnValueChange(string valueIn)
    {
        userId = valueIn; //여기에 닉네임 넣어주자
    }
    void Start()
    {
       
        userId = PhotonNetwork.LocalPlayer.NickName;
        Debug.Log(userId + " 포톤네임");
        isConnected = true;
        UserIdOnValueChange(userId); //일단 랜덤값
        
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userId)); //이렇게 연결

    }
    void Update()
    {
        if (isConnected)
        {
            chatClient.Service();
        }

        if (chatField.text != "" && Input.GetKey(KeyCode.Return))
        {
            SubmitPulbicChatOnClick();
            /* SubmitPrivateChatOnClick();*/
        }
    }
    public void TypeChatValueChange(string valueIn)
    {
        currentChat = valueIn;
    }
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("DebugReturn() " + level + " " + message);
    }

    public void OnDisconnected()
    {
      
        isConnected = false;
    }

    public void OnConnected()
    {
        Debug.Log("접속");
        chatClient.Subscribe(new string[] { "RegionChannel" });

    }

    public void OnChatStateChange(ChatState state)
    {
        if (state == ChatState.Uninitialized) isConnected = false;
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}: {1}", senders[i], messages[i]);
            chatDisplay.text += "\n" + msgs;

            Debug.Log(msgs);

        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage : " + message);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
}