

using PEProtocol;
using UnityEngine;

public class GameRoot : MonoBehaviour 
{
    
    public static GameRoot Instance = null;
    //加载场景和动态场景界面经常使用,因而交给GameRoot进行管理,方便调用
   
    public LoadingPanel loadingPanel;
    public DynamicPanel dynamicPanel;
    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        PECommon.Log("GameStart");

        ClearUIRoot();
        Init();
    }
    //关闭所有UI,确保只有dynamicPanel打开
    private void ClearUIRoot()
    {
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }   
    }
    private void Init()
    {
        //服务模块初始化
        ResSvc resSvc = GetComponent<ResSvc>();
        resSvc.InitSvc();
        AudioSvc audioSvc = GetComponent<AudioSvc>();
        audioSvc.InitSvc();
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitSvc();
        TimerSvc timerSvc = GetComponent<TimerSvc>();
        timerSvc.InitSvc();

        //业务系统初始化
        LoginSys login = GetComponent<LoginSys>();
        login.InitSys();
        MainCitySys mainCitySys = GetComponent<MainCitySys>();
        mainCitySys.InitSys();
        MissionSys missionSys = GetComponent<MissionSys>();
        missionSys.InitSys();
        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();

        dynamicPanel.SetPanelState();
        //进入登录场景并加载相应UI
        login.EnterLogin();   
    }

    //将添加tips设置为API方便其他对象进行调用
    public static void AddTips(string tips)
    {
        Instance.dynamicPanel.AddTips(tips);
    }

    //存储用户数据
    private PlayerData playerData = null;
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }
    public void SetPlayerData(RspLogin data)
    {
        playerData = data.playerData;
    }
    public void SetPlayerName(string name)
    {
        playerData.name = name;
    }
    public void SetPlayerDataByGuide(RspGuide data)
    {
        playerData.gold = data.gold;
        playerData.exp = data.exp;
        playerData.lv = data.lv;
        playerData.guideid = data.guideid;
    }

    public void SetPlayerDataByStrong(RspStrong data)
    {
        playerData.gold = data.gold;
        playerData.crystal = data.crystal;
        playerData.ad = data.ad;
        playerData.addef = data.addef;
        playerData.ap = data.ap;
        playerData.apdef = data.apdef;
        playerData.hp = data.hp;

        playerData.strongArr = data.strongArr;
    }
    public void SetPlayerDataByBuy(RspBuy data)
    {
        playerData.gold = data.gold;
        playerData.diamond = data.diamond;
        playerData.power = data.power;
    }

    public void SetPlayerDataByPower(PshPower data)
    {
        playerData.power = data.power;
    }
    public void SetPlayerDataByTakeTaskReward(RspTakeTaskReward data)
    {
        playerData.taskArr = data.taskArr;
        playerData.gold = data.gold;
        playerData.lv = data.lv;
        playerData.exp = data.exp;
    }
    public void SetPlayerDataByPshTaskPrgs(PshTaskPrgs data)
    {
        playerData.taskArr = data.taskArr;
    }
    public void SetPlayerDataByMissionStart(RspMissionStart data)
    {
        playerData.power = data.power;    
    }
    public void SetPlayerDataByMissionEnd(RspMissionEnd data)
    {
        playerData.gold = data.gold;
        playerData.exp = data.exp;
        playerData.lv = data.lv;
        playerData.crystal = data.crystal;
        playerData.mission = data.mission;
    }
}