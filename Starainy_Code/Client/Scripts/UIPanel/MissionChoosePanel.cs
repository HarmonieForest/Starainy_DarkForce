/****************************************************
    文件：MissionPanel.cs
	作者：Harmonie
	功能：副本选择界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class MissionChoosePanel : BasePanel 
{
    private PlayerData playerData;

    public Button[] btnMissionArr;
    public Transform curtPointImg;
    protected override void InitPanel()
    {
        playerData = GameRoot.Instance.PlayerData;
        base.InitPanel();

        RefreshUI();
    }

    public void RefreshUI()
    {
        int missionID = playerData.mission;
        for(int i = 0; i < btnMissionArr.Length; i++)
        {
            if (i < missionID % 10000)
            {
                SetActive(btnMissionArr[i].gameObject);
                if (i == missionID % 10000 - 1)
                {
                    curtPointImg.SetParent(btnMissionArr[i].transform);
                    curtPointImg.localPosition = new Vector3(25, 100, 0);
                }
            }
            else
            {
                SetActive(btnMissionArr[i].gameObject, false);
            }
        }
    }
    #region 点击事件
    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetPanelState(false);
    }
    public void OnClickBtnMission(int missionID)
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        int power = resSvc.GetMapCfg(missionID).power;
        if (playerData.power < power)
        {
            GameRoot.AddTips("体力不足");
        }
        else
        {
            netSvc.SendRequest(new GameMsg
            {
                cmd = (int)CMD.ReqMissionStart,
                reqMissionStart = new ReqMissionStart
                {
                    missionID = missionID
                }
            }) ;
        }
    }
    #endregion
}