/****************************************************
	文件：StateDie.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/15 13:54   	
	功能：死亡状态
*****************************************************/

public class StateDie : IState
{
	public void Enter(EntityBase entity, params object[] args)
	{
		entity.curtState = AniState.Die;
		entity.RemoveSkillCB();
	}

	public void Exit(EntityBase entity, params object[] args)
	{
		
	}

	public void Process(EntityBase entity, params object[] args)
	{
		entity.SetAction(Constants.ActionDie);
        if (entity.entityType == EntityType.Monster)
        {
			entity.GetCharacterCtrl().enabled = false;
            TimerSvc.Instance.AddTimeTask((int tid) => {
				entity.SetActive(false);				
			}, Constants.DieAniLength);

		}
	}
}

