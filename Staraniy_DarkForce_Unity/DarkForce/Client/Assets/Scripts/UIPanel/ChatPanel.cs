/****************************************************
    文件：ChatPanel.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/5 13:41:7
	功能：聊天功能面板
*****************************************************/

using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : BasePanel 
{
    #region UI define
    public Text txtChat;
    public InputField iptChatTxt;

    public Button btnWorld;
    public Button btnGuild;
    public Button btnFriend;
    public Button btnClose;
    public Button btnSend;

    public Image imgWorld;
    public Image imgGuild;
    public Image imgFriend;

    #endregion

    private int chatType;
    private List<string> chatList = new List<string>();

    protected override void InitPanel()
    {
        base.InitPanel();
        chatType = 0;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (chatType == 0)
        {//世界聊天
            string chatMsg = "";
            for(int i = 0; i < chatList.Count; i++)
            {
                chatMsg += chatList[i] + "\n";
            }
            SetText(txtChat, chatMsg);
            SetSprite(imgWorld, PathDefine.ImgChatState1);
            SetSprite(imgGuild, PathDefine.ImgChatState2);
            SetSprite(imgFriend, PathDefine.ImgChatState2);
        }
        
        else if (chatType==1)
        {//公会聊天
            SetText(txtChat, "尚未加入公会");
            SetSprite(imgWorld, PathDefine.ImgChatState2);
            SetSprite(imgGuild, PathDefine.ImgChatState1);
            SetSprite(imgFriend, PathDefine.ImgChatState2);
        }
        
        else if (chatType == 2)
        {//好友聊天
            SetText(txtChat, "暂无好友信息");
            SetSprite(imgWorld, PathDefine.ImgChatState2);
            SetSprite(imgGuild, PathDefine.ImgChatState2);
            SetSprite(imgFriend, PathDefine.ImgChatState1);
        }   
    }

    

    public void AddChatMsg(string name,string chat)
    {
        chatList.Add(Constants.Color(name + ":", TxtColor.Blue) + chat);
        if (chatList.Count > 14)
        {
            chatList.RemoveAt(0);
        }
        if (GetPanelState())
        {
            RefreshUI();
        }
        
    }
    #region UI点击事件
    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 0;
        SetPanelState(false);
    }
    public void OnClicWorldBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 0;
        RefreshUI();
    }
    public void OnClickGuildBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 1;
        RefreshUI();
    }
    public void OnClickFriendBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = 2;
        RefreshUI();
    }

    private bool cansend = true;
    public void OnClickSendBtn()
    {
        if (!cansend)
        {
            GameRoot.AddTips("聊天消息发送过于频繁");
            return;
        }
        if(iptChatTxt.text!=null&&iptChatTxt.text!=""&&iptChatTxt.text!=" ")
        {
            if (iptChatTxt.text.Length > 10)
            {
                GameRoot.AddTips("聊天信息过长");
            }
            else
            {
                //发送消息到网络服务器
                GameMsg msg = new GameMsg
                {
                    cmd = (int)CMD.ReqChat,
                    reqChat = new ReqChat
                    {
                        chat = iptChatTxt.text
                    }
                };
                iptChatTxt.text = "";
                netSvc.SendRequest(msg);
                cansend = false;
                timerSvc.AddTimeTask((int tid) =>
                {
                    cansend = true;

                },5,PETimeUnit.Second);
            }
        }
        else
        {
            GameRoot.AddTips("未输入聊天信息");
        }
    }
    
    #endregion
}