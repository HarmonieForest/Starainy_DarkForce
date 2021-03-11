/****************************************************
    文件：TaskPanel.cs
	作者：Harmonie
	功能：任务奖励系统
*****************************************************/

using PEProtocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanel : BasePanel 
{
    public Transform scrollTrans;

    private PlayerData playerData = null;
    private List<TaskRewardState> trsLst  = new List<TaskRewardState>();
    protected override void InitPanel()
    {
        base.InitPanel();

        playerData = GameRoot.Instance.PlayerData;
        RefreshUI();
    }


    public void OnClickCloseBtn()
    {
        SetPanelState(false);
    }

    public void RefreshUI()
    {
        trsLst.Clear();
        List<TaskRewardState> todoLst = new List<TaskRewardState>();
        List<TaskRewardState> doneLst = new List<TaskRewardState>();
        for(int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskInfo = playerData.taskArr[i].Split('|');
            TaskRewardState trs = new TaskRewardState
            {
                ID = int.Parse(taskInfo[0]),
                prgs = int.Parse(taskInfo[1]),
                taked = taskInfo[2].Equals("1"),
            };
            if (trs.taked)
            {
                doneLst.Add(trs);
            }
            else
            {
                todoLst.Add(trs);
            }
        }

        trsLst.AddRange(todoLst);
        trsLst.AddRange(doneLst);
        for(int i = 0; i < scrollTrans.childCount; i++)
        {
            Destroy(scrollTrans.GetChild(i).gameObject);
        }

        for(int i = 0; i < trsLst.Count; i++)
        {
            GameObject go = resSvc.LoadPrefab(PathDefine.TaskItemPrefab);
            go.transform.SetParent(scrollTrans);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.name = "taskItem_" + i;

            TaskRewardState trs = trsLst[i];
            TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trs.ID);

            SetText(GetTrans(go.transform, "txtName"),trc.taskName);

            SetText(GetTrans(go.transform, "txtPrg"),trs.prgs+"/"+trc.count); 
            SetText(GetTrans(go.transform, "txtexp"), trc.exp);
            SetText(GetTrans(go.transform, "txtgold"), trc.gold);

            Image imgprgs = GetTrans(go.transform, "prgBar/prgval").GetComponent<Image>();
            float prgval = trs.prgs * 1.0f / trc.count;
            imgprgs.fillAmount = prgval;

            Button btnTake = GetTrans(go.transform, "btnTake").GetComponent<Button>();

            btnTake.onClick.AddListener(() =>
            {
                OnClickBtnTake(go.name);
            });

            Transform transComp = GetTrans(go.transform, "imgcomp");
            if (trs.taked)
            {
                btnTake.interactable = false;
                SetActive(transComp);
            }
            else
            {             
                SetActive(transComp, false);
                if (trs.prgs == trc.count)
                {
                    btnTake.interactable = true;
                }
                else
                {
                    btnTake.interactable = false;
                }

            }
        }
    }

    private void OnClickBtnTake(string name)
    {
        string[] nameArr = name.Split('_');
        int index = int.Parse(nameArr[1]);
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqTakeTaskReward,
            reqTakeTaskReward=new ReqTakeTaskReward
            {
                rid=trsLst[index].ID,
            },
        };
        netSvc.SendRequest(msg);

        TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trsLst[index].ID);
        int gold = trc.gold;
        int exp = trc.exp;
        GameRoot.AddTips(Constants.Color("奖励:", TxtColor.Blue) + Constants.Color("金币 " + gold + "经验" + exp, TxtColor.Green));
    }
    
}