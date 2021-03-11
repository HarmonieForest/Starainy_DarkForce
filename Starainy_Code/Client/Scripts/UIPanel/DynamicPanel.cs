/****************************************************
    文件：DynamicPanel.cs
	作者：Harmonie
	功能: 动态UI元素界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicPanel : BasePanel 
{
    protected override void InitPanel()
    {
        base.InitPanel();
        SetActive(txtTips, false);
    }

    #region tips相关
    public Animation tipsAni;
    public Text txtTips;

    private bool isTipsShow = false;

    private Queue<string> tipsQue = new Queue<string>();

    private void Update()
    {
        if (tipsQue.Count > 0&&isTipsShow==false)
        {
            lock (tipsQue)
            {
                string tips = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tips);
            }
        }
    }
    public void AddTips(string tips)
    {
        //多线程使用添加锁
        lock (tipsQue)
        {
            tipsQue.Enqueue(tips);
        }
    }
    public void SetTips(string tips)
    {
        SetActive(txtTips,true);
        SetText(txtTips, tips);
        //获取动画组件
        AnimationClip aniClip = tipsAni.GetClip("TipsShowAnim");

        tipsAni.Play();

        //延时关闭激活状态
        StartCoroutine(AniPlayFin(aniClip.length, () => {
            SetActive(txtTips, false);
            isTipsShow = false;
        }));
    }
    private IEnumerator AniPlayFin(float sec,Action action)
    {
        yield return new WaitForSeconds(sec);
        //回调
        if (action != null)
        {
            action();
        }
    }
    #endregion
    #region 血条相关
    public Transform hpItemRoot;
    private Dictionary<string, ItemEntityHP> itemDic = new Dictionary<string, ItemEntityHP>();
    public void AddHPInfo(string mName,Transform trans,int hp)
    {
        ItemEntityHP item = null;
        if(itemDic.TryGetValue(mName,out item))
        {
            return;
        }
        GameObject go = resSvc.LoadPrefab(PathDefine.EntityHPItemPrefab,true);
        go.transform.SetParent(hpItemRoot);
        go.transform.localPosition = new Vector3(-1000, 0, 0);

        ItemEntityHP itemEntityHP = go.GetComponent<ItemEntityHP>();
        itemEntityHP.InitItemInfo(trans,hp);
        itemDic.Add(mName, itemEntityHP);
    }
    public void RemoveHPInfo(string mName)
    {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(mName, out item))
        {
            Destroy(item.gameObject);
            itemDic.Remove(mName);
        }      
    }
    public void SetDodge(string key)
    {
        ItemEntityHP item = null;
        if(itemDic.TryGetValue(key,out item))
        {
            item.SetDodge();
        }
    }
    public void SetCritical(string key,int critical)
    {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(key, out item))
        {
            item.SetCritical(critical);
        }
    }
    public void SetHurt(string key,int damage)
    {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(key, out item))
        {
            item.SetHurt(damage);
        }
    }
    public void SetHPVal(string key, int oldVal,int newVal)
    {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(key, out item))
        {
            item.SetHPVal(oldVal,newVal);
        }
    }
    public void RemoveAllHpItemInfo()
    {
        if (itemDic.Count > 0)
        {
            foreach(var item in itemDic)
            {
                Destroy(item.Value.gameObject);
            }
        }
        itemDic.Clear();

    }
    #endregion
    #region 玩家状态相关
    public Animation selfDodgeAni;
    public void SetSelfDodge()
    {
        selfDodgeAni.Stop();
        selfDodgeAni.Play();
    }
    #endregion
}