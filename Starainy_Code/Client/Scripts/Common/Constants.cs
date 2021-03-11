/****************************************************
    文件：Constants.cs
	作者：Harmonie
	功能：常量配置
*****************************************************/

using System.CodeDom;
using UnityEngine;

public enum TxtColor
{
    Red,
    Green,
    Blue,
    Yellow
}
public enum DamageType
{
    None,
    AD=1,
    AP=2,
}
public enum EntityType
{
    None,
    Player,
    Monster,
}
public enum EntityState
{
    None,
    BatiState,
}
public enum MonsterType
{
    None,
    Normal=1,
    Boss=2,
}
public class Constants 
{
    //游戏tips字体颜色设置
    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF00FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";
    public static string Color(string str, TxtColor c)
    {
        string result = "";
        switch (c)
        {
            case TxtColor.Red:
                result = ColorRed + str + ColorEnd;
                break;
            case TxtColor.Green:
                result = ColorGreen + str + ColorEnd;
                break;
            case TxtColor.Blue:
                result = ColorBlue + str + ColorEnd;
                break;
            case TxtColor.Yellow:
                result = ColorYellow + str + ColorEnd;
                break;
        }
        return result;
    }
    //场景名称id
    public const string LoginScene = "LoginScene";
    //public const string MainCityScene = "MainCityScene";
    public const int MainCityMapID = 10000;

    public const string AssassinCityName = "AssassinCity";

    //音效名称
    public const string BgLogin = "bgLogin";
    public const string BgMainCity = "bgMainCity";
    public const string BgHuangye = "bgHuangYe";

    public const string AssassinHit = "assassin_Hit";
    //登录按钮音效
    public const string UILoginBtn = "uiLoginBtn";
    //常规UI点击音效
    public const string UIClickBtn = "uiClickBtn";
    public const string UiExtenBtn = "uiExtenBtn";
    public const string UIOpenPage = "uiOpenPage";
    public const string FBItemEnter = "fbitem";
    public const string FBLose = "fblose";
    public const string FBWin = "fbwin";
    //屏幕标准宽高比
    public const int ScreenStandardHeight = 750;
    public const int ScreenStandardWidth = 1334;

    //摇杆点操作距离
    public const int ScreenOpDis = 70;

    //角色移动速度
    public const int PlayerMoveSpeed = 8;
    public const int MonsterMoveSpeed = 3;

    //运动平滑加速度
    public const float AccelerSpeed = 5;
    public const float AccelerHPSpeed = 0.3f;
    //action触发参数
    public const int ActionDefault = -1;
    public const int ActionBorn = 0;
    public const int ActionDie = 100;
    public const int ActionHit = 101;


    public const int DieAniLength = 5000;
    //混合参数
    public const int BlendIdle = 0;
    public const int BlendMove = 1;

    //自动导航任务id
    public const int NPCWiseman = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;

    public const int ComboSpace = 500;
}
