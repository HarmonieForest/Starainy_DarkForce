/****************************************************
	文件：EntityPlayer.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/14 9:38   	
	功能：玩家实体
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer:EntityBase
{
	public EntityPlayer()
	{
		entityType = EntityType.Player;
	}
	public override Vector2 GetCurtIptDir()
	{
		return battleMng.GetCurtDirInput();
	}
	public override Vector2 CalTargetDir()
	{
		EntityMonster monster= FindCloseTarget();
		if (monster != null)
		{
			Vector3 target = monster.GetPos();
			Vector3 self = GetPos();
			Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
			return dir.normalized;
		}
		return Vector2.zero;
	}
	private EntityMonster FindCloseTarget()
	{
		List<EntityMonster> lst = battleMng.GetEntityMonsters();
		if (lst.Count == 0 || lst == null)
		{
			return null;
		}
		else
		{
			Vector3 self = GetPos();
			EntityMonster targetMonster = null;
			float dis = 0;
			for(int i = 0; i < lst.Count; i++)
			{
				Vector3 target = lst[i].GetPos();
				if (i == 0)
				{
					dis = Vector3.Distance(self, target);
					targetMonster = lst[i];
				}
				else
				{
					float calcDis = Vector3.Distance(self, target);
					if (dis > calcDis)
					{
						dis = calcDis;
						targetMonster = lst[i];
					}
				}				
			}
			return targetMonster;
		}
	}
	public override void SetHPVal(int oldVal, int newVal)
	{
		BattleSys.Instance.playerCtrlPanel.SetCurtSelfHP(newVal);
	}
	public override void SetDodge()
	{
		GameRoot.Instance.dynamicPanel.SetSelfDodge();
	}
}


