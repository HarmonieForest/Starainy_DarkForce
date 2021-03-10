/****************************************************
	文件：StateHit.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/15 14:43   	
	功能：受击状态
*****************************************************/


using UnityEngine;

public class StateHit : IState
{
	public void Enter(EntityBase entity, params object[] args)
	{
		entity.curtState = AniState.Hit;
		entity.RemoveSkillCB();
    }

	public void Exit(EntityBase entity, params object[] args)
	{

	}

	public void Process(EntityBase entity, params object[] args)
	{
		if (entity.entityType == EntityType.Player)
		{
			entity.canReleaseSkill = false;
		}
		entity.SetDir(Vector2.zero);
		entity.SetAction(Constants.ActionHit);

		if (entity.entityType == EntityType.Player)
		{
			AudioSource charAudio = entity.GetAudio();
			AudioSvc.Instance.PlayCharAudio(Constants.AssassinHit, charAudio);
		}

		TimerSvc.Instance.AddTimeTask((int tid) =>
		{
			entity.SetAction(Constants.ActionDefault);
			entity.Idle();
		}, (int)(GetHitAniLength(entity)*1000));
	}
	private float GetHitAniLength(EntityBase entity)
	{
		AnimationClip[] clips = entity.GetAniClips();
		for(int i = 0; i < clips.Length; i++)
		{
			string clipName = clips[i].name;
			if(clipName.Contains("hit")|| clipName.Contains("Hit") || clipName.Contains("HIT"))
			{
				return clips[i].length;
			}
		}

		//保护值
		return 1;
	}
}



