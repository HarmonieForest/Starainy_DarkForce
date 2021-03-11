
/****************************************************
	文件：NetSvc.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/24 15:34   	
	功能：网络通信服务
*****************************************************/
using PENet;
using PEProtocol;
using System.Collections.Generic;

public class MsgPack
{
    public ServerSession session;
    public GameMsg msg;
    public MsgPack(ServerSession session,GameMsg msg)
    {
        this.session = session;
        this.msg = msg;
    }
}
public class NetSvc
{
    private static NetSvc instance = null;
    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();
            }
            return instance;
        }
    }
    //定义一个队列存储数据
    private Queue<MsgPack> msgQue = new Queue<MsgPack>();
    //定义一个锁确保多线程时正常运行
    public static readonly string obj = "lock";
    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SrvCfg.srvIP, SrvCfg.srvPort);

        PECommon.Log("NetSvc is done");
    }
    public void AddMsgQueue(ServerSession session,GameMsg msg)
    {
        lock(obj)
        {
            msgQue.Enqueue(new MsgPack (session,msg));
        } 
    }
    public void Update()
    {
        if (msgQue.Count > 0)
        {           
            lock (obj)
            {
                MsgPack pack = msgQue.Dequeue();
                HandleMsg(pack);
            }
        }
    }
    private void HandleMsg(MsgPack pack)
    {
        switch ((CMD)pack.msg.cmd)
        {
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
            case CMD.ReqGuide:
                GuideSys.Instance.ReqGuide(pack);
                break;
            case CMD.ReqStrong:
                StrongSys.Instance.ReqStrong(pack);
                break;
            case CMD.ReqChat:
                ChatSys.Instance.ReqChat(pack);
                break;
            case CMD.ReqBuy:
                BuySys.Instance.ReqBuy(pack);
                break;
            case CMD.ReqTakeTaskReward:
                TaskSys.Instance.ReqTaheTaskReward(pack);
                break;
            case CMD.ReqMissionStart:
                MissionSys.Instance.ReqMissionStart(pack);
                break;
            case CMD.ReqMissionEnd:
                MissionSys.Instance.ReqMissionEnd(pack);
                break;
        }
    }
}


