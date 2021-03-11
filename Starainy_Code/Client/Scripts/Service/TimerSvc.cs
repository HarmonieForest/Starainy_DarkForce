/****************************************************
    文件：TimerSvc.cs
	作者：Harmonie
	功能：计时服务
*****************************************************/


using System;

public class TimerSvc : BaseSystem
{
    public static TimerSvc Instance = null;

    private PETimer pt;
    public void InitSvc()
    {
        Instance = this;

        pt = new PETimer();

        //日志输出方式
        pt.SetLog((string info) =>
        {
            PECommon.Log(info);
        });

        PECommon.Log("Init TimerSvc.....");
    }

    public void Update()
    {
        pt.Update();
    }
    public int AddTimeTask(Action<int> callBack, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1)
    {
        return pt.AddTimeTask(callBack, delay, timeUnit, count);
    }

    public double GetNowTime()
    {
        return pt.GetMillisecondsTime();
    }
    public void DeleteTask(int tid)
    {
        pt.DeleteTimeTask(tid);
    }
}