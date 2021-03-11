/****************************************************
	文件：PowerSys.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/06 20:11   	
	功能：体力恢复系统
*****************************************************/

using PEProtocol;
using System.Collections.Generic;

public class PowerSys
{
    private static PowerSys instance = null;
    public static PowerSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PowerSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;  
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        TimerSvc.Instance.AddTimeTask(CalcPowerAdd, PECommon.PowerAddSpace, PETimeUnit.Minute, 0);
        PECommon.Log("PowerSys Init Done");
    }

    private void CalcPowerAdd(int tid)
    {
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshPower,
            pshPower=new PshPower
            {

            }
        };
        //在线玩家
        Dictionary<ServerSession, PlayerData> onlineDic = cacheSvc.GetOnlineCache();
        foreach(var item in onlineDic)
        {
            PlayerData playerData = item.Value;
            ServerSession session = item.Key;

            int powerMax = PECommon.GetMaxPower(playerData.lv);
            if (playerData.power > powerMax)
            {
                continue;
            }
            else
            {
                playerData.power += PECommon.PowerAddCount;
                playerData.time = TimerSvc.Instance.GetNowTime();
                if (playerData.power > powerMax)
                {
                    playerData.power = powerMax;
                }
            }
            if(!cacheSvc.IsUpdateSucc(playerData.id, playerData))
            {
                msg.err = (int)Error.UpdateDataError;
            }
            else
            {
                msg.pshPower.power = playerData.power;
            }
            session.SendMsg(msg);                      
        }
    }

}


