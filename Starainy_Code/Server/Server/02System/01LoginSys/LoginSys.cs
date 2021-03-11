/****************************************************
	文件：LoginSys.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/24 18:08   	
	功能：登录系统
*****************************************************/
using PEProtocol;

public class LoginSys
{
    private static LoginSys instance = null;
    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        PECommon.Log("LoginSys Init");
    }
    
    public void ReqLogin(MsgPack pack)
    {
        ReqLogin data = pack.msg.reqLogin;
        //判断当前账号是否上线
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,
            rspLogin = new RspLogin
            {

            }

        };
        
        if (cacheSvc.IsUserOnline(data.userID))
        {//已上线,返回错误信息
            msg.err = (int)Error.UserIsOnline;
        }
        else
        {
            //未上线
            PlayerData pd = cacheSvc.GetUserData(data.userID, data.password);
            //账号是否存在
            if (pd == null)
            {//存在,检测密码,密码错误
                msg.err = (int)Error.WrongPass;
            }
            else
            {//密码正确,响应
                //计算体力增长
                int power = pd.power;
                long nowtime = timerSvc.GetNowTime();
                long milliseconds = nowtime-pd.time;
                int addPower = (int)(milliseconds / (60 * 1000 * PECommon.PowerAddSpace)) * PECommon.PowerAddCount;
                if (addPower > 0)
                {
                    int powerMax = PECommon.GetMaxPower(pd.lv);
                    if (pd.power < powerMax)
                    {
                        pd.power += addPower;
                        if (pd.power > powerMax)
                        {
                            pd.power = powerMax;
                        }
                    }
                }
                if (power != pd.power)
                {
                    cacheSvc.IsUpdateSucc(pd.id,pd);
                }

                msg.rspLogin = new RspLogin { playerData = pd };              
                cacheSvc.UserDataOnline(data.userID, pack.session, pd);
            }                  
        }
        //回应客户端
        pack.session.SendMsg(msg);
    }
    public void ReqRename(MsgPack pack)
    {
        ReqRename data = pack.msg.reqRename;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspRename,
        };       
        if (cacheSvc.IsNameExist(data.name))
        {           
            msg.err = (int)Error.NameIsExist;
        }
        else
        {
            PlayerData playerData = cacheSvc.GetPlayerDataByServerSession(pack.session);
            playerData.name = data.name;
            if(cacheSvc.IsUpdateSucc(playerData.id, playerData)==false)
            {
                msg.err = (int)Error.UpdateDataError;
            }
            else
            {
                msg.rspRename = new RspRename
                {
                    name = data.name,
                };
            }
            pack.session.SendMsg(msg);
            
        }    
    }
    public void UserOffLine(ServerSession session)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataByServerSession(session);
        if (playerData != null)
        {
            playerData.time = timerSvc.GetNowTime();
            if (!cacheSvc.IsUpdateSucc(playerData.id, playerData))
            {
                PECommon.Log("Update OffLine time Error",LogType.Error);
            }
            PECommon.Log("用户已下线");
            cacheSvc.UserOffLine(session);
        }       
    }
}

