/****************************************************
    文件：MainCitySys.cs
	作者：Harmonie
	功能：主城业务系统
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.AI;

public class MainCitySys : BaseSystem 
{
    #region 基础设置
    public static MainCitySys Instance = null;

    public MainCityPanel mainCityPanel;
    public InfoPanel infoPanel;
    public GuidePanel guidePanel;
    public StrongPanel strongPanel;
    public ChatPanel chatPanel;
    public BuyPanel buyPanel;
    public TaskPanel taskPanel;
  
    private PlayerController playerCtrl;

    private Transform charCam;

    private AutoGuideCfg curtTaskData;
    private Transform[] npcPosTrans;
    private NavMeshAgent nav;

    private GameObject player;
    #endregion

    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init MainCitySys");
    }   
    public void EnterMainCity()
    {
        MapCfg mapData = resSvc.GetMapCfg(Constants.MainCityMapID);            
        resSvc.AsyncLoadScene(mapData.sceneName,()=> {
            PECommon.Log("Entr MainCityScene");
      
            LoadPlayer(mapData);
          
            mainCityPanel.SetPanelState(true);

            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;
         
            audioSvc.PlayBgMusic(Constants.BgMainCity);
          
            GameObject mapRoot = GameObject.FindGameObjectWithTag("MapRoot");
            MainCityMap mainCityMap = mapRoot.GetComponent<MainCityMap>();
            npcPosTrans = mainCityMap.NpcPosTrans;
          
            if (charCam != null)
            {
                charCam.gameObject.SetActive(false);
            }
            GameRoot.AddTips("欢迎进入暗黑战神的世界");
        });
    }

    private void LoadPlayer(MapCfg mapData)
    {
        player = resSvc.LoadPrefab(PathDefine.AssassinCityPlayerPrefab,true);
      
        player.transform.localPosition = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);      
        
        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.enabled = true;
        playerCtrl.Init();

        nav = player.GetComponent<NavMeshAgent>();
    }
    public void SetMoveDir(Vector2 dir)
    {
        StopNavGuide();
        if (dir == Vector2.zero)
        {
            playerCtrl.SetBlend(Constants.BlendIdle);
        }
        else
        {
            playerCtrl.SetBlend(Constants.BlendMove);
        }
        playerCtrl.Dir = dir;
    }
   
    #region 角色信息界面
    public void OpenInfoPanel()
    {
        StopNavGuide();
        infoPanel.SetPanelState(true);
        if (charCam == null)
        {
            charCam = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }
        //设置任务展示相机的相对位置
        charCam.localPosition = playerCtrl.transform.position + playerCtrl.transform.forward * 3.8f+new Vector3(0,1.2f,0);
        charCam.localEulerAngles = new Vector3(0, 180 + playerCtrl.transform.localEulerAngles.y, 0);
        charCam.localScale = Vector3.one;
        charCam.gameObject.SetActive(true);
    }

    public void CloseInfoPanel()
    {
        if (charCam != null)
        {
            charCam.gameObject.SetActive(false);
            infoPanel.SetPanelState(false);
        }
    }
    private float charStartRotate;
    public void SetCharStartRotate()
    {
        charStartRotate = playerCtrl.transform.localEulerAngles.y;
    }
    public void SetPlayerRotate(float rotate)
    {
        playerCtrl.transform.localEulerAngles = new Vector3(0, charStartRotate + rotate, 0);
    }
    #endregion


    #region 自动人物导航
    private bool isNavGuide = false;
    public void RunTask(AutoGuideCfg agcfg)
    {
        player.GetComponent<Animator>().applyRootMotion = false;
        if (agcfg != null)
        {
            curtTaskData = agcfg;
        }
        nav.enabled = true;
        //解析任务数据
        if (curtTaskData.npcID != -1)
        {
            float dis = Vector3.Distance(playerCtrl.transform.position, npcPosTrans[curtTaskData.npcID].position);
            
            if (dis < 0.5f)
            {

                isNavGuide = false;
                nav.isStopped = true;
                playerCtrl.SetBlend(Constants.BlendIdle);
                nav.enabled = false;
                OpenGuidePanel();
            }
            else
            {
                isNavGuide = true;
                nav.enabled = true;
                nav.speed = Constants.PlayerMoveSpeed;
                nav.SetDestination(npcPosTrans[agcfg.npcID].position);
                playerCtrl.SetBlend(Constants.BlendMove);
            }
        }
        else
        {
            OpenGuidePanel();
        }
    }
    private void StopNavGuide()
    {
        if (isNavGuide)
        {
            isNavGuide = false;
            nav.isStopped = true;
            nav.enabled = false;
            playerCtrl.SetBlend(Constants.BlendIdle);
            player.GetComponent<Animator>().applyRootMotion = true;
        }
    }
    private void IsArriveNavPos()
    {
        float dis = Vector3.Distance(playerCtrl.transform.position, npcPosTrans[curtTaskData.npcID].position);
        if (dis < 0.5f)
        {
            isNavGuide = false;
            nav.isStopped = true;
            playerCtrl.SetBlend(Constants.BlendIdle);
            nav.enabled = false;
            OpenGuidePanel();
        }
    }
    private void Update()
    {
        if (isNavGuide)
        {
            IsArriveNavPos();
            playerCtrl.SetCam();
        }
    }
    private void OpenGuidePanel()
    {
        guidePanel.SetPanelState(true);
    }
    

    //获取当前的data
    public AutoGuideCfg GetCurtTaskData()
    {
        return curtTaskData;
    }

    public void RspGuide(GameMsg msg)
    {
        GameRoot.AddTips(Constants.Color("任务奖励 金币:" + curtTaskData.gold + "经验:" + curtTaskData.gold,TxtColor.Blue));

        switch (curtTaskData.actID)
        {
            case 0:
                //与智者对话

                break;
            case 1:
                //进入副本
                EnterMission();
                break;
            case 2:
                //进入强化界面
                OpenStrongPanel();
                break;
            case 3:
                //进入体力界面
                OpenBuyPanel(0);
                break;
            case 4:
                //进入金币购买
                OpenBuyPanel(1);
                break;
            case 5:
                OpenChatPanel();
                //进入聊天系统
                break;
        }

        GameRoot.Instance.SetPlayerDataByGuide(msg.rspGuide);
        mainCityPanel.RefreshUI();
    }
    #endregion

    #region 强化升级界面
    public void OpenStrongPanel()
    {
        StopNavGuide();
        strongPanel.SetPanelState(true);
    }
    public void RspStrong(GameMsg msg)
    {
        int zhanliPre = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.Instance.SetPlayerDataByStrong(msg.rspStrong);
        int zhanliNow = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.AddTips(Constants.Color("战力提升了" + (zhanliNow - zhanliPre),TxtColor.Blue));

        strongPanel.UptateUI();
        mainCityPanel.RefreshUI();
    }
    #endregion
    #region 聊天界面
    public void OpenChatPanel()
    {
        StopNavGuide();
        chatPanel.SetPanelState(true);
    }
    public void RspChat(GameMsg msg)
    {
        chatPanel.AddChatMsg(msg.rspChat.name, msg.rspChat.chat);
    }
    #endregion

    #region 购买界面
    public void OpenBuyPanel(int type)
    {
        StopNavGuide();
        buyPanel.SetBuyType(type);
        buyPanel.SetPanelState(true);
    }
    public void RspBuy(GameMsg msg)
    {
        int powerpre = GameRoot.Instance.PlayerData.power;
        int goldpre = GameRoot.Instance.PlayerData.gold;
        GameRoot.Instance.SetPlayerDataByBuy(msg.rspBuy);
        int powerNow = GameRoot.Instance.PlayerData.power;
        int goldnow = GameRoot.Instance.PlayerData.gold;
        switch (msg.rspBuy.type)
        {
            case 0:
                GameRoot.AddTips("成功获得" + Constants.Color((powerNow - powerpre) + "体力", TxtColor.Blue));
                break;
            case 1:
                GameRoot.AddTips("成功获得" + Constants.Color((goldnow - goldpre) + "金币", TxtColor.Blue));
                break;
        }
        mainCityPanel.RefreshUI();
        buyPanel.SetPanelState(false);

        if (msg.pshTaskPrgs != null)
        {
            GameRoot.Instance.SetPlayerDataByPshTaskPrgs(msg.pshTaskPrgs);

            if (taskPanel.GetPanelState())
            {
                taskPanel.RefreshUI();
            }
        }
    }
    #endregion
    #region 体力自动更新
    public void PshPower(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByPower(msg.pshPower);
        if (mainCityPanel.GetPanelState())
        {
            mainCityPanel.RefreshUI();
        }
       
    }
    #endregion

    #region 任务奖励界面
    public void OpenTaskRewardPanel()
    {
        StopNavGuide();
        taskPanel.SetPanelState(true);
    }

    public void PshTaskPrgs(GameMsg msg)
    {
        
        GameRoot.Instance.SetPlayerDataByPshTaskPrgs(msg.pshTaskPrgs);
        
        if (taskPanel.GetPanelState())
        {
            taskPanel.RefreshUI();
        }
    }
    public void RspTakeTaskReward(GameMsg msg)
    {
       
        GameRoot.Instance.SetPlayerDataByTakeTaskReward(msg.rspTakeTaskReward);
        taskPanel.RefreshUI();
        mainCityPanel.RefreshUI();

    }
    #endregion

    #region 进入副本系统
    public void EnterMission()
    {
        StopNavGuide();
        MissionSys.Instance.EnterMission();
    }
    #endregion
}