/****************************************************
    文件：NetSvc.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/4/25 18:25:7
	功能：网络服务
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;

public class NetSvc : MonoBehaviour 
{
    public static NetSvc Instance = null;

    PENet.PESocket<ClientSession, GameMsg> client = null;
    private Queue<GameMsg> msgQue = new Queue<GameMsg>();
    //定义锁
    private static readonly string obj = "lock";   
    public void InitSvc()
    {
        Instance = this;
       
        PECommon.Log("NetSvc Init");
        client = new PENet.PESocket<ClientSession, GameMsg>();
        //设置日志
        client.SetLog(true, (string msg, int lv) =>
        {
            switch (lv)
            {
                case 0:
                    msg = "Log:" + msg;
                    Debug.Log(msg);
                    break;
                case 1:
                    msg = "Warn:" + msg;
                    Debug.LogWarning(msg);
                    break;
                case 2:
                    msg = "Error:" + msg;
                    Debug.LogError(msg);
                    break;
                case 3:
                    msg = "Info:" + msg;
                    Debug.Log(msg);
                    break;
            }
        });
        client.StartAsClient(SrvCfg.srvIP, SrvCfg.srvPort);      
    }
    public void SendRequest(GameMsg msg)
    {
        if (client.session != null)
        {
            client.session.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTips("服务器未连接");
            InitSvc();
        }
    }
    private void Update()
    {//读取消息队列
        if (msgQue.Count > 0)
        {
            lock (obj)
            {
                GameMsg msg = msgQue.Dequeue();
                HandleMsg(msg);
            }
            
        }
    }
    //将消息加入队列
    public void AddMsgQue(GameMsg msg)
    {
        lock (obj)
        {
            msgQue.Enqueue(msg);
        }
    }
    //处理读取到的消息
    public void HandleMsg(GameMsg msg)
    {
        if (msg.err != (int)Error.None)
        {
            switch ((Error)msg.err)
            {
                case Error.UserIsOnline:
                    GameRoot.AddTips("账号已在线");
                    break;
                case Error.WrongPass:
                    GameRoot.AddTips("密码输入错误");
                    break;
                case Error.NameIsExist:
                    GameRoot.AddTips("名字重复");
                    break;
                case Error.UpdateDataError:
                    PECommon.Log("数据库更新异常", LogType.Error);
                    GameRoot.AddTips("网络不稳定,请确认您的网络连接设置");
                    break;
                case Error.ServerDataError:
                    PECommon.Log("服务器数据异常", LogType.Error);
                    GameRoot.AddTips("客户端数据异常");
                    break;
                case Error.ClientDataError:
                    PECommon.Log("客户端数据数据异常", LogType.Error);                  
                    break;
                case Error.LakeCrystal:
                    GameRoot.AddTips("水晶余额不足");
                    break;
                case Error.LakeGold:
                    GameRoot.AddTips("金币余额不足");
                    break;
                case Error.LakeLevel:
                    GameRoot.AddTips("未满足等级要求");
                    break;
                case Error.LakeDiamond:
                    GameRoot.AddTips("钻石余额不足");
                    break;
                case Error.LakePower:
                    GameRoot.AddTips("体力不足");
                    break;
            }
            return;
        }
        switch ((CMD)msg.cmd)
        {
            case CMD.RspLogin:
                LoginSys.Instance.Response(msg);
                break;
            case CMD.RspRename:
                LoginSys.Instance.RspRename(msg);
                break;
            case CMD.RspGuide:
                MainCitySys.Instance.RspGuide(msg);
                break;
            case CMD.RspStrong:
                MainCitySys.Instance.RspStrong(msg);
                break;
            case CMD.RspChat:
                MainCitySys.Instance.RspChat(msg);
                break;
            case CMD.RspBuy:
                MainCitySys.Instance.RspBuy(msg);
                break;
            case CMD.PshPower:
                MainCitySys.Instance.PshPower(msg);
                break;
            case CMD.RspTakeTaskReward:
                MainCitySys.Instance.RspTakeTaskReward(msg);
                break;
            case CMD.PshTaskPrgs:
                MainCitySys.Instance.PshTaskPrgs(msg);
                break;
            case CMD.RspMissionStart:
                MissionSys.Instance.RspMissionStart(msg);
                break;
            case CMD.RspMissionEnd:
                BattleSys.Instance.RspMissionEnd(msg);
                break;
        }
    }
}