/****************************************************
	文件：GuideSys.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/04 8:53   	
	功能：服务器端任务导航系统
*****************************************************/
using PEProtocol;

public class GuideSys
{
    private static GuideSys instance = null;
    public static GuideSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GuideSys();
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
        PECommon.Log("GuideSys Init Done");
    }

    public void ReqGuide(MsgPack pack)
    {
        ReqGuide data = pack.msg.reqGuide;
       
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspGuide,

        };

        PlayerData playerData = cacheSvc.GetPlayerDataByServerSession(pack.session);
        GuideCfg gc = cfgSvc.GetGuideData(data.guideid);      
        if (playerData.guideid == data.guideid)
        {
            if (playerData.guideid == 1001)
            {
                TaskSys.Instance.CalTaskPrgs(playerData, 1);
            }
            playerData.guideid += 1;                     
            playerData.gold += gc.gold;
            PECommon.CalExp(playerData, gc.exp);
            if (!cacheSvc.IsUpdateSucc(playerData.id, playerData))
            {
                msg.err = (int)Error.UpdateDataError;
            }
            else
            {
                RspGuide rspGuide = new RspGuide
                {
                    guideid = playerData.guideid,
                    gold = playerData.gold,
                    lv = playerData.lv,
                    exp=playerData.exp,                    
                };
                msg.rspGuide = rspGuide;
            }
        }
        else
        {
            msg.err = (int)Error.ServerDataError;
        }
        pack.session.SendMsg(msg);
    }

   
}
