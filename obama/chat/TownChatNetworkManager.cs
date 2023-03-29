using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using TMPro;
public class TownChatNetworkManager : MonoBehaviour

{
    bool showChat;

    public GameObject joinChatButton;
    public  TMP_InputField chatField;
    public TMP_Text[] chatContent;
    public GameObject chatPanel;
    public PhotonView PV;

    string userId;
    int currentPage = 1, maxPage, multiple;
    string[] CharText = new string[7];

    public void ChatConnectOnClick()
    {
        PhotonNetwork.NickName = "hansaem";
      
        //일단 랜덤값

        if (!showChat)
        {

            showChat = true;
            //활성화
            chatField.text = "";
            for (int i = 0; i < chatContent.Length; i++) chatContent[i].text = "";
            chatPanel.SetActive(true);

        }
        else
        {
            showChat = false;

            //비활성화
            chatPanel.SetActive(false);


        }

    }
    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < chatContent.Length; i++)
            if (chatContent[i].text == "")
            {
                isInput = true;
                chatContent[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < chatContent.Length; i++) chatContent[i - 1].text = chatContent[i].text;
            chatContent[chatContent.Length - 1].text = msg;
        }
    }



    public void Send()
    {
 
        string msg = PhotonNetwork.NickName + " : " + chatField.text;
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + chatField.text);
        chatField.text = "";
    }

    
}