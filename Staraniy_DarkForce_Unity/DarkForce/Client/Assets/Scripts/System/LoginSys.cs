/****************************************************
    文件：LoginSys.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/4/18 21:28:18
	功能：登录注册系统
*****************************************************/

using PEProtocol;
using UnityEngine;

public class LoginSys : BaseSystem 
{
    public static LoginSys Instance = null;
    public LoginPanel loginPanel;
    public CreateCharPanel createCharPanel;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init LoginSys");
    }
    
    ///<summary>
    ///进入登录场景
    /// </summary>
    public void EnterLogin()
    {

        //异步加载登录场景
        //加载进度条
        resSvc.AsyncLoadScene(Constants.LoginScene, () =>
        {
            //加载完成后进入注册登录界面
            loginPanel.SetPanelState();
            //加载完成后播放音效
            audioSvc.PlayBgMusic(Constants.BgLogin, true);
            //加载完成后添加提示
            GameRoot.AddTips("Load Complete");
          
        });  
    }
    public void Response(GameMsg msg)
    {
        GameRoot.AddTips("登录成功");
        GameRoot.Instance.SetPlayerData(msg.rspLogin);
        if (msg.rspLogin.playerData .name== "")
        {
            createCharPanel.SetPanelState(true);
        }
        else
        {
            MainCitySys.Instance.EnterMainCity();
        }
        loginPanel.SetPanelState(false);
    }
    public void RspRename(GameMsg msg)
    {
        
        GameRoot.Instance.SetPlayerName(msg.rspRename.name);
        MainCitySys.Instance.EnterMainCity();
        //关闭创建页面
        createCharPanel.SetPanelState(false);
    }
    
 
}