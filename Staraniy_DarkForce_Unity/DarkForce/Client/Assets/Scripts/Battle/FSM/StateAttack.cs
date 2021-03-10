/****************************************************
    文件：StateAttack.cs
	作者：Harmonie
    邮箱: 3062831636@qq.com
    日期：2020/5/14 11:35:5
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