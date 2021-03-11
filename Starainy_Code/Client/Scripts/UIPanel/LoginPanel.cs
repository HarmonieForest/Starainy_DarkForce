/****************************************************
    文件：LoginPanel.cs
	作者：Harmonie
	功能：登录注册页面
*****************************************************/

//using UnityEditor.Build.Reporting;
using PEProtocol;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel 
{
    public InputField iptUserID;
    public InputField iptPassword;
    public Button btnLogin;
    public Button btnNotice;
    protected override void InitPanel()
    {
        base.InitPanel();
        //获取本地存取的用户数据
        if (PlayerPrefs.HasKey("UserID") && PlayerPrefs.HasKey("Password"))
        {
            iptUserID.text = PlayerPrefs.GetString("UserID");
            iptPassword.text = PlayerPrefs.GetString("Password");
        }
        else
        {
            iptUserID.text = "";
            iptPassword.text = "";
        }
    }
    ///<summary>
    ///点击进入游戏
    /// </summary>
    public void ClickEnterBtn()
    {
        audioSvc.PlayUIAudio(Constants.UILoginBtn);
        //判断账号数据是否正确
        string userID = iptUserID.text;
        string password = iptPassword.text;
        if (userID != "" && password != "")
        {
            //更新本地存储的账号资料
            PlayerPrefs.SetString("UserID", userID);
            PlayerPrefs.SetString("Password", password);

            //发送网络信息,请求登录
            GameMsg msg = new GameMsg {
                cmd = (int)CMD.ReqLogin,
                reqLogin = new ReqLogin
                {
                    userID = userID,
                    password = password,
                }
            };
            netSvc.SendRequest(msg);          
        }
        else
        {
            GameRoot.AddTips("账号或密码为空");
        }
    }
    public void ClickNoticeBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        GameRoot.AddTips("提示信息界面,尚未开发完成");
    }
    
}