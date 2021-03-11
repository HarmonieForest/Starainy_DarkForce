/****************************************************
    文件：StateAttack.cs
	作者：Harmonie
	功能：攻击状态
*****************************************************/

using UnityEngine;

public class StateAttack : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.curtState = AniState.Attack;
        entity.curtSkillCfg = ResSvc.Instance.GetSkillCfg((int)args[0]);
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        entity.ExitCurtSkill();
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if (entity.entityType == EntityType.Player)
        {
            entity.canReleaseSkill = false;
        }
        entity.SkillAttack((int)args[0]);
           
    }
}