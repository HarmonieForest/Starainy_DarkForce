/****************************************************
    文件：StateIdle.cs
	作者：Harmonie
	功能：待机状态
*****************************************************/



using UnityEngine;

public class StateIdle : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.curtState = AniState.Idle;
        entity.SetDir(Vector2.zero);
        entity.skEndCB = -1;
    }

    public void Exit(EntityBase entity, params object[] args)
    {    
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if (entity.nextSkID != 0)
        {
            entity.Attack(entity.nextSkID);
        }
        else
        {
            if (entity.entityType == EntityType.Player)
            {
                entity.canReleaseSkill = true;
            }
            if (entity.GetCurtIptDir() != Vector2.zero)
            {
                entity.Move();
                entity.SetDir(entity.GetCurtIptDir());
            }
            else
            {
                entity.SetBlend(Constants.BlendIdle);
            }
        }
               
    }
}