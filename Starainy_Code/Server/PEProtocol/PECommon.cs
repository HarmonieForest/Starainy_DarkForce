
/****************************************************
	文件：PECommon.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/25 18:49   	
	功能：客户端服务端通用工具类
*****************************************************/


using PENet;
using PEProtocol;

public enum LogType
{
	Log=0,
	Warn=1,
	Error=2,
	Info=3
}
public class PECommon
{
	public static void Log(string msg="",LogType tp = LogType.Log)
	{
		LogLevel lv = (LogLevel)tp;
		PETool.LogMsg(msg, lv);
	}

	//公用的计算战斗里的方法
	public static int GetFightByProps(PlayerData playerData)
	{
		return playerData.lv * 100 + playerData.ap + playerData.ad + playerData.addef + playerData.apdef;
	}
	//体力上限
	public static int GetMaxPower(int lv)
	{
		return ((lv - 1) / 10) * 150 + 150;
	}
	public static int GetNextLevelExp(int lv)
	{
		return lv * 100*lv;
	}

	public const int PowerAddSpace = 5;//分钟
	public const int PowerAddCount = 2;

    //升级相关数据的计算
    public static void CalExp(PlayerData playerData, int addExp)
    {
        int curtLv = playerData.lv;
        int curtExp = playerData.exp;
        int restAddExp = addExp;
        while (true)
        {
            int upNeedExp = GetNextLevelExp(curtLv) - curtExp;
            if (restAddExp >= upNeedExp)
            {
                curtLv += 1;
                curtExp = 0;
                restAddExp -= upNeedExp;
            }
            else
            {
                playerData.lv = curtLv;
                playerData.exp = curtExp + restAddExp;
                break;
            }
        }
    }
}


