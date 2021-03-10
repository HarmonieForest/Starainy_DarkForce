/****************************************************
	文件：EntityMonster.cs
	作者：Harmonie
	邮箱: 3062831636@qq.com
	日期：2020/05/14 19:39   	
	功能：怪物逻辑实体
*****************************************************/
using UnityEngine;

public class EntityMonster : EntityBase
{
	public MonsterData md;
	private float checkTime = 2;
	private float checkCountTime = 0;

	private float atkTime = 2;
	private float atkCountTime = 0;

	public EntityMonster()
	{
		entityType = EntityType.Monster;
	}
	public override void SetBattleProps(BattleProps props)
	{
		int level = md.mLevel;
		BattleProps p = new BattleProps
		{
			hp = props.hp * level,
			ad = props.ad * level,
			ap = props.ap * level,
			addef = props.addef * level,
			apdef = props.apdef * level,
			critical = props.critical * level,
			pierce = props.critical * level,
			dodge = props.dodge * level,
		};
		Props = p;
		HP = p.hp;
	}
	bool runAI = true;
	public override void TickAILogic()
	{
		if (!runAI)
		{
			return;
		}
		if (curtState == AniState.Idle || curtState == AniState.Move)
		{
			if (battleMng.isPauseGame)
			{
				Idle();
				return;
			}
            float delta = Time.deltaTime;
            checkCountTime += delta;
            if (checkCountTime < checkTime)
            {
                return;
            }
            else
            {
                Vector2 dir = CalTargetDir();
                if (!InAtkRange())
                {
                    SetDir(dir);
                    Move();
                }
                else
                {
                    SetDir(Vector2.zero);
                    atkCountTime += checkCountTime;
                    if (atkCountTime >= atkTime)
                    {
                        SetAtkRotation(dir);
                        Attack(md.monsterCfg.skillID);
                        atkCountTime = 0;
                    }
                    else
                    {
                        Idle();
                    }
                }
				checkCountTime = 0;
				checkTime = PETools.RDInt(1, 5) * 1.0f / 10;
            }
        }
		
	}
	public override Vector2 CalTargetDir()
	{
		EntityPlayer entityPlayer = battleMng.entitySelfPlayer;
		if (entityPlayer == null || entityPlayer.curtState == AniState.Die)
		{
			runAI = false;
			return Vector2.zero;
		}
		else
		{
			Vector3 target = entityPlayer.GetPos();
			Vector3 self = GetPos();
			return new Vector2(target.x - self.x, target.z - self.z);
		}
	}

	private bool InAtkRange()
	{
		EntityPlayer entityPlayer = battleMng.entitySelfPlayer;
        if (entityPlayer == null || entityPlayer.curtState == AniState.Die)
        {
            runAI = false;
            return false;
        }
        else
        {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
			target.y = 0;
			self.y = 0;
			float dis = Vector3.Distance(target, self);
			if (dis <= md.monsterCfg.atkDis)
			{
				return true;
			}
			else
			{
				return false;
			}
        }
    }
	public override bool GetBreakState()
	{
        if (md.monsterCfg.isStop)
        {
			if (curtSkillCfg != null)
			{
				return curtSkillCfg.isBreak;
			}
			else
			{
				return true;
			}
		}
		return false;

    }
	public override void SetHPVal(int oldVal, int newVal)
	{
		if (md.monsterCfg.monsterType == MonsterType.Boss)
		{
			BattleSys.Instance.playerCtrlPanel.SetBossHPBarVal(oldVal, newVal,Props.hp);
		}
		else
		{
			base.SetHPVal(oldVal, newVal);
		}
	}
}

