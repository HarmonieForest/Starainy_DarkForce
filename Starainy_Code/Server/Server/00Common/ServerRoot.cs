
/****************************************************
	文件：ServerRoot.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/04/24 15:23   	
	功能：服务器初始化
*****************************************************/
using System.Collections.Generic;
using System.Diagnostics;

public  class ServerRoot
{
	private static ServerRoot instance = null;
	public static ServerRoot Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ServerRoot();
			}
			return instance;
		}
	}
	//初始化方法
	public void Init()
	{
		//数据层
		DBMng.Instance.Init();

		//服务层
		NetSvc.Instance.Init();
		CacheSvc.Instance.Init();
		CfgSvc.Instance.Init();
		TimerSvc.Instance.Init();
		
		//业务系统层
		LoginSys.Instance.Init();
		GuideSys.Instance.Init();
		StrongSys.Instance.Init();
		ChatSys.Instance.Init();
		BuySys.Instance.Init();
		PowerSys.Instance.Init();
		TaskSys.Instance.Init();
		MissionSys.Instance.Init();
	}
	public void Update()
	{
		NetSvc.Instance.Update();
		TimerSvc.Instance.Update();
	}

	private int SessionID = 0;
	public int GetSessonID()
	{
		if (SessionID == int.MaxValue)
		{
			SessionID = 0;
		}
		return SessionID += 1;
	}

}

