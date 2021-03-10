/****************************************************
	文件：ServerSession.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/25 8:45   	
	功能：网络会话连接
*****************************************************/


using PENet;
using PEProtocol;

public class ServerSession:PESession<GameMsg>
{
    public int sessionID=0;
    protected override void OnConnected()
    {
        sessionID= ServerRoot.Instance.GetSessonID();
        PECommon.Log("SessionID:"+sessionID+"Client Connect");     
    }
    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("SessionID:" + sessionID + "RequestCode:" +((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQueue(this,msg);
      
    }
    protected override void OnDisConnected()
    {
        LoginSys.Instance.UserOffLine(this);
        PECommon.Log("SessionID:" + sessionID + "Client DisConnect");
    }
}

