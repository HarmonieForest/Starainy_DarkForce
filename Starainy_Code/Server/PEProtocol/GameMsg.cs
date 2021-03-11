/****************************************************
	文件：Class1.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/24 18:18   	
	功能：网络通信协议(服务器端与客户端共用)
*****************************************************/
using System;
using PENet;


namespace PEProtocol
{
	[Serializable]
    public class GameMsg:PEMsg
    {
		public ReqLogin reqLogin;
		public RspLogin rspLogin;
		public ReqRename reqRename;
		public RspRename rspRename;

		public ReqGuide reqGuide;
		public RspGuide rspGuide;

		public ReqStrong reqStrong;
		public RspStrong rspStrong;

		public ReqChat reqChat;
		public RspChat rspChat;

		public ReqBuy reqBuy;
		public RspBuy rspBuy;

		public PshPower pshPower;

		public ReqTakeTaskReward reqTakeTaskReward;
		public RspTakeTaskReward rspTakeTaskReward;

		public PshTaskPrgs pshTaskPrgs;

		public ReqMissionStart reqMissionStart;
		public RspMissionStart rspMissionStart;

		public ReqMissionEnd reqMissionEnd;
		public RspMissionEnd rspMissionEnd;
    }

    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int power;
        public int diamond;
		public int crystal;
        public int gold;
        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int dodge;//闪避概率
        public int pierce;//穿透比率
        public int critical;//暴击概率

        public int guideid;

		public int[] strongArr;

		public long time;

		public string[] taskArr;

		public int mission;
    }


    #region 登录相关
    [Serializable]
    public class ReqLogin
	{
		//客户端发送给服务器端的数据
		public string userID;
		public string password;
	}
	[Serializable]
	public class RspLogin
	{
		//服务器端对客户端的响应
		public PlayerData playerData;
	}
	
    [Serializable]
	public class ReqRename
	{
		public string name;
	}
	[Serializable]
	public class RspRename
	{
		public string name;
	}
    #endregion
    #region 引导相关
	[Serializable]
	public class ReqGuide
	{
		public int guideid;
	}
	[Serializable]
	public class RspGuide
	{
		public int guideid;
		public int gold;
		public int lv;
		public int exp;
	}
    #endregion
    #region 强化相关
	[Serializable]
	public class ReqStrong
	{
		public int pos;
	}
	[Serializable]
	public class RspStrong
	{
		public int gold;
		public int crystal;
		public int hp;
		public int ad;
		public int ap;
		public int addef;
		public int apdef;
		public int[] strongArr;
	}
    #endregion
    #region 聊天相关
	[Serializable]
	public class ReqChat
	{
		public string chat;
	}
	[Serializable]
	public class RspChat
	{
		public string chat;
		public string name;
	}
    #endregion
    #region 资源交易相关
	[Serializable]
	public class ReqBuy
	{
		public int type;
		public int cost;
	}
	[Serializable]
	public class RspBuy
	{
		public int type;
		public int diamond;
		public int gold;
		public int power;
	}
	[Serializable]
	public class PshPower
	{
		public int power;
	}
    #endregion
    #region 任务奖励相关
	[Serializable]
	public class ReqTakeTaskReward
	{
		public int rid;
	}
	[Serializable]
	public class RspTakeTaskReward
	{
		public int gold;
		public int exp;
		public int lv;
		public string[] taskArr;
	}
    [Serializable]
    public class PshTaskPrgs
    {
        public string[] taskArr;
    }
    #endregion
    #region 副本战斗相关
	[Serializable]
	public class ReqMissionStart
	{
		public int missionID;
	}
	[Serializable]
	public class RspMissionStart
	{
		public int missionID;
		public int power;
	}
	[Serializable]
	public class ReqMissionEnd
	{
		public int missionID;
		public bool win;
		public int restHP;
		public int costTime;
	}
	[Serializable]
	public class RspMissionEnd
	{
        public int missionID;
        public bool win;
        public int restHP;
        public int costTime;

		public int gold;
		public int lv;
		public int exp;
		public int crystal;
		public int mission;
    }

    #endregion
    //错误类型的枚举
    public enum Error
	{
		None=0,//没有错误
		ServerDataError,//服务器端数据异常
		ClientDataError,//客户端数据异常

		UserIsOnline,//账号已上线
		WrongPass,//密码错误
		NameIsExist,//重名
		UpdateDataError,//更新数据失败

		LakeLevel,
		LakeGold,
		LakeCrystal,

		LakeDiamond,

		LakePower,
	}
	//RequestCode类型
	public enum CMD{
		None=0,
		//登录相关100
		ReqLogin=101,
		RspLogin=102,
		ReqRename=103,
		RspRename=104,

		//主城

		//导航
		ReqGuide=201,
		RspGuide=202,
		//强化
		ReqStrong=203,
		RspStrong=204,
		//聊天
		ReqChat=205,
		RspChat=206,
		//资源
		ReqBuy=207,
		RspBuy=208,
		//体力自动增加
		PshPower=209,
		//任务奖励
		ReqTakeTaskReward=210,
		RspTakeTaskReward=211,

		PshTaskPrgs=212,

		//副本战斗模块
		ReqMissionStart=301,
		RspMissionStart=302,

		ReqMissionEnd=303,
		RspMissionEnd=304,

	}
	public class SrvCfg 
	{
		public const string srvIP = "39.97.171.221";
		public const int srvPort = 6688;
	}
}
