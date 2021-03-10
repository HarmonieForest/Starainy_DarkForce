/****************************************************
    文件：BuyPanel.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/5 17:0:24
	功能：购买界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class BuyPanel : BasePanel 
{
    #region UI Define
    public Text txtInfo;
    public Button btnSure;
    #endregion

    private int buyType;//0为购买体力;1为金币转换

    protected override void InitPanel()
    {
        base.InitPanel();

        btnSure.interactable = true;
        RefreshUI();
    }

    public void SetBuyType(int type)
    {
        this.buyType = type;
    }
    public void RefreshUI()
    {
        switch (buyType)
        {
            case 0:
                txtInfo.text = "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" + Constants.Color("30体力", TxtColor.Blue);
                break;
            case 1:
                txtInfo.text = "是否花费" + Constants.Color("20钻石", TxtColor.Red) + "购买" + Constants.Color("5000金币", TxtColor.Blue);
                break;
        }
    }
    public void OnClickSureBtn()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        audioSvc.PlayUIAudio(Constants.FBItemEnter);

        GameMsg msg = new GameMsg
        {
            cmd =(int)CMD.ReqBuy,           
        };
        if (playerData.diamond < 20)
        {
            GameRoot.AddTips("钻石不足");
        }
        else
        {
            switch (buyType)
            {
                case 0:
                    msg.reqBuy = new ReqBuy
                    {
                        type = buyType,
                        cost = 10,
                    };
                    break;
                case 1:
                    msg.reqBuy = new ReqBuy
                    {
                        type = buyType,
                        cost = 20,
                    };
                    break;
            }
            netSvc.SendRequest(msg);
            btnSure.interactable = false;
        }          
    }
    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetPanelState(false);
    }
}