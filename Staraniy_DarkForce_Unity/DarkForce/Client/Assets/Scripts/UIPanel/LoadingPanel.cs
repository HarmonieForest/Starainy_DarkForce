/****************************************************
    文件：LoadingPanel.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/4/18 22:30:42
	功能：加载进度页面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public Text textTips;
    public Image imgFG;
    public Image imgPercent;
    public Text txtPercent;

    private float fgWidth;
    protected override void InitPanel()
    {
        base.InitPanel();

        fgWidth = imgFG.GetComponent<RectTransform>().sizeDelta.x;//获取加载进度条的长度

        SetText(textTips, "游戏Tips:游戏正式开始,请耐心等待");
        SetText(txtPercent, "0%");
        
        imgFG.fillAmount = 0;
        imgPercent.transform.localPosition = new Vector3(-545f, 0, 0);
    }  
    public void SetProgress(float percent)
    {
        SetText(txtPercent,(int)(percent * 100) + "%");
        imgFG.fillAmount = percent;

        //设置实时进度条的长度
        float posX = percent * fgWidth - 545;
        imgPercent.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }
}