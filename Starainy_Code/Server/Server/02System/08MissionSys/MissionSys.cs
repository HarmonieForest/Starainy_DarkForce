/****************************************************
	文件：MissionSys.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/13 16:38   	
	功能：副本战斗系统
*****************************************************/
using PEProtocol;

public class MissionSys
{
    private static MissionSys instance = null;
    public static MissionSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MissionSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("MissionSys Init Done");
    }

    public void ReqMissionStart(MsgPack pack)
    {
        ReqMissionStart data = pack.msg.reqMissionStart;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspMissionStart,
        };
        PlayerData playerData = cacheSvc.GetPlayerDataByServerSession(pack.session);
        int power = cfgSvc.GetMapCfg(data.missionID).power;
        if (playerData.mission < data.missionID)
        {
            msg.err = (int)Error.ClientDataError;
        }else if(playerData.power<power)
        {
            msg.err = (int)Error.LakePower;
        }
        else
        {
            playerData.power -= power;
            if (cacheSvc.IsUpdateSucc(playerData.id,playerData))
            {
                msg.rspMissionStart = new RspMissionStart
                {
                    missionID = data.missionID,
                    power=playerData.power,
                };
            }
            else { msg.err = (int)Error.UpdateDataError; }
        }
        pack.session.SendMsg(msg);
    }
    public void ReqMissionEnd(MsgPack pack)
    {
        ReqMissionEnd data = pack.msg.reqMissionEnd;
        GameMsg msg = new GameMsg
        {
            cmd=(int)CMD.RspMissionEnd,
        };
        //校验战斗是否合法
        if (data.win)
        {
            if (data.costTime > 0 && data.restHP > 0)
            {
                MapCfg rd = cfgSvc.GetMapCfg(data.missionID);
                PlayerData pd = cacheSvc.GetPlayerDataByServerSession(pack.session);
                TaskSys.Instance.CalTaskPrgs(pd, 2);
                pd.gold += rd.gold;
                pd.crystal += rd.crystal;
                PECommon.CalExp(pd, rd.exp);
                if (pd.mission == data.missionID)
                {
                    pd.mission += 1;
                }
                if (!cacheSvc.IsUpdateSucc(pd.id, pd))
                {
                    msg.err = (int)Error.UpdateDataError;
                }
                else
                {
                    RspMissionEnd rspMissionEnd = new RspMissionEnd
                    {
                        win = data.win,
                        missionID = data.missionID,
                        mission = pd.mission,
                        restHP = data.restHP,
                        costTime = data.costTime,
                        lv = pd.lv,
                        crystal = pd.crystal,
                        exp = pd.exp,
                        gold = pd.gold,                    
                    };
                    msg.rspMissionEnd = rspMissionEnd;
                }
            }
        }
        else
        {
            msg.err = (int)Error.ClientDataError;
        }
        pack.session.SendMsg(msg);
    }
}

