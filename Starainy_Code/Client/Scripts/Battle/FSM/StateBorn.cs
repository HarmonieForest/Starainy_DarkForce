/****************************************************
	文件：StateBorn.cs
	作者：Harmonie	
	功能：出生动画
*****************************************************/
public class StateBorn : IState
{
	public void Enter(EntityBase entity, params object[] args)
	{
		entity.curtState = AniState.Born;
	}

	public void Exit(EntityBase entity, params object[] args)
	{
		
	}

	public void Process(EntityBase entity, params object[] args)
	{
		entity.SetAction(Constants.ActionBorn);
		TimerSvc.Instance.AddTimeTask((int tid) =>
		{
			entity.SetAction(Constants.ActionDefault);
		},500);
	}
}

