/****************************************************
    文件：GuidePanel.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/3 20:22:37
	功能：对话界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class GuidePanel : BasePanel 
{
    public Text txtName;
    public Text txtTalk;
    public Image imgIcon;
    public Button btnNext;

    private PlayerData playerData;
    private AutoGuideCfg curtTaskData;

    private string[] dialogArr;
    private int index;

    protected override void InitPanel()
    {
        base.InitPanel();
        playerData = GameRoot.Instance.PlayerData;
        curtTaskData=MainCitySys.Instance.GetCurtTaskData();
        dialogArr = curtTaskData.dilogArr.Split('#');
        index = 1;

        SetTalk();
    }
    private void SetTalk()
    {
        string[] talkArr = dialogArr[index].Split('|');
        if (talkArr[0] == "0")
        {
            //自己
            SetSprite(imgIcon, PathDefine.selfIcon);
            SetText(txtName, playerData.name);
        }
        else
        {
            //npc
            switch (curtTaskData.npcID)
            {
                case 0:
                    SetSprite(imgIcon, PathDefine.Npc0);
                    SetText(txtName, "智者");
                    break;
                case 1:
                    SetSprite(imgIcon, PathDefine.Npc1);
                    SetText(txtName, "将军");
                    break;
                case 2:
                    SetSprite(imgIcon, PathDefine.Npc2);
                    SetText(txtName, "工匠");
                    break;
                case 3:
                    SetSprite(imgIcon, PathDefine.Npc3);
                    SetText(txtName, "商人");
                    break;
                default:
                    SetSprite(imgIcon, PathDefine.NpcGuideImg);
                    SetText(txtName, "琼儿");
                    break;

            }
        }

        imgIcon.SetNativeSize();
        SetText(txtTalk, talkArr[1].Replace("$name", playerData.name));
    }

    public void OnClickNextBtn()
    {
        index += 1;
        
        if (index == dialogArr.Length)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqGuide,
                reqGuide = new ReqGuide
                {
                    guideid = curtTaskData.ID,
                },
            };
            netSvc.SendRequest(msg);
            SetPanelState(false);
        }
        else
        {
            SetTalk();
        }
    }
}