

using PEProtocol;
using UnityEngine;

public class BattleSys : BaseSystem 
{
    public static BattleSys Instance = null;

    public PlayerCtrlPanel playerCtrlPanel;
    public BattleEndPanel battleEndPanel;

    public BattleMng battleMng = null;

    private int missionID;
    private double startTime;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        PECommon.Log("Init BattleSys.....");
    }
    public void StartBattle(int missionID)
    {
        this.missionID = missionID;
        GameObject go = new GameObject
        {
            name = "BattleRoot"
        };
        go.transform.SetParent(GameRoot.Instance.transform);
        battleMng = go.AddComponent<BattleMng>();
        battleMng.InitMission(missionID,()=> {
            startTime = timerSvc.GetNowTime();
        });
        SetPlayerCtrlPanelState();
    }
    public void DestroyBattle()
    {
        GameRoot.Instance.dynamicPanel.RemoveAllHpItemInfo();
        SetPlayerCtrlPanelState(false);
        SetBattleEndPanelState(MissionEndType.None, false);
        
        Destroy(battleMng.gameObject);
    }

    public void EndBattle(bool iswin,int restHP)
    {
        playerCtrlPanel.SetPanelState(false);
        GameRoot.Instance.dynamicPanel.RemoveAllHpItemInfo();
        if (iswin)
        {
            double endTime = timerSvc.GetNowTime();
            //发送战斗结算请求
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqMissionEnd,
                reqMissionEnd = new ReqMissionEnd
                {
                    win = iswin,
                    missionID = missionID,
                    costTime = (int)(endTime - startTime) / 1000,
                    restHP=restHP,
                },
            };
            netSvc.SendRequest(msg);
        }
        else
        {
            SetBattleEndPanelState(MissionEndType.Lose);
        }
    }

    public void SetBattleEndPanelState(MissionEndType endType,bool isActive=true)
    {
        battleEndPanel.SetMissionEndType(endType);
        battleEndPanel.SetPanelState(isActive);
    }
    public void SetPlayerCtrlPanelState(bool isActive=true)
    {
        playerCtrlPanel.SetPanelState(isActive);
    }
    public void SetSelfPlayerMoveDirection(Vector2 dir)
    {
        battleMng.SetSelfPlayerMoveDirection(dir);
    }
    public void ReqReleaseSkill(int index)
    {
        battleMng.ReqReleaseSkill(index);
    }

    public Vector2 GetCurtDirInput()
    {
        return playerCtrlPanel.curtDir;
    }



    public void RspMissionEnd(GameMsg msg)
    {
        RspMissionEnd data = msg.rspMissionEnd;
        GameRoot.Instance.SetPlayerDataByMissionEnd(data);
        battleEndPanel.SetBattleEndData(data.missionID, data.costTime, data.restHP);
        SetBattleEndPanelState(MissionEndType.Win);
        
    }
}