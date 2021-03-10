/****************************************************
    文件：InfoPanel.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/2 16:43:18
	功能：角色信息展示界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoPanel : BasePanel 
{
    #region UI define
    public RawImage imgChar;

    public Text txtInfo;
    public Text txtExp;
    public Image imgExpPrg;
    public Text txtPower;
    public Image imgPowerPrg;

    public Text txtJob;
    public Text txtFight;
    public Text txtHp;
    public Text txtHurt;
    public Text txtDef;

    public Button btnClose;
    public Button btnOpenDetail;
    //角色详细信息
    public Transform transDetail;

    public Button btnCloseDetail;
    public Text txtHPDetail;
    public Text txtAD;
    public Text txtAP;
    public Text txtADDef;
    public Text txtAPDef;
    public Text txtDodge;
    public Text txtPierce;
    public Text txtCritical;
    
   
    #endregion
    protected override void InitPanel()
    {
        base.InitPanel();
        SetActive(transDetail, false);
        RegTouchEvts();
        RefreshUI();
    }

    private Vector2 startPos;
    //计算出旋转角度
    private void RegTouchEvts()
    {
        OnClickDown(imgChar.gameObject, (PointerEventData evt) =>
         {
             startPos = evt.position;
             MainCitySys.Instance.SetCharStartRotate();
         });

        OnClickDrag(imgChar.gameObject, (PointerEventData evt) =>
        {
            float rotate = -(evt.position.x - startPos.x)*0.4f;
            MainCitySys.Instance.SetPlayerRotate(rotate);
        });
    }

    private void RefreshUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        SetText(txtInfo, playerData.name + " lv" + playerData.lv);
        SetText(txtExp, playerData.exp + "/" + PECommon.GetNextLevelExp(playerData.lv));
        imgExpPrg.fillAmount = playerData.exp * 1.0f / PECommon.GetNextLevelExp(playerData.lv);
        SetText(txtPower, playerData.power + "/" + PECommon.GetMaxPower(playerData.lv));
        imgPowerPrg.fillAmount = playerData.power * 1.0f / PECommon.GetMaxPower(playerData.lv);

        SetText(txtJob, "职业  暗夜刺客");
        SetText(txtFight, "战力  " + PECommon.GetFightByProps(playerData));
        SetText(txtHp, "血量  " + playerData.hp);
        SetText(txtHurt, "伤害  " + (playerData.ad + playerData.ap));
        SetText(txtDef, "防御  " + (playerData.addef + playerData.apdef));
        //详细属性赋值
        SetText(txtHPDetail, playerData.hp);
        SetText(txtAD, playerData.ad);
        SetText(txtAP, playerData.ap);
        SetText(txtADDef, playerData.addef);
        SetText(txtAPDef, playerData.apdef);
        SetText(txtDodge, playerData.dodge);
        SetText(txtPierce, playerData.pierce);
        SetText(txtCritical, playerData.critical);
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        MainCitySys.Instance.CloseInfoPanel();
    }
    public void OnclickOpenDetailBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetActive(transDetail, true);
    }

    public void OnClickCloseDetailBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetActive(transDetail, false);
    }
}