/****************************************************
	文件：StrongSys.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/05 10:37   	
	功能：服务器端强化系统服务
*****************************************************/
using PEProtocol;

public class StrongSys
{
    private static StrongSys instance = null;
    public static StrongSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StrongSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
       
        PECommon.Log("StrongSys Init Done");
    }
    public void ReqStrong(MsgPack pack)
    {
        ReqStrong data = pack.msg.reqStrong;

        GameMsg msg = new GameMsg
        {
            cmd=(int)CMD.RspStrong,

        };
        PlayerData playerData = cacheSvc.GetPlayerDataByServerSession(pack.session);
        int curtStarLv = playerData.strongArr[data.pos];
        StrongCfg nextData = CfgSvc.Instance.GetStrongCfg(data.pos,curtStarLv+1);
        //条件判断
        if (curtStarLv < 10)
        {
            if (playerData.lv < nextData.minlv)
            {
                msg.err = (int)Error.LakeLevel;
            }
            else if (playerData.gold < nextData.gold)
            {
                msg.err = (int)Error.LakeGold;
            }
            else if (playerData.crystal < nextData.crystal)
            {
                msg.err = (int)Error.LakeCrystal;
            }
            else
            {
                //任务进度数据更新
                TaskSys.Instance.CalTaskPrgs(playerData, 3);
                //资源扣除
                playerData.gold -= nextData.gold;
                playerData.crystal -= nextData.crystal;
                playerData.strongArr[data.pos] += 1;
                //属性增加
                playerData.hp += nextData.addhp;
                playerData.ad += nextData.addhurt;
                playerData.ap += nextData.addhurt;
                playerData.addef += nextData.adddef;
                playerData.apdef += nextData.adddef;
            }
        }
        //更新数据库
        if (!cacheSvc.IsUpdateSucc(playerData.id, playerData))
        {
            msg.err = (int)Error.UpdateDataError;
        }
        else
        {
            msg.rspStrong = new RspStrong
            {
                gold = playerData.gold,
                crystal = playerData.crystal,
                strongArr = playerData.strongArr,
                hp=playerData.hp,
                ad=playerData.ad,
                ap=playerData.ap,
                addef=playerData.addef,
                apdef=playerData.apdef,
            };
        }
        pack.session.SendMsg(msg);
    }
}


