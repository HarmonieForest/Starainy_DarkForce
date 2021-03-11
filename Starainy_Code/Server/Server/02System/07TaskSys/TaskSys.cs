/****************************************************
	文件：TaskSys.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/07 20:42   	
	功能：任务奖励系统
*****************************************************/

using PEProtocol;

public class TaskSys
{
    private static TaskSys instance = null;
    public static TaskSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TaskSys();
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
        PECommon.Log("TaskSys Init Done");
    }

    public void ReqTaheTaskReward(MsgPack pack)
    {
        ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspTakeTaskReward,
        };

        PlayerData playerData = cacheSvc.GetPlayerDataByServerSession(pack.session);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.rid);
        TaskRewardState trs = CalTaskRewardState(playerData, data.rid);

        if (trs.prgs == trc.count && !trs.taked)
        {
            playerData.gold += trc.gold;
            PECommon.CalExp(playerData, trc.exp);
            trs.taked = true;
        }
        else
        {
            msg.err = (int)Error.ClientDataError;
        }
        //更新任务进度数据
        CalTaskArr(playerData, trs);
        if (!cacheSvc.IsUpdateSucc(playerData.id, playerData))
        {
            msg.err = (int)Error.UpdateDataError;
        }
        else
        {
            msg.rspTakeTaskReward = new RspTakeTaskReward
            {
                gold = playerData.gold,
                exp = playerData.exp,
                lv = playerData.lv,
                taskArr = playerData.taskArr,
            };
        }
        pack.session.SendMsg(msg);
        //ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;

        //GameMsg msg = new GameMsg
        //{
        //    cmd = (int)CMD.RspTakeTaskReward
        //};

        //PlayerData pd = cacheSvc.GetPlayerDataByServerSession(pack.session);

        //TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.rid);
        //TaskRewardState trs = CalTaskRewardState(pd, data.rid);

        //if (trs.prgs == trc.count && !trs.taked)
        //{
        //    pd.gold += trc.gold;
        //    PECommon.CalExp(pd, trc.exp);
        //    trs.taked = true;
        //    //更新任务进度数据
        //    CalTaskPrgs(pd, trs.ID);

        //    if (!cacheSvc.IsUpdateSucc(pd.id, pd))
        //    {
        //        msg.err = (int)Error.UpdateDataError;
        //    }
        //    else
        //    {
        //        RspTakeTaskReward rspTakeTaskReward = new RspTakeTaskReward
        //        {
        //            gold = pd.gold,
        //            lv = pd.lv,
        //            exp = pd.exp,
        //            taskArr = pd.taskArr
        //        };
        //        msg.rspTakeTaskReward = rspTakeTaskReward;
        //    }
        //}
        //else
        //{
        //    msg.err = (int)Error.ClientDataError;
        //}
        //pack.session.SendMsg(msg);
    }

    public TaskRewardState CalTaskRewardState(PlayerData playerData,int rid)
    {
        TaskRewardState trs = null;
        for(int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskinfo = playerData.taskArr[i].Split('|');
            if (int.Parse(taskinfo[0]) == rid)
            {
                trs = new TaskRewardState
                {
                    ID = int.Parse(taskinfo[0]),
                    prgs = int.Parse(taskinfo[1]),
                    taked = taskinfo[2].Equals("1"),
                };
                break;
            }
        }
        return trs;
    }

    public void CalTaskArr(PlayerData playerData,TaskRewardState trs)
    {
        string result = trs.ID + "|" + trs.prgs + "|" + (trs.taked ? 1 : 0);
        int index = -1;
        for(int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskInfo = playerData.taskArr[i].Split('|');
            if (int.Parse(taskInfo[0]) == trs.ID)
            {
                index = i;
                break;
            }
        }
        playerData.taskArr[index] = result;
    }

    public void CalTaskPrgs(PlayerData playerData,int tid)
    {
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);
        TaskRewardState trs = CalTaskRewardState(playerData, tid);
        if (trs.prgs < trc.count)
        {
            trs.prgs += 1;
            CalTaskArr(playerData, trs);
        }

        ServerSession session = cacheSvc.GetOnlineServerSession(playerData.id);
        if (session != null)
        {
            session.SendMsg(new GameMsg
            {
                cmd=(int)CMD.PshTaskPrgs,
                pshTaskPrgs=new PshTaskPrgs
                {
                    taskArr=playerData.taskArr,
                }
            });
        }
    }
    public PshTaskPrgs GetTaskPrgs(PlayerData playerData, int tid)
    {
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);
        TaskRewardState trs = CalTaskRewardState(playerData, tid);
        if (trs.prgs < trc.count)
        {
            trs.prgs += 1;
            CalTaskArr(playerData, trs);
        }
        return new PshTaskPrgs
        {
            taskArr = playerData.taskArr,
        };
    
    }
}


