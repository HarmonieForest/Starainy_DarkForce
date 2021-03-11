/****************************************************
    文件：ClientSession.cs
	作者：Harmonie
	功能：客户端网络会话
*****************************************************/


using PENet;
using PEProtocol;

public class ClientSession : PESession<GameMsg>
{
    protected override void OnConnected()
    {
        GameRoot.AddTips("服务器连接成功");
        PECommon.Log(" Connect to Server Success");
    }
    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("RcvPack CMD:"+((CMD)msg.cmd) .ToString());
        NetSvc.Instance.AddMsgQue(msg);
    }
    protected override void OnDisConnected()
    {
        GameRoot.AddTips("服务器断开");
        PECommon.Log("Diconnect to Server",LogType.Error);
    }
}