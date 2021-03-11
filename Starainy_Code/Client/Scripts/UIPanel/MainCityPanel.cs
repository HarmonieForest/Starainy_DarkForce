/****************************************************
    文件：MainCityPanel.cs
	作者：Harmonie
	功能：主城页面管理
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCityPanel : BasePanel 
{
    #region UIDefine
    public Text txtFight;
    public Text txtPower;
    public Text txtVip;
    public Image imgPowerPrg;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;
    public Transform expTrans;

    public Animation menuAnim;

    public Button btnMenu;
    public Button btnHead;
    public Button btnChat;
   
    //轮盘控制组件
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    private float pointDis;

    //导航系统
    private AutoGuideCfg curtGuideData;
    public Button btnGuide;
    #endregion

    public Button btnStrong;
    #region MainFunctons
    protected override void InitPanel()
    {
        base.InitPanel();
        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOpDis;
        SetActive(imgDirPoint, false);
        defaultPosition = imgDirBg.transform.position;
        RegisterTouchEvents();
        RefreshUI();
    }

    //实时更新UI面板数据
    public void RefreshUI()
    {
        #region 游戏人物数据
        PlayerData playerData = GameRoot.Instance.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(playerData));
        SetText(txtLevel, playerData.lv);
        SetText(txtName, playerData.name);
        SetText(txtPower, "体力:" + playerData.power + "/" + PECommon.GetMaxPower(playerData.lv));
        imgPowerPrg.fillAmount = playerData.power * 1.0f / PECommon.GetMaxPower(playerData.lv);
        #endregion
        #region 底部经验条显示
        GridLayoutGroup grid = expTrans.GetComponent<GridLayoutGroup>();
        float globalRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;
        float screenWidth = Screen.width * globalRate;
        float width = (screenWidth-166) / 10;
        grid.cellSize = new Vector2(width, 7);

        int expPrgVal = (int)(playerData.exp * 1.0f / PECommon.GetNextLevelExp(playerData.lv) * 100);
        SetText(txtExpPrg, expPrgVal + "%");
        int index = expPrgVal / 10;
        //遍历每一个
        for(int i = 0; i < expTrans.childCount; i++)
        {
            Image img = expTrans.GetChild(i).GetComponent<Image>();
            if (i < index)
            {
                img.fillAmount = 1;
            }
            else if(i ==index){
                img.fillAmount = expPrgVal % 10 * 1.0f / 10;
            }else if (i > index)
            {
                img.fillAmount = 0;
            }
        }
        #endregion
        #region 导航UI显示
        curtGuideData = resSvc.GetAutoGuideCfg(playerData.guideid);
        if (curtGuideData!= null)
        {
            SetGuideBtnIcon(curtGuideData.npcID);           
        }
        else
        {
            SetGuideBtnIcon(-1);
        }
        #endregion
    }
    private void SetGuideBtnIcon(int npcID)
    {
        string spath = "";
        Image img = btnGuide.GetComponent<Image>();
        switch (npcID)
        {
            case Constants.NPCWiseman:
                spath = PathDefine.WisemanHead;
                break;
            case Constants.NPCGeneral:
                spath = PathDefine.GeneralHead;
                break;
            case Constants.NPCArtisan:
                spath = PathDefine.ArtisanHead;
                break;
            case Constants.NPCTrader:
                spath = PathDefine.TraderHead;
                break;
            default:
                spath = PathDefine.TaskHead;
                break;
        }
        SetSprite(img, spath);
    }
    public void Update()
    {
        
    }
    #endregion

    private bool menuIsOpen = true;
    private Vector2 startPosition = Vector2.zero;
    private Vector2 defaultPosition = Vector2.zero;
    #region ClickEvents
    public void OnClickBtnMenu()
    {
        audioSvc.PlayUIAudio(Constants.UiExtenBtn);
        
        AnimationClip clip = null;
        if (menuIsOpen)
        {
            clip = menuAnim.GetClip("CloseMenuAnim");
        }
        else
        {
            clip = menuAnim.GetClip("OpenMenuAnim");
        }
        menuAnim.Play(clip.name);
        menuIsOpen = !menuIsOpen;
    }
    public void OnClickHeadBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenInfoPanel();
    
    }
    public void OnClickTaskBtn()
    {
        if (curtGuideData != null)
        {
            MainCitySys.Instance.RunTask(curtGuideData);
        }
        else
        {
            GameRoot.AddTips("更多人物开发中,敬请期待");
        }
    }
    public void OnClickStrongBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenStrongPanel();
    }
    public void OnClickBuyPowerBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyPanel(0);
    }
    public void OnClickMakeCoinBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenBuyPanel(1);
    }
    public void OnClickChatBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenChatPanel();
    }
    public void OnClickTaskRewardBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenTaskRewardPanel();
    }
   
    public void OnClickMissonBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.EnterMission();
    }

    //轮盘点击事件
    public void RegisterTouchEvents()
    {
        
        OnClickDown(imgTouch.gameObject, (PointerEventData evt) =>
         {
             startPosition = evt.position;
             SetActive(imgDirPoint);
             imgDirBg.transform.position = evt.position;
         });
        OnClickUp(imgTouch.gameObject, (PointerEventData evt) =>
         {
             imgDirBg.transform.position = defaultPosition;
             SetActive(imgDirPoint, false);
             imgDirPoint.transform.localPosition = Vector2.zero;

             MainCitySys.Instance.SetMoveDir(Vector2.zero);

         });
        OnClickDrag(imgTouch.gameObject,(PointerEventData evt)=> {
            Vector2 dir = evt.position - startPosition;
            float len = dir.magnitude;

            if (len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPosition + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evt.position;
            }
            MainCitySys.Instance.SetMoveDir(dir.normalized);
        });
    }

    #endregion
}