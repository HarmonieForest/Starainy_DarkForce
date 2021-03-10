/****************************************************
    文件：ItemEntityHP.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/15 15:47:4
	功能：战斗血条相关
*****************************************************/

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHP : MonoBehaviour 
{
    #region ui Define
    public Image imgGray;
    public Image imgRed;

    public Animation criticalAni;
    public Animation dodgeAni;
    public Animation hpAni;

    public Text txtCritical;
    public Text txtDodge;
    public Text txtHP;
    #endregion
 
    private Transform rootTrans;
    private RectTransform rectTrans;
    private float scaleRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;
    private int hpVal;

   
    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.position);
        rectTrans.anchoredPosition = screenPos*scaleRate;

        UpdateMixBlend();
        imgGray.fillAmount = currentPrg;
    }

    private void UpdateMixBlend()
    {
        if (Mathf.Abs(currentPrg-targetPrg) < Constants.AccelerHPSpeed*Time.deltaTime)
        {
            currentPrg = targetPrg;
        }else if (currentPrg > targetPrg)
        {
            currentPrg -= Constants.AccelerHPSpeed * Time.deltaTime;
        }
        else
        {
            currentPrg += Constants.AccelerHPSpeed * Time.deltaTime;
        }
    }


    public void InitItemInfo(Transform trans,int hp)
    {
        rectTrans = transform.GetComponent<RectTransform>();
        rootTrans = trans;
        hpVal = hp;
        imgGray.fillAmount = 1;
        imgRed.fillAmount = 1;
    }

    public void SetCritical(int critical)
    {
        criticalAni.Stop();
        txtCritical.text = "暴击" + critical;
        criticalAni.Play();
    }
    public void SetDodge()
    {
        dodgeAni.Stop(); ;
        txtDodge.text = "Miss";
        dodgeAni.Play();
    }
    public void SetHurt(int damage)
    {
        hpAni.Stop();
        txtHP.text = "-" + damage;
        hpAni.Play();
    }
    private float currentPrg;
    private float targetPrg;
    public void SetHPVal(int oldVal,int newVal)
    {
        currentPrg = oldVal * 1.0f / hpVal;
        targetPrg= newVal * 1.0f / hpVal;
        
        imgRed.fillAmount = targetPrg;
    }
}