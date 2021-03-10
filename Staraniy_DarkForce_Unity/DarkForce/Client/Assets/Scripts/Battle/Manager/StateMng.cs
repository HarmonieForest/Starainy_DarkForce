/****************************************************
    文件：StateMng.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/13 17:42:0
	功能：状态管理器
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class StateMng : MonoBehaviour 
{
    private Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();
    public void Init()
    {
        fsm.Add(AniState.Idle, new StateIdle());
        fsm.Add(AniState.Move, new StateMove());
        fsm.Add(AniState.Attack, new StateAttack());
        fsm.Add(AniState.Born, new StateBorn());
        fsm.Add(AniState.Die, new StateDie());
        fsm.Add(AniState.Hit, new StateHit());
        PECommon.Log("StateMng Init Done");
    }

    public void ChangeStatus(EntityBase entity,AniState targetState, params object[] args)
    {
        if (entity.curtState == targetState)
        {
            return;
        }
        if (fsm.ContainsKey(targetState))
        {
            if (entity.curtState != AniState.None)
            {
                fsm[entity.curtState].Exit(entity,args);
            }
            
            fsm[targetState].Enter(entity,args);
            fsm[targetState].Process(entity,args);
        }
    }
}