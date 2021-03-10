/****************************************************
    文件：StrongPanel.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/4 13:27:17
	功能：强化面板
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class StrongPanel : BasePanel 
{
    #region UI Define
    public Image imgCurtPos;
    public Text txtStartLv;
    public Transform starGroupTrans;

    public Text txtHP1;
    public Text txtHP2;
    public Text txtHurt1;
    public Text txtHurt2;
    public Text txtDef1;
    public Text txtDef2;

    public Text txtNeedLv;
    public Text txtCostGold;
    public Text txtCostCrystal;

    public Text txtCurtGold;

    public Button btnStrong;

    public Transform costInfoTrans;

    public Image propArray1;
    public Image propArray2;
    public Image propArray3;
    #endregion

    #region data area
    public Transform postBtnTrans;

    public Button closePanelBtn;

    private Image[] imgs=new Image[6];

    private int curtIndex;

    private PlayerData playerData;

    private StrongCfg nextData;
    #endregion
    protected override void InitPanel()
    {
        base.InitPanel();
        playerData = GameRoot.Instance.PlayerData;
        RegClickEvts();
        ClickPostItem(0);
    }


    private void RegClickEvts()
    {
        for(int i = 0; i < postBtnTrans.childCount; i++)
        {
            Image img = postBtnTrans.GetChild(i).GetComponent<Image>();

            OnClick(img.gameObject, (object args) =>
             {
                 audioSvc.PlayUIAudio(Constants.UIClickBtn);
                 ClickPostItem((int)args);
             },i);
            imgs[i] = img;
        }
    }

    private void ClickPostItem(int index)
    { 
        curtIndex = index;
        for(int i = 0; i < imgs.Length; i++)
        {
            Transform trans = imgs[i].transform;
            if (i == curtIndex)
            {
                SetSprite(imgs[i], PathDefine.ItemArrorBg);
                trans.localPosition = new Vector3(10, trans.localPosition.y, 0);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 95);
            }
            else
            {
                SetSprite(imgs[i], PathDefine.ItemPlatBg);
                trans.localPosition = new Vector3(0, trans.localPosition.y, 0);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 85);
            }
        }
        RefreshItem();
    }
    private void RefreshItem()
    {
        SetText(txtCurtGold, playerData.gold);

        switch (curtIndex)
        {
            case 0:
                SetSprite(imgCurtPos, PathDefine.ItemTokui);
                break;
            case 1:
                SetSprite(imgCurtPos, PathDefine.ItemBody);
                break;
            case 2:
                SetSprite(imgCurtPos, PathDefine.ItemYaobu);
                break;
            case 3:
                SetSprite(imgCurtPos, PathDefine.ItemHand);
                break;
            case 4:
                SetSprite(imgCurtPos, PathDefine.ItemLeg);
                break;
            case 5:
                SetSprite(imgCurtPos, PathDefine.ItemFoot);
                break;
        }

        SetText(txtStartLv, playerData.strongArr[curtIndex] + "星级");
        int curtStarLv = playerData.strongArr[curtIndex];
        for(int i = 0; i < starGroupTrans.childCount; i++)
        {
            Image img = starGroupTrans.GetChild(i).GetComponent<Image>();
            if (i < curtStarLv)
            {
                SetSprite(img, PathDefine.SpStar2);
            }
            else
            {
                SetSprite(img, PathDefine.SpStar1);
            }
        }
        
        int nextStarLv = curtStarLv + 1;
        int sumAddhp = resSvc.GetPropAddValPreLv(curtIndex, nextStarLv, 1);
        int sumAddhurt = resSvc.GetPropAddValPreLv(curtIndex, nextStarLv, 2);
        int sumAdddef = resSvc.GetPropAddValPreLv(curtIndex, nextStarLv, 3);
        SetText(txtHP1,"+"+ sumAddhp);
        SetText(txtHurt1, "+"+sumAddhurt);
        SetText(txtDef1, "+"+sumAdddef);

       
        nextData = resSvc.GetStrongCfg(curtIndex, nextStarLv);
        if (nextData != null)
        {
            SetActive(txtHP2, true);
            SetActive(txtDef2, true);
            SetActive(txtHurt2, true);

            SetActive(costInfoTrans, true);
            SetActive(propArray1, true);
            SetActive(propArray2, true);
            SetActive(propArray3, true);

            SetText(txtHP2, "+"+nextData.addhp);
            SetText(txtHurt2,"+"+ nextData.addhurt);
            SetText(txtDef2, "+"+nextData.adddef);

            SetText(txtNeedLv, nextData.minlv);
            SetText(txtCostGold, nextData.gold);
            SetText(txtCostCrystal, nextData.crystal + "/" + playerData.crystal);

        }
        else
        {
            SetActive(txtHP2, false);
            SetActive(txtDef2, false);
            SetActive(txtHurt2, false);

            SetActive(costInfoTrans, false);
            SetActive(propArray1, false);
            SetActive(propArray2, false);
            SetActive(propArray3, false);

        }
    }





    public void OnClickClosePanelBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        SetPanelState(false);

    }
    public void OnClickStrongBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        //本地数据校验
        if (playerData.strongArr[curtIndex] < 10)
        {
            if (playerData.lv < nextData.minlv)
            {
                GameRoot.AddTips("角色等级不够");
                return;
            }
            if (playerData.gold < nextData.gold)
            {
                GameRoot.AddTips("金币不够");
                return;
            }
            if (playerData.crystal < nextData.crystal)
            {
                GameRoot.AddTips("水晶不够");
                return;
            }
            netSvc.SendRequest(new GameMsg
            {
                cmd = (int)CMD.ReqStrong,
                reqStrong = new ReqStrong
                {
                    pos = curtIndex,
                }
            }) ;
        }
        else
        {
            GameRoot.AddTips("星级已升满");
        }
        
    }

    public void UptateUI()
    {
        audioSvc.PlayUIAudio(Constants.FBItemEnter);
        ClickPostItem(curtIndex);
    }
    
}