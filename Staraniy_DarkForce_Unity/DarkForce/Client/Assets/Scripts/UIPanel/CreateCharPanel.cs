/****************************************************
    文件：CreateCharPanel.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/4/21 19:55:38
	功能：创建角色界面
*****************************************************/

using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharPanel : BasePanel 
{
    public InputField iptName;
    protected override void InitPanel()
    {
        base.InitPanel();

        //显示一个角色名
        iptName.text = resSvc.GetNameData(false);
    }
    //随机名字点击按钮
    public void ClickRandBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        string rdName = resSvc.GetNameData(false);
        iptName.text = rdName;
    }
    //进入游戏按钮
    public void ClickEnterButtton()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        if (iptName.text != "")
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename
                {
                    name = iptName.text
                }               
            };
            netSvc.SendRequest(msg);
        }
        else
        {
            GameRoot.AddTips("当前名字不符合规范");
        }
    }
}