/****************************************************
    文件：BaseSystem.cs
	作者：Harmonie
	功能：业务系统基类
*****************************************************/

using System.Collections;
using UnityEngine;

public class BaseSystem : MonoBehaviour 
{
    protected ResSvc resSvc;
    protected AudioSvc audioSvc;
    protected NetSvc netSvc;
    protected TimerSvc timerSvc;

    public virtual void InitSys()
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        timerSvc = TimerSvc.Instance;

       
    }

    ////自己写的解决posoition问题的方法
    //protected IEnumerator ApplyRootMotions(GameObject obj)
    //{
    //    yield return new WaitForSeconds(0.003f);
    //    Animator ani = obj.GetComponent<Animator>();
    //    ani.applyRootMotion = true;
    //}
}