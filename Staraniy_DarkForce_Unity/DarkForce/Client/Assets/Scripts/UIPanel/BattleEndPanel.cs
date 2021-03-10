/****************************************************
    文件：BattleEndPanel.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/17 8:28:29
	功能：战斗结算页面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class BattleEndPanel : BasePanel 
{
    #region UI Define
    public Transform rewardTrans;
    public Button btnClose;
    public Button btnSure;
    public Button btnExit;

    public Text txtTime;
    public Text txtRestHP;
    public Text txtReward;

    public Animation ani;

    #endregion
    private MissionEndType endType = MissionEndType.None;
    protected override void InitPanel()
    {
        base.InitPanel();
        RefreshUI();
    }
    private void RefreshUI()
    {
        switch (endType)
        {
            case MissionEndType.Pause:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject);
                break;
            case MissionEndType.Win:
                SetActive(rewardTrans,false);
                SetActive(btnExit.gameObject,false);
                SetActive(btnClose.gameObject,false);

                MapCfg mapCfg = resSvc.GetMapCfg(missionID);
                timerSvc.AddTimeTask((int tid) =>
                {
                    SetActive(rewardTrans);
                    ani.Play();
                    timerSvc.AddTimeTask((int tid1) =>
                    {
                        audioSvc.PlayUIAudio(Constants.FBItemEnter);
                        timerSvc.AddTimeTask((int tid2) =>
                        {
                            audioSvc.PlayUIAudio(Constants.FBItemEnter);   
                            timerSvc.AddTimeTask((int tid3) =>
                            {
                                audioSvc.PlayUIAudio(Constants.FBItemEnter);
                                timerSvc.AddTimeTask((int tid4) =>
                                {
                                    audioSvc.PlayUIAudio(Constants.FBWin);
                                }, 300);
                            }, 270);

                        }, 270);
                    },325);
                }, 1000);
                int min = costTime / 60;
                int sec = costTime % 60;
                int gold = mapCfg.gold;
                int crystal = mapCfg.crystal;
                int exp = mapCfg.exp;
                SetText(txtRestHP,"剩余血量"+ restHP);
                SetText(txtTime, "通关时间:" + min + "分" + sec + "秒");
                SetText(txtReward, "通关奖励:" + "金币" + gold + "经验" + exp + "水晶" + crystal);
                break;
            case MissionEndType.Lose:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject,false);
                audioSvc.PlayUIAudio(Constants.FBLose);
                break;
            case MissionEndType.None:
                break;
        }
    }
    private int missionID;
    private int costTime;
    private int restHP;
    public void SetBattleEndData(int missionID,int costTime,int restHP)
    {
        this.missionID = missionID;
        this.costTime = costTime;
        this.restHP = restHP;
    }
    public void SetMissionEndType(MissionEndType endType)
    {
        this.endType = endType;
    }
    public void OnClickClosBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        BattleSys.Instance.battleMng.isPauseGame = false;
        SetPanelState(false);
    }

    public void OnClickExitBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        //进入主城界面,销毁当前战斗      
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
    }
    public void OnClickSureBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        //进入主城界面,销毁当前战斗
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        MissionSys.Instance.EnterMission();
    }
}
public enum MissionEndType
{
    None,
    Pause,
    Win,
    Lose,
}