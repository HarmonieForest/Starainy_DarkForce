/****************************************************
    文件：PlayerCtrlPanel.cs
	作者：Harmonie
	功能：玩家控制面板
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCtrlPanel : BasePanel 
{
    public Text txtLevel;
    public Text txtName;
    public Text txtHP;
    public Image imgSelfHP;

    public Text txtExpPrg;
    public Transform expTrans;

    public Text txtSelfHP;
 

    //轮盘控制组件
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    private float pointDis;

    private Vector2 startPosition = Vector2.zero;
    private Vector2 defaultPosition = Vector2.zero;

    public Vector2 curtDir;

    private int HPSum;
    protected override void InitPanel()
    {
        base.InitPanel();
        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOpDis;
        SetActive(imgDirPoint, false);
        defaultPosition = imgDirBg.transform.position;
        RegisterTouchEvents();
        sk1CDTime = resSvc.GetSkillCfg(101).cdTime / 1000.0f;
        sk2CDTime = resSvc.GetSkillCfg(102).cdTime / 1000.0f;
        sk3CDTime = resSvc.GetSkillCfg(103).cdTime / 1000.0f;

        HPSum = GameRoot.Instance.PlayerData.hp;
        SetText(txtSelfHP, HPSum+"/"+HPSum);
        imgSelfHP.fillAmount = 1;

        SetBossHPBar(false);
        RefreshUI();
    }


    private void Update()
    {
        #region  技能cd
if (Input.GetKeyDown(KeyCode.W))
        {
            OnClickSkill1();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnClickSkill2();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnClickSkill3();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnClickNormalAttk();
        }


        float delta = Time.deltaTime;
        if (IsSk1CD)
        {
            sk1FillCount += delta;
            if (sk1FillCount >= sk1CDTime)
            {
                IsSk1CD = false;
                SetActive(imgCD1, false);
                sk1FillCount = 0;
            }
            else
            {
                imgCD1.fillAmount = 1 - sk1FillCount * 1.0f / sk1CDTime;
            }
            sk1NumCount += delta;
            if (sk1NumCount >= 1.0f)
            {
                sk1NumCount -= 1;
                sk1Num -= 1;
                SetText(txtCD1, sk1Num);
            }
        }
        if (IsSk2CD)
        {
            sk2FillCount += delta;
            if (sk2FillCount >= sk2CDTime)
            {
                IsSk2CD = false;
                SetActive(imgCD2, false);
                sk2FillCount = 0;
            }
            else
            {
                imgCD2.fillAmount = 1 - sk2FillCount * 1.0f / sk2CDTime;
            }
            sk2NumCount += delta;
            if (sk2NumCount >= 1.0f)
            {
                sk2NumCount -= 1;
                sk2Num -= 1;
                SetText(txtCD2, sk2Num);
            }
        }
        if (IsSk3CD)
        {
            sk3FillCount += delta;
            if (sk3FillCount >= sk3CDTime)
            {
                IsSk3CD = false;
                SetActive(imgCD3, false);
                sk3FillCount = 0;
            }
            else
            {
                imgCD3.fillAmount = 1 - sk3FillCount * 1.0f / sk3CDTime;
            }
            sk3NumCount += delta;
            if (sk3NumCount >= 1.0f)
            {
                sk3NumCount -= 1;
                sk3Num -= 1;
                SetText(txtCD3, sk3Num);
            }
        }
        #endregion

        if (transBossHPBar.gameObject.activeSelf)
        {
            UpdateMixBlend();
            imgYellow.fillAmount = curtPrg;
        }
    }


    public void RefreshUI()
    {
        #region 游戏人物数据
        PlayerData playerData = GameRoot.Instance.PlayerData;
        
        SetText(txtLevel, playerData.lv);
        SetText(txtName, playerData.name);
        //待做
     
        #endregion
        #region 底部经验条显示
        GridLayoutGroup grid = expTrans.GetComponent<GridLayoutGroup>();
        float globalRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;
        float screenWidth = Screen.width * globalRate;
        float width = (screenWidth - 166) / 10;
        grid.cellSize = new Vector2(width, 7);

        int expPrgVal = (int)(playerData.exp * 1.0f / PECommon.GetNextLevelExp(playerData.lv) * 100);
        SetText(txtExpPrg, expPrgVal + "%");
        int index = expPrgVal / 10;
        //遍历每一个
        for (int i = 0; i < expTrans.childCount; i++)
        {
            Image img = expTrans.GetChild(i).GetComponent<Image>();
            if (i < index)
            {
                img.fillAmount = 1;
            }
            else if (i == index)
            {
                img.fillAmount = expPrgVal % 10 * 1.0f / 10;
            }
            else if (i > index)
            {
                img.fillAmount = 0;
            }
        }
        #endregion       
    }

    public Transform transBossHPBar;
    public Image imgRed;
    public Image imgYellow;
    private float curtPrg=1f;
    private float targetPrg=1f;
    public void SetBossHPBar(bool state,float prg=1)
    {
        SetActive(transBossHPBar, state);
        imgRed.fillAmount = prg;
        imgYellow.fillAmount = prg;
    }
    public void SetBossHPBarVal(int oldval,int newval,int sumval)
    {
        curtPrg = oldval * 1.0f / sumval;
        targetPrg = newval * 1.0f / sumval;
        imgRed.fillAmount = targetPrg;

    }
    private void UpdateMixBlend()
    {
        if (Mathf.Abs(curtPrg - targetPrg) < Constants.AccelerHPSpeed * Time.deltaTime)
        {
            curtPrg = targetPrg;
        }
        else if (curtPrg > targetPrg)
        {
            curtPrg -= Constants.AccelerHPSpeed * Time.deltaTime;
        }
        else
        {
            curtPrg += Constants.AccelerHPSpeed * Time.deltaTime;
        }
    }

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
            curtDir = Vector2.zero;
            BattleSys.Instance.SetSelfPlayerMoveDirection(curtDir);
           

        });
        OnClickDrag(imgTouch.gameObject, (PointerEventData evt) =>
        {
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
            curtDir = dir.normalized;
            BattleSys.Instance.SetSelfPlayerMoveDirection(curtDir);
        });
    }
    #region skill release
    public void OnClickNormalAttk()
    {
        BattleSys.Instance.ReqReleaseSkill(0);
    }
    public Image imgCD1;
    public Text txtCD1;
    private bool IsSk1CD=false;
    private float sk1CDTime;
    private int sk1Num;
    private float sk1FillCount=0;
    private float sk1NumCount = 0;
    public void OnClickSkill1()
    {
        if (!IsSk1CD&&GetCanReleaseSkill())
        {          
            BattleSys.Instance.ReqReleaseSkill(1);

            IsSk1CD = true;
            SetActive(imgCD1);
            imgCD1.fillAmount = 1;
            sk1Num = (int)sk1CDTime;
            SetText(txtCD1, sk1Num);
        }
        
    }
    public Image imgCD2;
    public Text txtCD2;
    private bool IsSk2CD = false;
    private float sk2CDTime;
    private int sk2Num;
    private float sk2FillCount = 0;
    private float sk2NumCount = 0;
    public void OnClickSkill2()
    {
        if (!IsSk2CD&&GetCanReleaseSkill())
        {
            BattleSys.Instance.ReqReleaseSkill(2);

            IsSk2CD = true;
            SetActive(imgCD2);
            imgCD2.fillAmount = 1;
            sk2Num = (int)sk2CDTime;
            SetText(txtCD2, sk2Num);
        }

    }
    public Image imgCD3;
    public Text txtCD3;
    private bool IsSk3CD = false;
    private float sk3CDTime;
    private int sk3Num;
    private float sk3FillCount = 0;
    private float sk3NumCount = 0;
    public void OnClickSkill3()
    {
        if (!IsSk3CD&&GetCanReleaseSkill())
        {
            BattleSys.Instance.ReqReleaseSkill(3);

            IsSk3CD = true;
            SetActive(imgCD3);
            imgCD3.fillAmount = 1;
            sk3Num = (int)sk3CDTime;
            SetText(txtCD3, sk3Num);
        }

    }
    public void ClickResetCfg()
    {
        resSvc.ResetCfg();
    }
    public void OnClickBtnHead()
    {
        BattleSys.Instance.battleMng.isPauseGame = true;
        BattleSys.Instance.SetBattleEndPanelState(MissionEndType.Pause);
    }
    public void SetCurtSelfHP(int val)
    {
        SetText(txtSelfHP, val + "/" + HPSum);
        imgSelfHP.fillAmount = val * 1.0f / HPSum;
    }
    public bool GetCanReleaseSkill()
    {
        return BattleSys.Instance.battleMng.CanReleaseSkill();
    }
    #endregion
}