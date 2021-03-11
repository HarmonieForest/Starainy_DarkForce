/****************************************************
	文件：BuySys.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/05 22:22   	
	功能：资源交易系统
*****************************************************/

using PEProtocol;

public class BuySys
{
    private static BuySys instance = null;
    public static BuySys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BuySys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;

        PECommon.Log("BuySys Init Done");
    }
    public void ReqBuy(MsgPack pack)
    {
        ReqBuy data = pack.msg.reqBuy;
        PlayerData playerData = cacheSvc.GetPlayerDataByServerSession(pack.session);

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspBuy,      
        };

        if (playerData.diamond < data.cost)
        {
            msg.err = (int)Error.LakeDiamond;
        }
        else
        {
            playerData.diamond -= data.cost;
            PshTaskPrgs pshTaskPrgs = null;
            switch (data.type)
            {
                case 0:                  
                    playerData.power += 30;
                    pshTaskPrgs=TaskSys.Instance.GetTaskPrgs(playerData, 4);
                    break;
                case 1:                 
                    playerData.gold += 5000;
                    pshTaskPrgs= TaskSys.Instance.GetTaskPrgs(playerData, 5);
                    break;
            }
            if (!cacheSvc.IsUpdateSucc(playerData.id, playerData))
            {
                msg.err = (int)Error.UpdateDataError;
            }
            else
            {
                msg.rspBuy = new RspBuy
                {
                    type = data.type,
                    gold = playerData.gold,
                    diamond = playerData.diamond,
                    power = playerData.power,
                };
                msg.pshTaskPrgs = pshTaskPrgs;
            }
        }    
        pack.session.SendMsg(msg);
    }
}

