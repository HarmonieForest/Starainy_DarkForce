/****************************************************
    文件：MissionSys.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/12 19:11:22
	功能：副本业务系统
*****************************************************/

using PEProtocol;
using System.Security.Cryptography;
using UnityEngine;

public class MissionSys : BaseSystem 
{
    public static MissionSys Instance = null;
    public MissionChoosePanel missionChoosePanel;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init MissionSys.....");
    }
    public void EnterMission()
    {
        SetMissionChoosePanelState();
    }
    #region MissionChoosePanel
    public void SetMissionChoosePanelState(bool isactive=true)
    {
        missionChoosePanel.SetPanelState(isactive);
    }
    public void RspMissionStart(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByMissionStart(msg.rspMissionStart);

        MainCitySys.Instance.mainCityPanel.SetPanelState(false);
        SetMissionChoosePanelState(false);

        BattleSys.Instance.StartBattle(msg.rspMissionStart.missionID);
    }
    #endregion
}