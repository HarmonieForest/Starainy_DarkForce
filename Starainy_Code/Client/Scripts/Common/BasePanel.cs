/****************************************************
    文件：BasePanel.cs
	作者：Harmonie
	功能：UI界面基类
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour 
{
    protected ResSvc resSvc=null;
    protected AudioSvc audioSvc = null;
    protected NetSvc netSvc = null;
    protected TimerSvc timerSvc = null;
    public void SetPanelState(bool isActive=true)
    {
        if (gameObject.activeSelf != isActive)
        {
            SetActive(gameObject,isActive);
        }
        if (isActive)
        {
            InitPanel();
        }
        else
        {
            ClearPanel();
        }
    }
    public bool GetPanelState()
    {
        return gameObject.activeSelf;
    }
    protected virtual void InitPanel()
    {
        //获取资源加载的引用
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }
    protected virtual void ClearPanel()
    {
        //清空资源
        resSvc = null;
        audioSvc = null;
        netSvc = null;
        timerSvc = null;
    }

    #region Tool Functions
    protected void SetActive(GameObject go,bool isActive = true)
    {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans, bool state = true) 
    { 
        trans.gameObject.SetActive(state); 
    }
    protected void SetActive(RectTransform rectTrans, bool state = true) 
    { 
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true) 
    { 
        img.transform.gameObject.SetActive(state);
    }
    protected void SetActive(Text txt, bool state = true) 
    { 
        txt.transform.gameObject.SetActive(state);
    }
    protected void SetText(Text txt,string context="")
    {
        txt.text = context;
    }
    protected void SetText(Transform trans,int num=0)
    {
        SetText(trans.GetComponent<Text>(), num);
    }
    protected void SetText(Transform trans,string context="")
    {
        SetText(trans.GetComponent<Text>(), context);
    }
    protected void SetText(Text txt,int num=0)
    {
        SetText(txt, num.ToString());
    }


    //判断是获取组件还是添加组件
    protected T GetOrAddComponent<T>(GameObject go) where T:Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }
    protected void SetSprite(Image img,string path)
    {
        Sprite sp= resSvc.LoadSprite(path,true);
        img.sprite = sp;
    }
    #endregion

    #region ClickEvents
    protected void OnClickDown(GameObject go,Action<PointerEventData> cb)//cb:回调事件
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClickDown = cb;
    }
    protected void OnClickUp(GameObject go, Action<PointerEventData> cb)//cb:回调事件
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClickUp = cb;
    }
    protected void OnClickDrag(GameObject go, Action<PointerEventData> cb)//cb:回调事件
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClickDrag = cb;
    }
    protected void OnClick(GameObject go, Action<object> cb,object args)//cb:回调事件
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClick = cb;
        listener.args = args;
    }
    #endregion
    protected Transform GetTrans(Transform trans,string name)
    {
        if (trans != null)
        {
            return trans.Find(name);
        }
        else
        {
            return transform.Find(name);
        }
    }

}